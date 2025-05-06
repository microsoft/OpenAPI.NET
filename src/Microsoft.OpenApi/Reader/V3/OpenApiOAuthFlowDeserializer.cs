// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
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
                    (o, n, _) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.AuthorizationUrl = new(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "tokenUrl",
                    (o, n, _) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.TokenUrl = new(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {
                    "refreshUrl",
                    (o, n, _) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.RefreshUrl = new(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {"scopes", (o, n, _) => o.Scopes = n.CreateSimpleMap(LoadString).Where(kv => kv.Value is not null).ToOrderedDictionary(kv => kv.Key, kv => kv.Value!)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("OAuthFlow");

            var oauthFlow = new OpenApiOAuthFlow();
            foreach (var property in mapNode)
            {
                property.ParseField(oauthFlow, _oAuthFlowFixedFields, _oAuthFlowPatternFields, hostDocument);
            }

            return oauthFlow;
        }
    }
}
