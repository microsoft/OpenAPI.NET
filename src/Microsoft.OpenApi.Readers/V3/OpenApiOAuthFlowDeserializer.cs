// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOAuthFlow> OAuthFlowFixedFileds =
            new FixedFieldMap<OpenApiOAuthFlow>
            {
                {"authorizationUrl", (o, n) => o.AuthorizationUrl = new Uri(n.GetScalarValue())},
                {"tokenUrl", (o, n) => o.TokenUrl = new Uri(n.GetScalarValue())},
                {"refreshUrl", (o, n) => o.RefreshUrl = new Uri(n.GetScalarValue())},
                {"scopes", (o, n) => o.Scopes = n.CreateSimpleMap(LoadString)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlow> OAuthFlowPatternFields =
            new PatternFieldMap<OpenApiOAuthFlow>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
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
    }
}