﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiEncoding> _encodingFixedFields = new()
        {
            {
                "contentType",
                (o, n, _) => o.ContentType = n.GetScalarValue()
            },
            {
                "headers",
                (o, n, t) => o.Headers = n.CreateMap(LoadHeader, t)
            },
            {
                "style",
                (o, n, _) => 
                {
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode",
                (o, n, _) =>
                {
                    var explode = n.GetScalarValue();
                    if (explode != null)
                    {
                        o.Explode = bool.Parse(explode);
                    }
                }
            },
            {
                "allowedReserved", 
                (o, n, _) =>
                {
                    var allowReserved = n.GetScalarValue();
                    if (allowReserved != null)
                    {
                         o.AllowReserved = bool.Parse(allowReserved);
                     }
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiEncoding> _encodingPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiEncoding LoadEncoding(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("encoding");

            var encoding = new OpenApiEncoding();
            foreach (var property in mapNode)
            {
                property.ParseField(encoding, _encodingFixedFields, _encodingPatternFields, hostDocument);
            }

            return encoding;
        }
    }
}
