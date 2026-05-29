using System;
using System.Text.Json.Nodes;

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
                {"implicit", (o, n, t, c) => o.Implicit = LoadOAuthFlow(n, t, c)},
                {"password", (o, n, t, c) => o.Password = LoadOAuthFlow(n, t, c)},
                {"clientCredentials", (o, n, t, c) => o.ClientCredentials = LoadOAuthFlow(n, t, c)},
                {"authorizationCode", (o, n, t, c) => o.AuthorizationCode = LoadOAuthFlow(n, t, c)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlows> _oAuthFlowsPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static OpenApiOAuthFlows LoadOAuthFlows(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("OAuthFlows", context);

            var oAuthFlows = new OpenApiOAuthFlows();
            ParseMap(JsonObject, oAuthFlows, _oAuthFlowsFixedFields, _oAuthFlowsPatternFields, hostDocument, context);

            return oAuthFlows;
        }
    }
}
