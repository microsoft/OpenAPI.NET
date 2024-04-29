using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

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
                    "propertyName", (o, n, _) =>
                    {
                        o.PropertyName = n.GetScalarValue();
                    }
                },
                {
                    "mapping", (o, n, _) =>
                    {
                        o.Mapping = n.CreateSimpleMap(LoadString);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiDiscriminator LoadDiscriminator(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("discriminator");

            var discriminator = new OpenApiDiscriminator();
            foreach (var property in mapNode)
            {
                property.ParseField(discriminator, _discriminatorFixedFields, _discriminatorPatternFields);
            }

            return discriminator;
        }
    }
}
