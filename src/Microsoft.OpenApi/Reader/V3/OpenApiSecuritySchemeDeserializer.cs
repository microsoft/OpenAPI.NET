// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

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
                    (o, n, _, c) => 
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<SecuritySchemeType>(c, out var type))
                        {
                            return;
                        }
                        o.Type = type;
                    }
                },
                {
                    "description",
                    (o, n, _, _) => o.Description = n.GetScalarValue()
                },
                {
                    "name",
                    (o, n, _, _) => o.Name = n.GetScalarValue()
                },
                {
                    "in",
                    (o, n, _, c) => 
                    {
                        if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(c, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
                {
                    "scheme",
                    (o, n, _, _) => o.Scheme = n.GetScalarValue()
                },
                {
                    "bearerFormat",
                    (o, n, _, _) => o.BearerFormat = n.GetScalarValue()
                },
                {
                    "openIdConnectUrl",
                    (o, n, _, _) =>
                    {
                        var connectUrl = n.GetScalarValue();
                        if (connectUrl != null)
                        {
                            o.OpenIdConnectUrl = new(connectUrl, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "flows",
                    (o, n, t, c) => o.Flows = LoadOAuthFlows(n, t, c)
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => 
                {
                    if (p.Equals(OpenApiConstants.OAuth2MetadataUrlExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        var metadataUrl = n.GetScalarValue();
                        if (metadataUrl != null)
                        {
                            o.OAuth2MetadataUrl = new(metadataUrl, UriKind.RelativeOrAbsolute);
                        }
                    }
                    else if (p.Equals("x-oai-deprecated", StringComparison.OrdinalIgnoreCase))
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                    else
                    {
                        o.AddExtension(p, LoadExtension(p, n, c));
                    }
                }}
            };

        public static IOpenApiSecurityScheme LoadSecurityScheme(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("securityScheme", context);
            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiSecuritySchemeReference(reference.Item1, hostDocument, reference.Item2);
            }

            var securityScheme = new OpenApiSecurityScheme();
            ParseMap(jsonObject, securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument, context);

            return securityScheme;
        }
    }
}
