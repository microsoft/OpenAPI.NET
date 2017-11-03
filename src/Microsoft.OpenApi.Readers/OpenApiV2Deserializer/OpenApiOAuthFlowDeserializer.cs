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
        #region OAuthFlowObject

        private static FixedFieldMap<OpenApiOAuthFlow> OAuthFlowFixedFileds = new FixedFieldMap<OpenApiOAuthFlow>
        {
            { "authorizationUrl", (o,n) => o.AuthorizationUrl = new Uri(n.GetScalarValue()) },
            { "tokenUrl", (o,n) => o.TokenUrl = new Uri(n.GetScalarValue()) },
            { "refreshUrl", (o,n) => o.RefreshUrl = new Uri(n.GetScalarValue()) },
            { "scopes", (o,n) => o.Scopes = n.CreateMap(LoadString) }
        };

        private static PatternFieldMap<OpenApiOAuthFlow> OAuthFlowPatternFields = new PatternFieldMap<OpenApiOAuthFlow>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OAuthFlow");

            var oauthFlow = new OpenApiOAuthFlow();
            foreach (var property in mapNode)
            {
                property.ParseField(oauthFlow, OAuthFlowFixedFileds, OAuthFlowPatternFields);
            }

            return oauthFlow;
        }
        #endregion
    }
}
