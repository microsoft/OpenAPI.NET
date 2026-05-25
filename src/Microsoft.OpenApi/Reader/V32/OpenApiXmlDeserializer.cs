// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiXml> _xmlFixedFields = new FixedFieldMap<OpenApiXml>
        {
            {
                "name", (o, n, _, c) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "namespace",
                (o, n, _, c) =>
                {
                    var value = n.GetScalarValue();
                    if (value != null)
                    {
                        o.Namespace = new(value, UriKind.Absolute);
                    }
                }
            },
            {
                "prefix",
                (o, n, _, c) => o.Prefix = n.GetScalarValue()
            },
            {
                "nodeType",
                (o, n, _, c) =>
                {
                    if (!n.GetScalarValue().TryGetEnumFromDisplayName<OpenApiXmlNodeType>(c, out var nodeType))
                    {
                        return;
                    }
                    o.NodeType = nodeType;
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields =
            new PatternFieldMap<OpenApiXml>
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiXml LoadXml(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("xml", context);

            var xml = new OpenApiXml();
            ParseMap(JsonObject, xml, _xmlFixedFields, _xmlPatternFields, hostDocument, context);

            return xml;
        }
    }
}

