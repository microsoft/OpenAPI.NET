// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new()
            {
                {
                    "type",
                    (o, n, _) => 
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<SecuritySchemeType>(n.Context, out var type))
                        {
                            return;
                        }
                        o.Type = type;
                    }
                },
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "name",
                    (o, n, _) => o.Name = n.GetScalarValue()
                },
                {
                    "in",
                    (o, n, _) => 
                    {
                        if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(n.Context, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
                {
                    "scheme",
                    (o, n, _) => o.Scheme = n.GetScalarValue()
                },
                {
                    "bearerFormat",
                    (o, n, _) => o.BearerFormat = n.GetScalarValue()
                },
                {
                    "openIdConnectUrl",
                    (o, n, _) => o.OpenIdConnectUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {
                    "flows",
                    (o, n, t) => o.Flows = LoadOAuthFlows(n, t)
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node, OpenApiDocument hostDocument)
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
                property.ParseField(securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument);
            }

            return securityScheme;
        }
    }
}
