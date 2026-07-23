// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OpenApi;

/// <summary>
/// Converts OpenAPI 3.0 deserialized schema constructs into OpenAPI 3.1 constructs.
/// </summary>
[Experimental("OPENAPI001")]
public sealed class OpenApiV3ToV3_1DeserializationConverter : OpenApiVisitorBase
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
        if (schema is OpenApiSchema concreteSchema && ShouldConvertNullEnumToNullType(concreteSchema))
        {
            concreteSchema.Type = JsonSchemaType.Null;
            concreteSchema.Enum = null;
        }
    }

    private static bool ShouldConvertNullEnumToNullType(OpenApiSchema schema) =>
        schema.Type is null &&
        schema.OneOf is not { Count: > 0 } &&
        schema.AnyOf is not { Count: > 0 } &&
        schema.AllOf is not { Count: > 0 } &&
        schema.Enum is { Count: 1 } enumValues &&
        enumValues[0].IsJsonNullSentinel();
}
