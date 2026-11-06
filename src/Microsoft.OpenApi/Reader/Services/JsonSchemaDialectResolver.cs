// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Resolves unknown JSON Schema dialects to parsing rules already known by the reader.
/// </summary>
internal sealed class JsonSchemaDialectResolver
{
    internal const int MaxResolutionDepth = 16;
    internal const int MaxMetaSchemaSize = 1024 * 1024;

    private readonly IStreamLoader _loader;
    private readonly Uri _baseUrl;
    private readonly Dictionary<string, Uri?> _metaSchemaDialects = new(StringComparer.Ordinal);

    internal JsonSchemaDialectResolver(IStreamLoader loader, Uri baseUrl)
    {
        _loader = loader;
        _baseUrl = baseUrl;
    }

    internal async Task<IReadOnlyDictionary<Uri, Uri>> ResolveAsync(
        IEnumerable<Uri> dialects,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Uri, Uri>(new DialectUriEqualityComparer());
        foreach (var dialect in dialects)
        {
            if (await ResolveAsync(dialect, cancellationToken).ConfigureAwait(false) is { } knownDialect)
            {
                result[dialect] = knownDialect;
            }
        }

        return result;
    }

    private async Task<Uri?> ResolveAsync(Uri dialect, CancellationToken cancellationToken)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = dialect;

        for (var depth = 0; depth < MaxResolutionDepth; depth++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (JsonSchemaDialectService.IsKnownDialect(current))
            {
                return current;
            }

            var key = GetDialectKey(current);
            if (!visited.Add(key))
            {
                return null;
            }

            // Cached parent links still count toward this chain's depth limit.
            if (!_metaSchemaDialects.TryGetValue(key, out var parentDialect))
            {
                parentDialect = await LoadParentDialectAsync(current, cancellationToken).ConfigureAwait(false);
                _metaSchemaDialects[key] = parentDialect;
            }

            if (parentDialect is null)
            {
                return null;
            }

            current = parentDialect;
        }

        if (JsonSchemaDialectService.IsKnownDialect(current))
        {
            return current;
        }

