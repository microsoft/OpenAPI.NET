// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiXml> _xmlFixedFields = new FixedFieldMap<OpenApiXml>
        {
            {
                "name", (o, n, _, _) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "namespace",
                (o, n, _, _) =>
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
                (o, n, _, _) => o.Prefix = n.GetScalarValue()
            },
            {
                "attribute",
                (o, n, _, _) =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    o.Attribute = n.GetScalarBoolValue();
#pragma warning restore CS0618 // Type or member is obsolete
                }
            },
            {
                "wrapped",
                (o, n, _, _) =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    o.Wrapped = n.GetScalarBoolValue();
#pragma warning restore CS0618 // Type or member is obsolete
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
            var jsonObject = node.CheckMapNode("xml", context);

            var xml = new OpenApiXml();
            ParseMap(jsonObject, xml, _xmlFixedFields, _xmlPatternFields, hostDocument, context);

            return xml;
        }
    }
}
