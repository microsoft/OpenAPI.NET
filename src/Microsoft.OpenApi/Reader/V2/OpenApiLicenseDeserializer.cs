// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

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
                (o, n, _, c) => o.Name = n.GetScalarValue()
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
            },
        };

        private static readonly PatternFieldMap<OpenApiLicense> _licensePatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiLicense LoadLicense(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("OpenApiLicense", context);

            var license = new OpenApiLicense();

            ParseMap(JsonObject, license, _licenseFixedFields, _licensePatternFields, hostDocument, context);

            return license;
        }
    }
}
