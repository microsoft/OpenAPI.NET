// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Linq;

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
                    (o, n, _, _) =>
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
                    (o, n, _, _) =>
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
                    (o, n, _, _) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.RefreshUrl = new(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                },
                {"scopes", (o, n, _, c) => o.Scopes = n.CreateSimpleMap(LoadString, c).Where(kv => kv.Value is not null).ToDictionary(kv => kv.Key, kv => kv.Value!)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) =>
                {
                    if (p.Equals("x-oai-deviceAuthorizationUrl", StringComparison.OrdinalIgnoreCase))
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.DeviceAuthorizationUrl = new(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                    else
                    {
                        o.AddExtension(p, LoadExtension(p, n, c));
                    }
                }}
            };

        public static OpenApiOAuthFlow LoadOAuthFlow(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("OAuthFlow", context);

            var oauthFlow = new OpenApiOAuthFlow();
            ParseMap(jsonObject, oauthFlow, _oAuthFlowFixedFields, _oAuthFlowPatternFields, hostDocument, context);

            return oauthFlow;
        }
    }
}
