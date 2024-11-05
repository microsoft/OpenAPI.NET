// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new()
            {
                {
                    "type", (o, n, _) =>
                    {
                        o.Type = n.GetScalarValue().GetEnumFromDisplayName<SecuritySchemeType>();
                    }
                },
                {
                    "description", (o, n, _) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "name", (o, n, _) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
                },
                {
                    "in", (o, n, _) =>
                    {
                        o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>();
                    }
                },
                {
                    "scheme", (o, n, _) =>
                    {
                        o.Scheme = n.GetScalarValue();
                    }
                },
                {
                    "bearerFormat", (o, n, _) =>
                    {
                        o.BearerFormat = n.GetScalarValue();
                    }
                },
                {
                    "openIdConnectUrl", (o, n, _) =>
                    {
                        o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                    }
                },
                {
                    "flows", (o, n, t) =>
                    {
                        o.Flows = LoadOAuthFlows(n, t);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("securityScheme");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiSecuritySchemeReference(reference.Item1, hostDocument, reference.Item2);
            }

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields);
            }

            return securityScheme;
        }
    }
}
