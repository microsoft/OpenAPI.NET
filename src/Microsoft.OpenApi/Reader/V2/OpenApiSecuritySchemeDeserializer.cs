// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
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
            new()
            {
                {
                    "type",
                    (o, n, _) =>
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
                        }
                    }
                },
                {"description", (o, n, _) => o.Description = n.GetScalarValue()},
                {"name", (o, n, _) => o.Name = n.GetScalarValue()},
                {"in", (o, n, _) => o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>()},
                {
                    "flow", (_, n, _) => _flowValue = n.GetScalarValue()
                },
                {
                    "authorizationUrl",
                    (_, n, _) => _flow.AuthorizationUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {
                    "tokenUrl",
                    (_, n, _) => _flow.TokenUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {
                    "scopes", (_, n, _) => _flow.Scopes = n.CreateSimpleMap(LoadString)
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node, OpenApiDocument hostDocument = null)
        {
            // Reset the local variables every time this method is called.
            // TODO: Change _flow to a tempStorage variable to make the deserializer thread-safe.
            _flowValue = null;
            _flow = new();

            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, _securitySchemeFixedFields, _securitySchemePatternFields);
            }

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
