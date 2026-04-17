// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#pragma warning disable OAI020 // Internal implementation uses experimental APIs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Provides helper methods for converting OpenAPI JSON Pointer paths between specification versions.
/// </summary>
[Experimental("OAI020", UrlFormat = "https://aka.ms/openapi/net/experimental/{0}")]
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
            new V30UnsupportedPathPolicy(),
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

        // Parse once, share across all policies.
        var segments = GetSegments(path);
        if (segments.Length == 0)
        {
            return path;
        }

        string? versionedPath = null;
        if (matchingPolicies.Any(policy => policy.TryGetVersionedPath(segments, out versionedPath)))
        {
            return versionedPath;
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

        // Work on the original string directly to avoid an extra allocation from span.ToString().
        var startIndex = path.StartsWith("#/", StringComparison.Ordinal) ? 2 : 0;
        if (startIndex >= path.Length)
        {
            return [];
        }

        return path.Substring(startIndex).Split('/');
    }

    /// <summary>
    /// Rebuilds a JSON Pointer path from segments, allocating only one string.
    /// </summary>
    /// <param name="segments">The segment buffer.</param>
    /// <param name="length">The number of segments to use from the buffer.</param>
    internal static string BuildPath(string[] segments, int length)
    {
#if NET8_0_OR_GREATER
        // Pre-calculate total length: "#/" + segments joined by "/"
        var totalLength = 2; // "#/"
        for (var i = 0; i < length; i++)
        {
            if (i > 0)
            {
                totalLength++; // "/"
            }

            totalLength += segments[i].Length;
        }

        return string.Create(totalLength, (segments, length), static (span, state) =>
        {
            span[0] = '#';
            span[1] = '/';
            var pos = 2;
            for (var i = 0; i < state.length; i++)
            {
                if (i > 0)
                {
                    span[pos++] = '/';
                }

                state.segments[i].AsSpan().CopyTo(span.Slice(pos));
                pos += state.segments[i].Length;
            }
        });
#else
        var sb = new System.Text.StringBuilder(2 + length * 8);
        sb.Append("#/");
        for (var i = 0; i < length; i++)
        {
            if (i > 0)
            {
                sb.Append('/');
            }

            sb.Append(segments[i]);
        }

        return sb.ToString();
#endif
    }

    /// <summary>
    /// Copies segments into the target buffer, skipping a contiguous range.
    /// Returns the number of segments written.
    /// </summary>
    internal static int CopySkipping(string[] source, int sourceLength, string[] target, int skipStart, int skipCount)
    {
        var written = 0;
        for (var i = 0; i < sourceLength; i++)
        {
            if (i >= skipStart && i < skipStart + skipCount)
            {
                continue;
            }

            target[written++] = source[i];
        }

        return written;
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

    // Segments that are always unsupported regardless of position (except index 0 which is checked separately).
    private static readonly HashSet<string> UnsupportedSegments = new(StringComparer.Ordinal)
    {
        OpenApiConstants.Servers,
        OpenApiConstants.Callbacks,
        OpenApiConstants.Links,
        OpenApiConstants.RequestBody,
    };

    public bool TryGetVersionedPath(string[] segments, out string? result)
    {
        result = null;

        if (segments.Length == 0)
        {
            return false;
        }

        // Top-level: #/servers/** or #/webhooks/**
        if (string.Equals(segments[0], OpenApiConstants.Servers, StringComparison.Ordinal) ||
            string.Equals(segments[0], OpenApiConstants.Webhooks, StringComparison.Ordinal))
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

        // Walk through segments looking for v3-only constructs
        for (var i = 1; i < segments.Length; i++)
        {
            var segment = segments[i];

            // servers, callbacks, links, requestBody at any nested level
            if (UnsupportedSegments.Contains(segment))
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
}

/// <summary>
/// Returns null for paths that have no equivalent in OpenAPI v3.0.
/// Covers: webhooks (added in v3.1).
/// </summary>
internal sealed class V30UnsupportedPathPolicy : IOpenApiPathRepresentationPolicy
{
    public bool TryGetVersionedPath(string[] segments, out string? result)
    {
        result = null;

        if (segments.Length > 0 &&
            string.Equals(segments[0], OpenApiConstants.Webhooks, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }
}

/// <summary>
/// Renames v3 component paths to their v2 equivalents and applies nested transformations
/// (content unwrapping, header schema unwrapping) in a single pass.
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

    public bool TryGetVersionedPath(string[] segments, out string? result)
    {
        result = null;

        if (segments.Length < 2 ||
            !string.Equals(segments[0], OpenApiConstants.Components, StringComparison.Ordinal) ||
            !ComponentMappings.TryGetValue(segments[1], out var v2Name))
        {
            return false;
        }

        // Build the transformed path in one pass:
        // - Skip "components" (index 0), replace component type (index 1) with v2 name
        // - Apply content unwrapping and header schema unwrapping inline
        var buffer = new string[segments.Length]; // upper bound
        var written = 0;
        buffer[written++] = v2Name;

        for (var i = 2; i < segments.Length; i++)
        {
            // Content unwrapping: skip "content" and "{mediaType}" after "responses/{code}"
            if (string.Equals(segments[i], OpenApiConstants.Content, StringComparison.Ordinal) &&
                i >= 3 &&
                string.Equals(segments[i - 2], OpenApiConstants.Responses, StringComparison.Ordinal) &&
                i + 1 < segments.Length)
            {
                i++; // skip mediaType too
                continue;
            }

            // Header schema unwrapping: skip "schema" after "headers/{name}"
            if (string.Equals(segments[i], OpenApiConstants.Schema, StringComparison.Ordinal) &&
                i >= 3 &&
                string.Equals(segments[i - 2], OpenApiConstants.Headers, StringComparison.Ordinal))
            {
                continue;
            }

            buffer[written++] = segments[i];
        }

        result = OpenApiPathHelper.BuildPath(buffer, written);
        return true;
    }
}

/// <summary>
/// Unwraps response content media type from v3 paths to v2 paths.
/// <c>.../responses/{code}/content/{mediaType}/schema/**</c> → <c>.../responses/{code}/schema/**</c>
/// </summary>
internal sealed class V2ResponseContentUnwrappingPolicy : IOpenApiPathRepresentationPolicy
{
    public bool TryGetVersionedPath(string[] segments, out string? result)
    {
        result = null;

        // Find: responses / {code} / content / {mediaType}
        var contentIndex = -1;
        for (var i = 0; i < segments.Length - 3; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Responses, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Content, StringComparison.Ordinal))
            {
                contentIndex = i + 2;
                break;
            }
        }

        if (contentIndex < 0)
        {
            return false;
        }

        // Remove "content" and "{mediaType}" — copy segments skipping those two
        var buffer = new string[segments.Length - 2];
        var written = OpenApiPathHelper.CopySkipping(segments, segments.Length, buffer, contentIndex, 2);
        result = OpenApiPathHelper.BuildPath(buffer, written);
        return true;
    }
}

/// <summary>
/// Unwraps the "schema" segment from header paths in v3 to produce v2-style header paths.
/// <c>.../headers/{name}/schema/**</c> → <c>.../headers/{name}/**</c>
/// </summary>
internal sealed class V2HeaderSchemaUnwrappingPolicy : IOpenApiPathRepresentationPolicy
{
    public bool TryGetVersionedPath(string[] segments, out string? result)
    {
        result = null;

        // Find: headers / {name} / schema
        var schemaIndex = -1;
        for (var i = 0; i < segments.Length - 2; i++)
        {
            if (string.Equals(segments[i], OpenApiConstants.Headers, StringComparison.Ordinal) &&
                string.Equals(segments[i + 2], OpenApiConstants.Schema, StringComparison.Ordinal))
            {
                schemaIndex = i + 2;
                break;
            }
        }

        if (schemaIndex < 0)
        {
            return false;
        }

        // Remove "schema" — copy segments skipping that one
        var buffer = new string[segments.Length - 1];
        var written = OpenApiPathHelper.CopySkipping(segments, segments.Length, buffer, schemaIndex, 1);
        result = OpenApiPathHelper.BuildPath(buffer, written);
        return true;
    }
}
