// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiXml> _xmlFixedFields = new()
        {
            {
                "name",
                (o, n, _, c) => o.Name = n.GetScalarValue()
            },
            {
                "namespace", (o, n, _, c) =>
                {
                    var scalarValue = n.GetScalarValue();
                    if (Uri.IsWellFormedUriString(scalarValue, UriKind.Absolute) && scalarValue is not null)
                    {
                        o.Namespace = new(scalarValue, UriKind.Absolute);
                    }
                    else
                    {
                        throw new OpenApiReaderException($"Xml Namespace requires absolute URL. '{n.GetScalarValue()}' is not valid.");
                    }
                }
            },
            {
                "prefix",
                (o, n, _, c) => o.Prefix = n.GetScalarValue()
            },
            {
                "attribute",
                (o, n, _, c) =>
                {
                    var attribute = n.GetScalarValue();
                    if (attribute is not null)
                    {
                        o.Attribute = bool.Parse(attribute);
                    }
                }
            },
            {
                "wrapped",
                (o, n, _, c) =>
                {
                    var wrapped = n.GetScalarValue();
                    if (wrapped is not null)
                    {
                        o.Wrapped = bool.Parse(wrapped);
                    }
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiXml LoadXml(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("xml", context);

            var xml = new OpenApiXml();
            ParseMap(jsonObject, xml, _xmlFixedFields, _xmlPatternFields, hostDocument, context);

            return xml;
        }
    }
}
