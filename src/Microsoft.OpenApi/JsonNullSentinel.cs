// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// A sentinel value representing JSON null. 
/// This can only be used for OpenAPI properties of type <see cref="JsonNode"/>
/// </summary>
public static class JsonNullSentinel
{
    private const string SentinelValue = "openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E464";
    private static readonly JsonValue SentinelJsonValue = JsonValue.Create(SentinelValue)!;
    /// <summary>
	/// A sentinel value representing JSON null. 
    /// This can only be used for OpenAPI properties of type <see cref="JsonNode"/>.
    /// This can only be used for the root level of a JSON structure.
    /// Any use outside of these constraints is unsupported and may lead to unexpected behavior.
    /// Because this is returning a cloned instance, so the value can be added in a tree, reference equality checks will not work.
    /// You must use the <see cref="IsJsonNullSentinel(JsonNode?)"/> method to check for this sentinel.
	/// </summary>
    public static JsonValue JsonNull => (JsonValue)SentinelJsonValue.DeepClone();

    /// <summary>
    /// Determines if the given node is the JSON null sentinel.
    /// </summary>
    /// <param name="node">The JsonNode to check.</param>
    /// <returns>Whether or not the given node is the JSON null sentinel.</returns>
    public static bool IsJsonNullSentinel(this JsonNode? node)
    {
        return node is JsonValue jsonValue &&
                jsonValue.GetValueKind() == JsonValueKind.String &&
                jsonValue.TryGetValue<string>(out var value) &&
                SentinelValue.Equals(value, StringComparison.Ordinal);
    }
}
