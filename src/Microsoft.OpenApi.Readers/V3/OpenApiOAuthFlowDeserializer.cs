// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
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
        private static readonly FixedFieldMap<OpenApiOAuthFlow> _oAuthFlowFixedFileds =
            new FixedFieldMap<OpenApiOAuthFlow>
            {
                {
                    "authorizationUrl", (o, n) =>
                    {
                        Uri uri;
                        if (Uri.TryCreate(n.GetScalarValue(), UriKind.RelativeOrAbsolute, out uri))
                        {
                            o.AuthorizationUrl = uri;
                        }
                    }
                },
                {
                    "tokenUrl", (o, n) =>
                    {
                        Uri uri;
                        if (Uri.TryCreate(n.GetScalarValue(), UriKind.RelativeOrAbsolute, out uri))
                        {
                            o.TokenUrl = uri;
                        }
                    }
                },
                {
                    "refreshUrl", (o, n) =>
                    {
                        Uri uri;
                        if (Uri.TryCreate(n.GetScalarValue(), UriKind.RelativeOrAbsolute, out uri))
                        {
                            o.RefreshUrl = uri;
                        }
                    }
                },
                {"scopes", (o, n) => o.Scopes = n.CreateSimpleMap(LoadString)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields =
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
                property.ParseField(oauthFlow, _oAuthFlowFixedFileds, _oAuthFlowPatternFields);
            }

            return oauthFlow;
        }
    }
}