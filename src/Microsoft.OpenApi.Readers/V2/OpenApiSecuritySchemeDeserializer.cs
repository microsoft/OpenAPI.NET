// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static string _flowValue;

        private static OpenApiOAuthFlow _flow;

        private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields =
            new FixedFieldMap<OpenApiSecurityScheme>
            {
                {
                    "type",
                    (o, n) =>
                    {
                        var type = n.GetScalarValue();
                        switch (type)
                        {
                            case "basic":
                                o.Type = SecuritySchemeType.Http;
                                o.Scheme = "basic";
                                break;

                            case "apiKey":
                                o.Type = SecuritySchemeType.ApiKey;
                                break;

                            case "oauth2":
                                o.Type = SecuritySchemeType.OAuth2;
                                break;
                        }
                    }
                },
                {"description", (o, n) => o.Description = n.GetScalarValue()},
                {"name", (o, n) => o.Name = n.GetScalarValue()},
                {"in", (o, n) => o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>()},
                {
                    "flow", (o, n) =>
                    {
                        _flowValue = n.GetScalarValue();
                    }
                },
                {
                    "authorizationUrl",
                    (o, n) =>
                    {
                        _flow.AuthorizationUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                    }
                },
                {
                    "tokenUrl",
                    (o, n) =>
                    {
                        _flow.TokenUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                    }
                },
                {
                    "scopes", (o, n) =>
                    {
                        _flow.Scopes = n.CreateSimpleMap(LoadString);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new PatternFieldMap<OpenApiSecurityScheme>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
        {
            // Reset the local variables every time this method is called.
            // TODO: Change _flow to a tempStorage variable to make the deserializer thread-safe.
            _flowValue = null;
            _flow = new OpenApiOAuthFlow();

            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields);
            }

            // Put the Flow object in the right Flows property based on the string in "flow"
            if (_flowValue == OpenApiConstants.Implicit)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    Implicit = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.Password)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    Password = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.Application)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = _flow
                };
            }
            else if (_flowValue == OpenApiConstants.AccessCode)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = _flow
                };
            }

            return securityScheme;
        }
    }
}