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
        private static readonly FixedFieldMap<OpenApiLicense> _licenseFixedFields = new()
        {
            {
                "name", (o, n, _, c) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "identifier", (o, n, _, c) =>
                {
                    o.Identifier = n.GetScalarValue();
                }
            },
            {
                "url",
                (o, n, _, c) =>
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
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        internal static OpenApiLicense LoadLicense(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("License", context);

            var license = new OpenApiLicense();

            ParseMap(jsonObject, license, _licenseFixedFields, _licensePatternFields, hostDocument, context);

            return license;
        }
    }
}

