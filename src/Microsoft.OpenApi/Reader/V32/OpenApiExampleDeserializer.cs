using System;

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
                "summary", (o, n, _) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "value", (o, n, _) =>
                {
                    o.Value = n.CreateAny();
                }
            },
            {
                "externalValue", (o, n, _) =>
                {
                    o.ExternalValue = n.GetScalarValue();
                }
            },
            {
                "dataValue", (o, n, _) =>
                {
                    o.DataValue = n.CreateAny();
                }
            },
            {
                "serializedValue", (o, n, _) =>
                {
                    o.SerializedValue = n.GetScalarValue();
                }
            },

        };

        private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static IOpenApiExample LoadExample(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("example");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var exampleReference = new OpenApiExampleReference(reference.Item1, hostDocument, reference.Item2);
                exampleReference.Reference.SetMetadataFromMapNode(mapNode);
                return exampleReference;
            }

            var example = new OpenApiExample();
            foreach (var property in mapNode)
            {
                property.ParseField(example, _exampleFixedFields, _examplePatternFields, hostDocument);
            }

            return example;
        }
    }
}