        return null;
    }

    private async Task<Uri?> LoadParentDialectAsync(Uri dialect, CancellationToken cancellationToken)
    {
        try
        {
            using var input = await _loader.LoadAsync(_baseUrl, dialect, cancellationToken).ConfigureAwait(false);
            if (input.CanSeek && input.Length - input.Position > MaxMetaSchemaSize)
            {
                return null;
            }

            using var buffer = new MemoryStream();
            var bytes = new byte[81920];
            var totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await input.ReadAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (bytesRead > MaxMetaSchemaSize - totalBytesRead)
                {
                    return null;
                }

                await buffer.WriteAsync(bytes, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }

            buffer.Position = 0;
            var metaSchema = await JsonNode.ParseAsync(buffer, cancellationToken: cancellationToken).ConfigureAwait(false);
            return FindParentDialect(metaSchema, dialect, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
        catch (Exception exception) when (exception is IOException or HttpRequestException or JsonException or OpenApiException or
                                           ArgumentException or InvalidOperationException or SecurityException or UnauthorizedAccessException or UriFormatException)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return null;
        }
    }

    private static Uri? FindParentDialect(JsonNode? metaSchema, Uri dialect, CancellationToken cancellationToken)
    {
        var fragment = dialect.Fragment.Length == 0 ? string.Empty : Uri.UnescapeDataString(dialect.Fragment.Substring(1));
        var isPointer = fragment.StartsWith("/", StringComparison.Ordinal);
        var isAnchor = fragment.Length > 0 && !isPointer;
        if (isAnchor && (!IsAnchorName(fragment) || metaSchema is JsonObject root && HasInvalidIdentifier(root, dialect)))
        {
            return null;
        }

        Uri? result = null;
        var found = false;
        foreach (var (schema, pointer, effectiveDialect) in EnumerateSchemas(metaSchema, string.Empty, null, isAnchor, cancellationToken))
        {
            var matches = isAnchor
                ? schema is JsonObject jsonObject &&
                  (HasAnchor(jsonObject, OpenApiConstants.Anchor, fragment) || HasAnchor(jsonObject, OpenApiConstants.DynamicAnchor, fragment))
                : string.Equals(pointer, fragment, StringComparison.Ordinal);
            if (!matches)
            {
                continue;
            }

            if (found)
            {
                // Plain-name fragments must identify one schema in the current resource.
                return null;
            }

            found = true;
            result = effectiveDialect;
            if (!isAnchor)
            {
                return result;
            }
        }

        return result;
    }

    private static IEnumerable<(JsonNode Schema, string Pointer, Uri? Dialect)> EnumerateSchemas(
        JsonNode? node,
        string pointer,
        Uri? inheritedDialect,
        bool restrictToResource,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsSchema(node))
        {
            yield break;
        }

        var effectiveDialect = inheritedDialect;
        if (node is JsonObject schema)
        {
            // Anchors belong to their nearest resource root, not to enclosing resources.
            if (restrictToResource && pointer.Length > 0 && schema.TryGetPropertyValue("$id", out var identifier) &&
                (identifier is not JsonValue idValue || !idValue.TryGetValue<string>(out var id) || !string.IsNullOrEmpty(id)))
            {
                yield break;
            }

            if (schema.TryGetPropertyValue(OpenApiConstants.DollarSchema, out var dialectNode))
            {
                effectiveDialect = dialectNode is JsonValue value && value.TryGetValue<string>(out var declaredDialect) &&
                    Uri.TryCreate(declaredDialect, UriKind.Absolute, out var parentDialect) ? parentDialect : null;
            }
        }

        yield return (node!, pointer, effectiveDialect);
        if (node is not JsonObject jsonObject)
        {
            yield break;
        }

        foreach (var (child, childPointer) in EnumerateChildSchemas(jsonObject, pointer))
        {
            foreach (var descendant in EnumerateSchemas(child, childPointer, effectiveDialect, restrictToResource, cancellationToken))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<(JsonNode Schema, string Pointer)> EnumerateChildSchemas(JsonObject schema, string pointer)
    {
        // Only locations with known schema semantics are traversed. Examples and custom
        // vocabulary payloads cannot safely be interpreted as schemas or anchor declarations.
        foreach (var property in schema)
        {
            var propertyPointer = pointer + "/" + property.Key;
            switch (property.Key)
            {
                case "$defs" or "properties" or "patternProperties" or "dependentSchemas" when property.Value is JsonObject map:
                    foreach (var child in map)
                    {
                        if (IsSchema(child.Value))
                        {
                            yield return (child.Value!, propertyPointer + "/" + OpenApiVisitorBase.EncodeJsonPointerSegment(child.Key));
                        }
                    }
                    break;
                case "allOf" or "anyOf" or "oneOf" or "prefixItems" when property.Value is JsonArray array:
                    for (var index = 0; index < array.Count; index++)
                    {
                        if (IsSchema(array[index]))
                        {
                            yield return (array[index]!, propertyPointer + "/" + index.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    break;
                case "items" or "not" or "contains" or "propertyNames" or "additionalProperties" or
                     "unevaluatedProperties" or "unevaluatedItems" or "contentSchema" or "if" or "then" or "else":
                    if (IsSchema(property.Value))
                    {
                        yield return (property.Value!, propertyPointer);
                    }
                    break;
            }
        }
    }

    private static bool IsSchema(JsonNode? node) => node is JsonObject || node is JsonValue value && value.TryGetValue<bool>(out _);

    private static bool HasAnchor(JsonObject schema, string keyword, string anchor) =>
        schema.TryGetPropertyValue(keyword, out var node) && node is JsonValue value &&
        value.TryGetValue<string>(out var name) && string.Equals(name, anchor, StringComparison.Ordinal);

    private static bool HasInvalidIdentifier(JsonObject schema, Uri dialect) =>
        schema.TryGetPropertyValue("$id", out var node) &&
        (node is not JsonValue value || !value.TryGetValue<string>(out var id) ||
         !string.IsNullOrEmpty(id) && (!Uri.TryCreate(dialect, id, out var identifier) || identifier.Fragment.Length > 1));

    private static bool IsAnchorName(string name)
    {
        for (var index = 0; index < name.Length; index++)
        {
            var character = name[index];
            if (!(character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_' ||
                  index > 0 && character is >= '0' and <= '9' or '-' or '.'))
            {
                return false;
            }
        }

        return name.Length > 0;
    }

    private static string GetDialectKey(Uri dialect)
    {
        var value = dialect.AbsoluteUri;
        return value.EndsWith("#", StringComparison.Ordinal) ? value.Substring(0, value.Length - 1) : value;
    }

    private sealed class DialectUriEqualityComparer : IEqualityComparer<Uri>
    {
        public bool Equals(Uri? x, Uri? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(GetDialectKey(x), GetDialectKey(y));
        }

        public int GetHashCode(Uri obj)
        {
            return StringComparer.Ordinal.GetHashCode(GetDialectKey(obj));
        }
    }
}
