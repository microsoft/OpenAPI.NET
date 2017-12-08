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

        private static readonly OpenApiOAuthFlow flow = new OpenApiOAuthFlow();

        private static readonly FixedFieldMap<OpenApiSecurityScheme> SecuritySchemeFixedFields =
            new FixedFieldMap<OpenApiSecurityScheme>
            {
                {
                    "type",
                    (o, n) => o.Type = (SecuritySchemeType)Enum.Parse(typeof(SecuritySchemeType), n.GetScalarValue())
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
                        flow.Scopes = n.CreateMap(LoadString);
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
            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, SecuritySchemeFixedFields, SecuritySchemePatternFields);
            }

            securityScheme.Flows = new OpenApiOAuthFlows();
            ;

            // Put the Flow object in the right Flows property based on the string in "flow"
            if (flowValue == OpenApiConstants.Implicit)
            {
                securityScheme.Flows.Implicit = flow;
            }
            else if (flowValue == OpenApiConstants.Password)
            {
                securityScheme.Flows.Password = flow;
            }
            else if (flowValue == OpenApiConstants.Application)
            {
                securityScheme.Flows.ClientCredentials = flow;
            }
            else if (flowValue == OpenApiConstants.AccessCode)
            {
                securityScheme.Flows.AuthorizationCode = flow;
            }

            return securityScheme;
        }
    }
}