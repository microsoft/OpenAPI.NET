using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExample> _exampleFixedFields = new()
        {
            {
                "summary", (o, n, _, _) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "value", (o, n, _, _) =>
                {
                    o.Value = n;
                }
            },
            {
                "externalValue", (o, n, _, _) =>
                {
                    o.ExternalValue = n.GetScalarValue();
                }
            },
            {
                "dataValue", (o, n, _, _) =>
                {
                    o.DataValue = n;
                }
            },
            {
                "serializedValue", (o, n, _, _) =>
                {
                    o.SerializedValue = n.GetScalarValue();
                }
            },

        };

        private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static IOpenApiExample LoadExample(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("example", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var exampleReference = new OpenApiExampleReference(reference.Item1, hostDocument, reference.Item2);
                exampleReference.Reference.SetMetadataFromJsonObject(jsonObject);
                return exampleReference;
            }

            var example = new OpenApiExample();
            ParseMap(jsonObject, example, _exampleFixedFields, _examplePatternFields, hostDocument, context);

            return example;
        }
    }
}
