using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static FixedFieldMap<OpenApiLicense> _licenseFixedFields = new FixedFieldMap<OpenApiLicense>
        {
            {
                "name", (o, n) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "identifier", (o, n) =>
                {
                    o.Identifier = n.GetScalarValue();
                }
            },
            {
                "url", (o, n) =>
                {
                    o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                }
            },
        };

        private static PatternFieldMap<OpenApiLicense> _licensePatternFields = new PatternFieldMap<OpenApiLicense>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };
        
        internal static OpenApiLicense LoadLicense(ParseNode node)
        {
            var mapNode = node.CheckMapNode("License");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, _licenseFixedFields, _licensePatternFields);

            return license;
        }
    }
}
