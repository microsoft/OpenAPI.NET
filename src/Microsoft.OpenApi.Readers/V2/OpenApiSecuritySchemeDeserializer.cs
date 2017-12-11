// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
        private static string flowValue;

        private static OpenApiOAuthFlow flow;

        private static readonly FixedFieldMap<OpenApiSecurityScheme> SecuritySchemeFixedFields =
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
                        flowValue = n.GetScalarValue();
                    }
                },
                {
                    "authorizationUrl",
                    (o, n) =>
                    {
                        flow.AuthorizationUrl = new Uri(n.GetScalarValue());
                    }
                },
                {
                    "tokenUrl",
                    (o, n) =>
                    {
                        flow.TokenUrl = new Uri(n.GetScalarValue());
                    }
                },
                {
                    "scopes", (o, n) =>
                    {
                        flow.Scopes = n.CreateSimpleMap(LoadString);
                    }
                }
            };

        private static readonly PatternFieldMap<OpenApiSecurityScheme> SecuritySchemePatternFields =
            new PatternFieldMap<OpenApiSecurityScheme>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
        {
            // Reset the local variables every time this method is called.
            flowValue = null;
            flow = new OpenApiOAuthFlow();

            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, SecuritySchemeFixedFields, SecuritySchemePatternFields);
            }

            // Put the Flow object in the right Flows property based on the string in "flow"
            if (flowValue == OpenApiConstants.Implicit)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    Implicit = flow
                };
            }
            else if (flowValue == OpenApiConstants.Password)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    Password = flow
                };
            }
            else if (flowValue == OpenApiConstants.Application)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = flow
                };
            }
            else if (flowValue == OpenApiConstants.AccessCode)
            {
                securityScheme.Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = flow
                };
            }

            return securityScheme;
        }
    }
}