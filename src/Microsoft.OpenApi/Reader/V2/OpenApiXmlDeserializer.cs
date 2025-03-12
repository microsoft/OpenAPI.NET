﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
                (o, n, _) => o.Prefix = n.GetScalarValue()
            },
            {
                "attribute",
                (o, n, _) =>
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
                (o, n, _) =>
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
