// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
                "name", (o, n, _) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "namespace",
                (o, n, _) =>
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
                (o, n, _) => o.Prefix = n.GetScalarValue()
            },
            {
                "attribute",
                (o, n, _) =>
                {
                    var attribute = n.GetScalarValue();
                    if (attribute is not null)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        o.Attribute = bool.Parse(attribute);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            },
            {
                "wrapped",
                (o, n, _) =>
                {
                    var wrapped = n.GetScalarValue();
                    if (wrapped is not null)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        o.Wrapped = bool.Parse(wrapped);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields =
            new PatternFieldMap<OpenApiXml>
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiXml LoadXml(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("xml");

            var xml = new OpenApiXml();
            foreach (var property in mapNode)
            {
                property.ParseField(xml, _xmlFixedFields, _xmlPatternFields, hostDocument);
            }

            return xml;
        }
    }
}
