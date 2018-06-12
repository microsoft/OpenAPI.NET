// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
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
        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new FixedFieldMap<OpenApiSecurityScheme>
            {
                {
                    "type", (o, n) =>
                    {
                        o.Type = n.GetScalarValue().GetEnumFromDisplayName<SecuritySchemeType>();
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "name", (o, n) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
                },
                {
                    "in", (o, n) =>
                    {
                        o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>();
                    }
                },
                {
                    "scheme", (o, n) =>
                    {
                        o.Scheme = n.GetScalarValue();
                    }
                },
                {
                    "bearerFormat", (o, n) =>
                    {
                        o.BearerFormat = n.GetScalarValue();
                    }
                },
                {
                    "openIdConnectUrl", (o, n) =>
                    {
                        o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                    }
                },
                {
                    "flows", (o, n) =>
                    {
                        o.Flows = LoadOAuthFlows(n);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new PatternFieldMap<OpenApiSecurityScheme>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
        {
            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields);
            }

            return securityScheme;
        }
    }
}