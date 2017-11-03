// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.OpenApiV2Deserializer
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        #region OAuthFlowsObject

        private static FixedFieldMap<OpenApiOAuthFlows> OAuthFlowsFixedFileds = new FixedFieldMap<OpenApiOAuthFlows>
        {
            { "implicit", (o,n) => o.Implicit = LoadOAuthFlow(n) },
            { "password", (o,n) => o.Password = LoadOAuthFlow(n) },
            { "clientCredentials", (o,n) => o.ClientCredentials = LoadOAuthFlow(n) },
            { "authorizationCode", (o,n) => o.AuthorizationCode = LoadOAuthFlow(n) }
        };

        private static PatternFieldMap<OpenApiOAuthFlows> OAuthFlowsPatternFields = new PatternFieldMap<OpenApiOAuthFlows>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OAuthFlows");

            var oAuthFlows = new OpenApiOAuthFlows();
            foreach (var property in mapNode)
            {
                property.ParseField(oAuthFlows, OAuthFlowsFixedFileds, OAuthFlowsPatternFields);
            }

            return oAuthFlows;
        }
        #endregion
    }
}
