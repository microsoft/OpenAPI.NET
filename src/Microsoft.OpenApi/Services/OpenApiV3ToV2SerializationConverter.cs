// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OpenApi;

/// <summary>
/// Converts OpenAPI 3.x schema constructs into OpenAPI 2.0 serialization-compatible constructs.
/// </summary>
[Experimental("OPENAPI001")]
public sealed class OpenApiV3ToV2SerializationConverter : OpenApiVisitorBase
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
        if (schema is OpenApiSchema concreteSchema)
        {
            OpenApiV3_1ToV3SerializationConverter.ConvertConstToEnum(concreteSchema);
            OpenApiV3_1ToV3SerializationConverter.ConvertExamplesToExampleAndExtension(concreteSchema);
            OpenApiV3_1ToV3SerializationConverter.InferFormatFromComposedSchemas(concreteSchema);
            concreteSchema.ConvertNullTypeToNullableCompatibility();
            concreteSchema.ConvertExclusiveBoundsToCompatibilityBooleans();
        }
    }
}
