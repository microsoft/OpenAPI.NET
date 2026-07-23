// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        if (schema is not OpenApiSchema concreteSchema)
        {
            return;
        }

        if (ShouldConvertNullTypeToNullEnum(concreteSchema))
        {
            concreteSchema.Enum = new List<JsonNode>
            {
                JsonNullSentinel.JsonNull
            };
            concreteSchema.Type = null;
        }
        else
        {
            concreteSchema.ConvertNullTypeToNullableCompatibility();
        }

        ConvertConstToEnum(concreteSchema);
        ConvertExamplesToExampleAndExtension(concreteSchema);
        InferFormatFromComposedSchemas(concreteSchema);
        concreteSchema.ConvertExclusiveBoundsToCompatibilityBooleans();
    }

    private static bool ShouldConvertNullTypeToNullEnum(OpenApiSchema schema) =>
        schema.Type == JsonSchemaType.Null &&
        schema.OneOf is not { Count: > 0 } &&
        schema.AnyOf is not { Count: > 0 } &&
        schema.AllOf is not { Count: > 0 } &&
        schema.Enum is not { Count: > 0 };

    internal static void ConvertConstToEnum(OpenApiSchema schema)
    {
        if (schema.Enum is not { Count: > 0 } && schema.WasConstExplicitlySet)
        {
            schema.Enum = new List<JsonNode>
            {
                JsonValue.Create(schema.Const)!
            };
        }
    }

    internal static void ConvertExamplesToExampleAndExtension(OpenApiSchema schema)
    {
#pragma warning disable CS0618
        if (schema.Examples is not { Count: > 0 })
        {
            return;
        }

        if (schema.Example is null)
        {
            schema.Example = schema.Examples[0];
        }

        var extensionExamples = ReferenceEquals(schema.Example, schema.Examples[0])
            ? schema.Examples.Skip(1)
            : schema.Examples;
        var jsonArray = new JsonArray(extensionExamples.Select(static example => example.DeepClone()).ToArray());
        if (jsonArray.Count > 0)
        {
            schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
            schema.Extensions[OpenApiConstants.JsonSchemaExamplesExtension] = new JsonNodeExtension(jsonArray);
        }
#pragma warning restore CS0618
    }

    internal static void InferFormatFromComposedSchemas(OpenApiSchema schema)
    {
        if (!string.IsNullOrEmpty(schema.Format))
        {
            return;
        }

        schema.Format = schema.AllOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
            schema.AnyOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
            schema.OneOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format;
    }
}
