// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static string? _flowValue;

        private static OpenApiOAuthFlow? _flow;

        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new()
            {
                {
                    "type",
                    (o, n, _, c) =>
                    {
                        var type = n.GetScalarValue();
                        switch (type)
                        {
                            case "basic":
                                o.Type = SecuritySchemeType.Http;
                                o.Scheme = OpenApiConstants.Basic;
                                break;

                            case "apiKey":
                                o.Type = SecuritySchemeType.ApiKey;
                                break;

                            case "oauth2":
                                o.Type = SecuritySchemeType.OAuth2;
                                break;

                            default:
                                c.Diagnostic.Errors.Add(new OpenApiError(c.GetLocation(), $"Security scheme type {type} is not recognized."));
                                break;
                        }
                    }
                },
                {"description", (o, n, _, _) => o.Description = n.GetScalarValue()},
                {"name", (o, n, _, _) => o.Name = n.GetScalarValue()},
                {"in", (o, n, _, c) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(c, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
                {
                    "flow", (_, n, _, _) => _flowValue = n.GetScalarValue()
                },
                {
                    "authorizationUrl",
                    (_, n, _, _) =>
                    {
                        var scalarValue = n.GetScalarValue();
                        if (_flow is not null && scalarValue is not null)
                        {
                            _flow.AuthorizationUrl = new(scalarValue, UriKind.RelativeOrAbsolute);
                        }                        
                    }
                },
                {
                    "tokenUrl",
                    (_, n, _, _) =>
                    {
                        var scalarValue = n.GetScalarValue();
                        if (_flow is not null && scalarValue is not null)
                        {
                             _flow.TokenUrl = new(scalarValue, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "scopes", (_, n, _, c) =>
                    {
                        if (_flow is not null)
                        {
                            _flow.Scopes = n.CreateSimpleMap(LoadString, c)
                                .Where(kv => kv.Value != null)
                                .ToDictionary(kv => kv.Key, kv => kv.Value!);
                        } 
                    }
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
                    else
                    {
                        o.AddExtension(p, LoadExtension(p, n, c));
                    }
                }}
            };

        public static IOpenApiSecurityScheme LoadSecurityScheme(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            // Reset the local variables every time this method is called.
            // TODO: Change _flow to a tempStorage variable to make the deserializer thread-safe.
            _flowValue = null;
            _flow = new();

            var jsonObject = node.CheckMapNode("securityScheme", context);

            var securityScheme = new OpenApiSecurityScheme();
            ParseMap(jsonObject, securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument, context);

            // Put the Flow object in the right Flows property based on the string in "flow"
            if (_flowValue == OpenApiConstants.Implicit)
            {
                securityScheme.Flows = new()
                {
                    Implicit = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.Password)
            {
                securityScheme.Flows = new()
                {
                    Password = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.Application)
            {
                securityScheme.Flows = new()
                {
                    ClientCredentials = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.AccessCode)
            {
                securityScheme.Flows = new()
                {
                    AuthorizationCode = _flow
                };
            }

            return securityScheme;
        }
    }
}
