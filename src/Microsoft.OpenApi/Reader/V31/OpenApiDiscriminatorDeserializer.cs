using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiDiscriminator> _discriminatorFixedFields =
            new()
            {
                {
                    "propertyName", (o, n, _, c) =>
                    {
                        o.PropertyName = n.GetScalarValue();
                    }
                },
                {
                    "mapping", (o, n, doc, c) =>
                    {
                        o.Mapping = n.CreateSimpleMap(node => LoadMapping(node, doc, c), c);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiDiscriminator LoadDiscriminator(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("discriminator", context);

            var discriminator = new OpenApiDiscriminator();
            ParseMap(jsonObject, discriminator, _discriminatorFixedFields, _discriminatorPatternFields, hostDocument, context);

            return discriminator;
        }

        public static OpenApiSchemaReference LoadMapping(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var pointer = node.GetScalarValue() ?? throw new InvalidOperationException("Could not get a pointer reference");
            var reference = GetReferenceIdAndExternalResource(pointer);
            return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
        }
    }
}
