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
        private static readonly FixedFieldMap<OpenApiEncoding> _encodingFixedFields = new()
        {
            {
                "contentType", (o, n, _) =>
                {
                    o.ContentType = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n, t) =>
                {
                    o.Headers = n.CreateMap(LoadHeader, t);
                }
            },
            {
                "style", (o, n, _) =>
                {
                    o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>();
                }
            },
            {
                "explode", (o, n, _) =>
                {
                    o.Explode = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowedReserved", (o, n, _) =>
                {
                    o.AllowReserved = bool.Parse(n.GetScalarValue());
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiEncoding> _encodingPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiEncoding LoadEncoding(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("encoding");

            var encoding = new OpenApiEncoding();
            foreach (var property in mapNode)
            {
                property.ParseField(encoding, _encodingFixedFields, _encodingPatternFields);
            }

            return encoding;
        }
    }
}
