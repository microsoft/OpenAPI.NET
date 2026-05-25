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
        private static readonly FixedFieldMap<OpenApiEncoding> _encodingFixedFields = new()
        {
            {
                "contentType", (o, n, _, c) =>
                {
                    o.ContentType = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n, t, c) =>
                {
                    o.Headers = n.CreateMap(LoadHeader, t, c);
                }
            },
            {
                "encoding", (o, n, t, c) =>
                {
                    o.Encoding = n.CreateMap(LoadEncoding, t, c);
                }
            },
            {
                "itemEncoding", (o, n, t, c) =>
                {
                    o.ItemEncoding = LoadEncoding(n, t, c);
                }
            },
            {
                "prefixEncoding", (o, n, t, c) =>
                {
                    o.PrefixEncoding = n.CreateList(LoadEncoding, t, c);
                }
            },
            {
                "style", (o, n, _, c) =>
                {
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(c, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode", (o, n, _, c) =>
                {
                    var explode = n.GetScalarValue();
                    if (explode is not null)
                    {
                        o.Explode = bool.Parse(explode);
                    }                    
                }
            },
            {
                "allowReserved", (o, n, _, c) =>
                {
                    var allowReserved = n.GetScalarValue();
                    if (allowReserved is not null)
                    {
                        o.AllowReserved = bool.Parse(allowReserved);
                    }
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiEncoding> _encodingPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiEncoding LoadEncoding(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("encoding", context);

            var encoding = new OpenApiEncoding();
            ParseMap(JsonObject, encoding, _encodingFixedFields, _encodingPatternFields, hostDocument, context);

            return encoding;
        }
    }
}

