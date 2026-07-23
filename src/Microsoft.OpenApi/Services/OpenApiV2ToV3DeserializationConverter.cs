// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Converts OpenAPI 2.0 deserialized schema constructs into OpenAPI 3.x constructs.
/// </summary>
[Experimental("OPENAPI001")]
public sealed class OpenApiV2ToV3DeserializationConverter : OpenApiVisitorBase
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
            ConvertSingletonStringEnumToConst(concreteSchema);
            ConvertExampleToExamples(concreteSchema);
            concreteSchema.ConvertNullableCompatibilityToNullType();
            concreteSchema.ConvertCompatibilityBooleansToExclusiveBounds();
        }
    }

    internal static void ConvertSingletonStringEnumToConst(OpenApiSchema schema)
    {
        if (!schema.WasConstExplicitlySet &&
            schema.Const is null &&
            schema.Enum is { Count: 1 } enumValues &&
            enumValues[0] is JsonValue jsonValue &&
            jsonValue.GetValueKind() == JsonValueKind.String)
        {
            schema.Const = jsonValue.GetValue<string>();
        }
    }

    internal static void ConvertExampleToExamples(OpenApiSchema schema)
    {
#pragma warning disable CS0618
        if (schema.Example is not null && schema.Examples is not { Count: > 0 })
        {
            schema.Examples = new List<JsonNode>
            {
                schema.Example
            };
        }
#pragma warning restore CS0618
    }
}
