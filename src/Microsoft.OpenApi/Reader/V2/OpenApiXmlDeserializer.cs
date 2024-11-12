// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

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
                (o, n, _) => o.Name = n.GetScalarValue()
            },
            {
                "namespace", (o, n, _) =>
                {
                    if (Uri.IsWellFormedUriString(n.GetScalarValue(), UriKind.Absolute))
                    {
                        o.Namespace = new(n.GetScalarValue(), UriKind.Absolute);
                    }
                    else
                    {
                        throw new OpenApiReaderException($"Xml Namespace requires absolute URL. '{n.GetScalarValue()}' is not valid.");
                    }
                }
            },
            {
                "prefix",
                (o, n, _) => o.Prefix = n.GetScalarValue()
            },
            {
                "attribute",
                (o, n, _) => o.Attribute = bool.Parse(n.GetScalarValue())
            },
            {
                "wrapped",
                (o, n, _) => o.Wrapped = bool.Parse(n.GetScalarValue())
            },
        };

        private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiXml LoadXml(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("xml");

            var xml = new OpenApiXml();
            foreach (var property in mapNode)
            {
                property.ParseField(xml, _xmlFixedFields, _xmlPatternFields);
            }

            return xml;
        }
    }
}
