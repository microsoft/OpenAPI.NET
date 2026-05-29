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
        private static readonly FixedFieldMap<OpenApiContact> _contactFixedFields = new()
        {
            {
                "name", (o, n, _, c) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "email", (o, n, _, c) =>
                {
                    o.Email = n.GetScalarValue();
                }
            },
            {
                "url",
                (o, n, t, c) =>
                {
                    var url = n.GetScalarValue();
                    if (url != null)
                    {
                        o.Url = new(url, UriKind.RelativeOrAbsolute);
                    }
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiContact> _contactPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiContact LoadContact(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node as JsonObject;
            var contact = new OpenApiContact();

            ParseMap(jsonObject, contact, _contactFixedFields, _contactPatternFields, hostDocument, context);

            return contact;
        }
    }
}

