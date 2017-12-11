// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiXml> XmlFixedFields = new FixedFieldMap<OpenApiXml>
        {
            {
                "name", (o, n) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                "namespace", (o, n) =>
                {
                    o.Namespace = new Uri(n.GetScalarValue());
                }
            },
            {
                "prefix", (o, n) =>
                {
                    o.Prefix = n.GetScalarValue();
                }
            },
            {
                "attribute", (o, n) =>
                {
                    o.Attribute = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "wrapped", (o, n) =>
                {
                    o.Wrapped = bool.Parse(n.GetScalarValue());
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiXml> XmlPatternFields =
            new PatternFieldMap<OpenApiXml>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiXml LoadXml(ParseNode node)
        {
            var mapNode = node.CheckMapNode("xml");
            
            var xml = new OpenApiXml();
            foreach (var property in mapNode)
            {
                property.ParseField(xml, XmlFixedFields, XmlPatternFields);
            }

            return xml;
        }
    }
}