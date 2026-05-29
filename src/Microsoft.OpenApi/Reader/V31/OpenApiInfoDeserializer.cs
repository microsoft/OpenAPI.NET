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
        public static readonly FixedFieldMap<OpenApiInfo> InfoFixedFields = new()
        {
            {
                "title", (o, n, _, c) =>
                {
                    o.Title = n.GetScalarValue();
                }
            },
            {
                "version", (o, n, _, c) =>
                {
                    o.Version = n.GetScalarValue();
                }
            },
            {
                "summary", (o, n, _, c) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _, c) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "termsOfService",
                (o, n, _, c) =>
                {
                    var terms = n.GetScalarValue();
                    if (terms != null)
                    {
                        o.TermsOfService = new(terms, UriKind.RelativeOrAbsolute);
                    }
                }
            },
            {
                "contact", (o, n, t, c) =>
                {
                    o.Contact = LoadContact(n, t, c);
                }
            },
            {
                "license", (o, n, t, c) =>
                {
                    o.License = LoadLicense(n, t, c);
                }
            }
        };

        public static readonly PatternFieldMap<OpenApiInfo> InfoPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n, _, c) => o.AddExtension(k,LoadExtension(k, n, c))}
        };

        public static OpenApiInfo LoadInfo(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("Info", context);
            var info = new OpenApiInfo();
            ParseMap(jsonObject, info, InfoFixedFields, InfoPatternFields, hostDocument, context);

            return info;
        }
    }
}
