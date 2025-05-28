// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiLicense> _licenseFixedFields = new()
        {
            {
                "name",
                (o, n, _) => o.Name = n.GetScalarValue()
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
            },
        };

        private static readonly PatternFieldMap<OpenApiLicense> _licensePatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiLicense LoadLicense(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("OpenApiLicense");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, _licenseFixedFields, _licensePatternFields, doc: hostDocument);

            return license;
        }
    }
}
