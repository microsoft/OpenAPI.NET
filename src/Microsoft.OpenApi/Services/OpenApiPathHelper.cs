// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Provides helper methods for converting OpenAPI JSON Pointer paths between specification versions.
/// </summary>
public static class OpenApiPathHelper
{
    private static readonly Dictionary<OpenApiSpecVersion, IOpenApiPathRepresentationPolicy[]> _policies = new()
    {
        [OpenApiSpecVersion.OpenApi2_0] =
        [
            // Order matters: null policies first, then transformations.
            new V2UnsupportedPathPolicy(),
            new V2ComponentRenamePolicy(),
            new V2ResponseContentUnwrappingPolicy(),
            new V2HeaderSchemaUnwrappingPolicy(),
        ],
        [OpenApiSpecVersion.OpenApi3_0] =
        [
            new V3_0UnsupportedPathPolicy(),
        ],
    };

    /// <summary>
    /// Converts a JSON Pointer path produced by the walker (latest version) to its equivalent
    /// for the specified target specification version.
    /// </summary>
    /// <param name="path">The latest version JSON Pointer path (e.g. <c>#/paths/~1items/get/responses/200/content/application~1json/schema</c>).</param>
    /// <param name="targetVersion">The target OpenAPI specification version.</param>
    /// <returns>
    /// The equivalent path in the target version, the original path if no transformation is needed,
    /// or <c>null</c> if the path has no equivalent in the target version.
    /// </returns>
    public static string? GetVersionedPath(string path, OpenApiSpecVersion targetVersion)
    {
        if (string.IsNullOrEmpty(path) || targetVersion == OpenApiSpecVersion.OpenApi3_2)
        {
            return path;
        }

        if (!_policies.TryGetValue(targetVersion, out var matchingPolicies))
        {
            return path;
        }

        foreach (var policy in matchingPolicies)
        {
            if (policy.IsMatch(path))
            {
                return policy.GetVersionedPath(path);
            }
        }

        return path;
    }

    /// <summary>
    /// Splits a JSON Pointer path into its constituent segments, stripping the <c>#/</c> prefix.
    /// </summary>
    internal static string[] GetSegments(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return [];
        }

        ReadOnlySpan<char> span = path.AsSpan();
        if (span.StartsWith("#/".AsSpan(), StringComparison.Ordinal))
        {
            span = span.Slice(2);
        }

        if (span.IsEmpty)
        {
            return [];
        }

        return span.ToString().Split('/');
    }

    /// <summary>
    /// Rebuilds a JSON Pointer path from segments.
    /// </summary>
    internal static string BuildPath(string[] segments)
    {
        return "#/" + string.Join("/", segments);
    }

    /// <summary>
    /// Removes segments at the specified indices by building a new array without them.
    /// </summary>
    internal static string[] RemoveSegments(string[] segments, int startIndex, int count)
    {
        var result = new string[segments.Length - count];
        Array.Copy(segments, 0, result, 0, startIndex);
        Array.Copy(segments, startIndex + count, result, startIndex, segments.Length - startIndex - count);
        return result;
    }
}

/// <summary>
/// Returns null for paths that have no equivalent in OpenAPI v2 (Swagger).
/// Covers: servers, webhooks, callbacks, links, requestBody (inline),
/// encoding, and unsupported component types.
/// </summary>
internal sealed class V2UnsupportedPathPolicy : IOpenApiPathRepresentationPolicy
{
    private static readonly HashSet<string> UnsupportedComponentTypes = new(StringComparer.Ordinal)
    {
        OpenApiConstants.Examples,
        OpenApiConstants.Headers,
        OpenApiConstants.PathItems,
        OpenApiConstants.Links,
        OpenApiConstants.Callbacks,
        OpenApiConstants.RequestBodies,
        OpenApiConstants.MediaTypes,
    };

