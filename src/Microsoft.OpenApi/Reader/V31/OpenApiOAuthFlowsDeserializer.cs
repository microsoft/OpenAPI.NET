﻿using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOAuthFlows> _oAuthFlowsFixedFields =
            new()
            {
                {"implicit", (o, n, t) => o.Implicit = LoadOAuthFlow(n, t)},
                {"password", (o, n, t) => o.Password = LoadOAuthFlow(n, t)},
                {"clientCredentials", (o, n, t) => o.ClientCredentials = LoadOAuthFlow(n, t)},
                {"authorizationCode", (o, n, t) => o.AuthorizationCode = LoadOAuthFlow(n, t)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlows> _oAuthFlowsPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("OAuthFlows");

            var oAuthFlows = new OpenApiOAuthFlows();
            foreach (var property in mapNode)
            {
                property.ParseField(oAuthFlows, _oAuthFlowsFixedFields, _oAuthFlowsPatternFields, hostDocument);
            }

            return oAuthFlows;
        }
    }
}
