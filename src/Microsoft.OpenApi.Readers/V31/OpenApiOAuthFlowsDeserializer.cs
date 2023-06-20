using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOAuthFlows> _oAuthFlowsFixedFileds =
            new FixedFieldMap<OpenApiOAuthFlows>
            {
                {"implicit", (o, n) => o.Implicit = LoadOAuthFlow(n)},
                {"password", (o, n) => o.Password = LoadOAuthFlow(n)},
                {"clientCredentials", (o, n) => o.ClientCredentials = LoadOAuthFlow(n)},
                {"authorizationCode", (o, n) => o.AuthorizationCode = LoadOAuthFlow(n)}
            };

        private static readonly PatternFieldMap<OpenApiOAuthFlows> _oAuthFlowsPatternFields =
            new PatternFieldMap<OpenApiOAuthFlows>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OAuthFlows");

            var oAuthFlows = new OpenApiOAuthFlows();
            foreach (var property in mapNode)
            {
                property.ParseField(oAuthFlows, _oAuthFlowsFixedFileds, _oAuthFlowsPatternFields);
            }

            return oAuthFlows;
        }
    }
}
