using System;
using System.Linq;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOAuthFlow> _oAuthFlowFixedFileds =
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
                {"scopes", (o, n, _) => o.Scopes = n.CreateSimpleMap(LoadString).Where(kv => kv.Value is not null).ToDictionary(kv => kv.Key, kv => kv.Value!)}
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
                property.ParseField(oauthFlow, _oAuthFlowFixedFileds, _oAuthFlowPatternFields, hostDocument);
            }

            return oauthFlow;
        }
    }
}
