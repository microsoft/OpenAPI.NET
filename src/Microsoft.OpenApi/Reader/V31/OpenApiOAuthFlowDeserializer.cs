using System;
using System.Text.Json.Nodes;
using System.Linq;

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
                    (o, n, _, c) =>
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
                    (o, n, _, c) =>
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
                    (o, n, _, c) =>
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
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiOAuthFlow LoadOAuthFlow(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("OAuthFlow", context);

            var oauthFlow = new OpenApiOAuthFlow();
            ParseMap(jsonObject, oauthFlow, _oAuthFlowFixedFileds, _oAuthFlowPatternFields, hostDocument, context);

            return oauthFlow;
        }
    }
}
