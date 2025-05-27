using System;

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
                "title", (o, n, _) =>
                {
                    o.Title = n.GetScalarValue();
                }
            },
            {
                "version", (o, n, _) =>
                {
                    o.Version = n.GetScalarValue();
                }
            },
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
                "termsOfService",
                (o, n, _) =>
                {
                    var terms = n.GetScalarValue();
                    if (terms != null)
                    {
                        o.TermsOfService = new(terms, UriKind.RelativeOrAbsolute);
                    }
                }
            },
            {
                "contact", (o, n, t) =>
                {
                    o.Contact = LoadContact(n, t);
                }
            },
            {
                "license", (o, n, t) =>
                {
                    o.License = LoadLicense(n, t);
                }
            }
        };

        public static readonly PatternFieldMap<OpenApiInfo> InfoPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, k, n, _) => o.AddExtension(k,LoadExtension(k, n))}
        };

        public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Info");
            var info = new OpenApiInfo();
            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, hostDocument);

            return info;
        }
    }
}
