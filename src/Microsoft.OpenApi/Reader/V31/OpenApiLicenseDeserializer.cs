using System;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiLicense> _licenseFixedFields = new()
        {
            {
                "name", (o, n, _) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "identifier", (o, n, _) =>
                {
                    o.Identifier = n.GetScalarValue();
                }
            },
            {
                "url",
                (o, n, _) =>
                {
                    var url = n.GetScalarValue();
                    if (url != null)
                    {
                        o.Url = new(url, UriKind.RelativeOrAbsolute);
                    }
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiLicense> _licensePatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        internal static OpenApiLicense LoadLicense(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("License");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, _licenseFixedFields, _licensePatternFields, hostDocument);

            return license;
        }
    }
}
