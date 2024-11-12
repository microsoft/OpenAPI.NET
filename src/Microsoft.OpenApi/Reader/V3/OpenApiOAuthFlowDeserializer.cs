// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOAuthFlow> _oAuthFlowFixedFields =
            new()
            {
                {
                    "authorizationUrl",
                    (o, n, _) => o.AuthorizationUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {
                    "tokenUrl",
                    (o, n, _) => o.TokenUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {
                    "refreshUrl",
                    (o, n, _) => o.RefreshUrl = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
                {"scopes", (o, n, _) => o.Scopes = n.CreateSimpleMap(LoadString)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("OAuthFlow");

            var oauthFlow = new OpenApiOAuthFlow();
            foreach (var property in mapNode)
            {
                property.ParseField(oauthFlow, _oAuthFlowFixedFields, _oAuthFlowPatternFields);
            }

            return oauthFlow;
        }
    }
}
