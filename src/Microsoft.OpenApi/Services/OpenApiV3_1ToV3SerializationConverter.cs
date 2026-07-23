// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Converts OpenAPI 3.1 schema constructs into OpenAPI 3.0 serialization-compatible constructs.
/// </summary>
[Experimental("OPENAPI001")]
public sealed class OpenApiV3_1ToV3SerializationConverter : OpenApiVisitorBase
{
    /// <summary>
    /// Applies the conversion to an OpenAPI document.
    /// </summary>
    /// <param name="document">The document to convert.</param>
    public void Convert(OpenApiDocument document)
    {
        Utils.CheckArgumentNull(document);
        new OpenApiWalker(this).Walk(document);
    }

    /// <summary>
    /// Applies the conversion to a schema.
    /// </summary>
    /// <param name="schema">The schema to convert.</param>
    public void Convert(IOpenApiSchema schema)
    {
        Utils.CheckArgumentNull(schema);
        new OpenApiWalker(this).Walk(schema);
    }

    /// <inheritdoc />
    public override void Visit(IOpenApiSchema schema)
    {
        if (schema is OpenApiSchema concreteSchema && ShouldConvertNullTypeToNullEnum(concreteSchema))
        {
            concreteSchema.Enum = new List<JsonNode>
            {
                JsonNullSentinel.JsonNull
            };
            concreteSchema.Type = null;
        }
    }

    private static bool ShouldConvertNullTypeToNullEnum(OpenApiSchema schema) =>
        schema.Type == JsonSchemaType.Null &&
        schema.OneOf is not { Count: > 0 } &&
        schema.AnyOf is not { Count: > 0 } &&
        schema.AllOf is not { Count: > 0 } &&
        schema.Enum is not { Count: > 0 };
}
