// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
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
    private readonly Dictionary<string, Uri> _resolvedDialects = new(StringComparer.Ordinal);

    internal JsonSchemaDialectResolver(IStreamLoader loader, Uri baseUrl)
    {
        _loader = loader;
        _baseUrl = baseUrl;
    }

    internal async Task<IReadOnlyDictionary<Uri, Uri>> ResolveAsync(
        IEnumerable<Uri> dialects,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<Uri, Uri>();
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
        var chain = new List<Uri>();
        var current = dialect;

        for (var depth = 0; depth < MaxResolutionDepth; depth++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (JsonSchemaDialectService.IsKnownDialect(current))
            {
                foreach (var item in chain)
                {
                    _resolvedDialects[GetDialectKey(item)] = current;
                }
                return current;
            }

            var key = GetDialectKey(current);
            if (_resolvedDialects.TryGetValue(key, out var resolvedDialect))
            {
                return resolvedDialect;
            }

            if (!visited.Add(key))
            {
                return null;
            }

            chain.Add(current);
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
            foreach (var item in chain)
            {
                _resolvedDialects[GetDialectKey(item)] = current;
            }
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
            if (metaSchema is not JsonObject jsonObject ||
                !jsonObject.TryGetPropertyValue(OpenApiConstants.DollarSchema, out var dialectNode) ||
                dialectNode is not JsonValue)
            {
                return null;
            }

            return Uri.TryCreate(dialectNode.GetScalarValue(), UriKind.Absolute, out var parentDialect)
                ? parentDialect
                : null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or HttpRequestException or JsonException or OpenApiException or
                                           ArgumentException or InvalidOperationException or SecurityException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string GetDialectKey(Uri dialect)
    {
        var value = dialect.AbsoluteUri;
        return value.EndsWith("#", StringComparison.Ordinal) ? value.Substring(0, value.Length - 1) : value;
    }
}