    public bool IsMatch(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        if (segments.Length == 0)
        {
            return false;
        }

        // Top-level servers: #/servers/**
        if (string.Equals(segments[0], OpenApiConstants.Servers, StringComparison.Ordinal))
        {
            return true;
        }

        // Top-level webhooks: #/webhooks/**
        if (string.Equals(segments[0], OpenApiConstants.Webhooks, StringComparison.Ordinal))
        {
            return true;
        }

        // Unsupported component types: #/components/{unsupported}/**
        if (segments.Length >= 2 &&
            string.Equals(segments[0], OpenApiConstants.Components, StringComparison.Ordinal) &&
            UnsupportedComponentTypes.Contains(segments[1]))
        {
            return true;
        }

        // Walk through segments looking for v3-only constructs in context
        for (var i = 1; i < segments.Length; i++)
        {
            var segment = segments[i];

            // servers at any nested level (path-item/operation level)
            if (string.Equals(segment, OpenApiConstants.Servers, StringComparison.Ordinal))
            {
                return true;
            }

            // callbacks at operation level
            if (string.Equals(segment, OpenApiConstants.Callbacks, StringComparison.Ordinal))
            {
                return true;
            }

            // links at response level
            if (string.Equals(segment, OpenApiConstants.Links, StringComparison.Ordinal))
            {
                return true;
            }

            // inline requestBody at operation level
            if (string.Equals(segment, OpenApiConstants.RequestBody, StringComparison.Ordinal))
            {
                return true;
            }

            // encoding under content/{mediaType}: .../content/{mt}/encoding/**
            if (string.Equals(segment, OpenApiConstants.Encoding, StringComparison.Ordinal) &&
                i >= 2 &&
                string.Equals(segments[i - 2], OpenApiConstants.Content, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    public string? GetVersionedPath(string path) => null;
}

/// <summary>
/// Returns null for paths that have no equivalent in OpenAPI v3.0.
/// Covers: webhooks (added in v3.1).
/// </summary>
internal sealed class V3_0UnsupportedPathPolicy : IOpenApiPathRepresentationPolicy
{
    public bool IsMatch(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        if (segments.Length == 0)
        {
            return false;
        }

        // webhooks were added in v3.1
        return string.Equals(segments[0], OpenApiConstants.Webhooks, StringComparison.Ordinal);
    }

    public string? GetVersionedPath(string path) => null;
}

/// <summary>
/// Renames v3 component paths to their v2 equivalents.
/// <list type="bullet">
///   <item><c>#/components/schemas/{name}/**</c> → <c>#/definitions/{name}/**</c></item>
///   <item><c>#/components/parameters/{name}/**</c> → <c>#/parameters/{name}/**</c></item>
///   <item><c>#/components/responses/{name}/**</c> → <c>#/responses/{name}/**</c></item>
///   <item><c>#/components/securitySchemes/{name}/**</c> → <c>#/securityDefinitions/{name}/**</c></item>
/// </list>
/// </summary>
internal sealed class V2ComponentRenamePolicy : IOpenApiPathRepresentationPolicy
{
    private static readonly Dictionary<string, string> ComponentMappings = new(StringComparer.Ordinal)
    {
        [OpenApiConstants.Schemas] = OpenApiConstants.Definitions,
        [OpenApiConstants.Parameters] = OpenApiConstants.Parameters,
        [OpenApiConstants.Responses] = OpenApiConstants.Responses,
        [OpenApiConstants.SecuritySchemes] = OpenApiConstants.SecurityDefinitions,
    };

    public bool IsMatch(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);

        return segments.Length >= 2 &&
               string.Equals(segments[0], OpenApiConstants.Components, StringComparison.Ordinal) &&
               ComponentMappings.ContainsKey(segments[1]);
    }

    public string? GetVersionedPath(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        var v2Name = ComponentMappings[segments[1]];

        // Remove "components" (index 0) and replace the component type name (index 1)
        segments[1] = v2Name;
        var result = OpenApiPathHelper.RemoveSegments(segments, 0, 1);

        // Apply further transformations to the result (e.g., schema unwrapping within components)
        var resultPath = OpenApiPathHelper.BuildPath(result);
        return ApplyNestedTransformations(resultPath);
    }

    private static string? ApplyNestedTransformations(string path)
    {
        // After renaming, a component response might still have content/{mt}/schema
        // e.g. #/components/responses/NotFound/content/application~1json/schema →
        //       #/responses/NotFound/schema (needs content unwrapping)
        var segments = OpenApiPathHelper.GetSegments(path);
        segments = V2ResponseContentUnwrappingPolicy.UnwrapContentSegments(segments);
        segments = V2HeaderSchemaUnwrappingPolicy.UnwrapHeaderSchemaSegments(segments);
        return OpenApiPathHelper.BuildPath(segments);
    }
}

/// <summary>
/// Unwraps response content media type from v3 paths to v2 paths.
/// <c>.../responses/{code}/content/{mediaType}/schema/**</c> → <c>.../responses/{code}/schema/**</c>
/// </summary>
internal sealed class V2ResponseContentUnwrappingPolicy : IOpenApiPathRepresentationPolicy
{
    public bool IsMatch(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        return FindContentIndex(segments) >= 0;
    }

    public string? GetVersionedPath(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        segments = UnwrapContentSegments(segments);
        return OpenApiPathHelper.BuildPath(segments);
    }

    /// <summary>
    /// Finds the "content" segment that follows a "responses/{code}" sequence.
    /// Returns the index of "content", or -1 if not found.
    /// </summary>
    private static int FindContentIndex(string[] segments)
    {
        // Look for: responses / {code} / content / {mediaType}
        for (var i = 0; i < segments.Length - 3; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Responses, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Content, StringComparison.Ordinal))
            {
                return i + 2;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes the "content" and "{mediaType}" segments from a response path.
    /// </summary>
    internal static string[] UnwrapContentSegments(string[] segments)
    {
        // Look for: responses / {code} / content / {mediaType} / ...
        for (var i = 0; i < segments.Length - 3; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Responses, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Content, StringComparison.Ordinal))
            {
                // Remove "content" (i+2) and "{mediaType}" (i+3)
                return OpenApiPathHelper.RemoveSegments(segments, i + 2, 2);
            }
        }

        return segments;
    }
}

/// <summary>
/// Unwraps the "schema" segment from header paths in v3 to produce v2-style header paths.
/// <c>.../headers/{name}/schema/**</c> → <c>.../headers/{name}/**</c>
/// </summary>
internal sealed class V2HeaderSchemaUnwrappingPolicy : IOpenApiPathRepresentationPolicy
{
    public bool IsMatch(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        return FindHeaderSchemaIndex(segments) >= 0;
    }

    public string? GetVersionedPath(string path)
    {
        var segments = OpenApiPathHelper.GetSegments(path);
        segments = UnwrapHeaderSchemaSegments(segments);
        return OpenApiPathHelper.BuildPath(segments);
    }

    /// <summary>
    /// Finds the "schema" segment that follows a "headers/{name}" sequence.
    /// Returns the index of "schema", or -1 if not found.
    /// </summary>
    private static int FindHeaderSchemaIndex(string[] segments)
    {
        // Look for: headers / {name} / schema
        for (var i = 0; i < segments.Length - 2; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Headers, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Schema, StringComparison.Ordinal))
            {
                return i + 2;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes the "schema" segment from a header path.
    /// </summary>
    internal static string[] UnwrapHeaderSchemaSegments(string[] segments)
    {
        // Look for: headers / {name} / schema
        for (var i = 0; i < segments.Length - 2; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Headers, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Schema, StringComparison.Ordinal))
            {
                // Remove "schema" (i+2)
                return OpenApiPathHelper.RemoveSegments(segments, i + 2, 1);
            }
        }

        return segments;
    }
}
