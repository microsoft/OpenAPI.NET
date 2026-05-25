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
        private static readonly FixedFieldMap<OpenApiInfo> _infoFixedFields = new()
        {
            {
                "title",
                (o, n, _, c) => o.Title = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _, c) => o.Description = n.GetScalarValue()
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
                "contact",
                (o, n, t, c) => o.Contact = LoadContact(n, t, c)
            },
            {
                "license",
                (o, n, t, c) => o.License = LoadLicense(n, t, c)
            },
            {
                "version",
                (o, n, _, c) => o.Version = n.GetScalarValue()
            }
        };

        private static readonly PatternFieldMap<OpenApiInfo> _infoPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiInfo LoadInfo(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("Info", context);

            var info = new OpenApiInfo();

            ParseMap(JsonObject, info, _infoFixedFields, _infoPatternFields, hostDocument, context);

            return info;
        }
    }
}
