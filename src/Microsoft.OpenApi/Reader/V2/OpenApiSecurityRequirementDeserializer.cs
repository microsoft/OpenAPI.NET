// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System.Linq;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        public static OpenApiSecurityRequirement LoadSecurityRequirement(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("security", context);

            var securityRequirement = new OpenApiSecurityRequirement();

            foreach (var property in JsonObject)
            {
                var scheme = LoadSecuritySchemeByReference(
                    hostDocument,
                    property.Key);

                var scopes = property.Value.CreateSimpleList((n2, p) => n2.GetScalarValue(), hostDocument, context)
                    .OfType<string>()
                    .ToList();

                if (scheme != null)
                {
                    securityRequirement.Add(scheme, scopes);
                }
                else
                {
                    context.Diagnostic.Errors.Add(
                        new(context.GetLocation(),
                        $"Scheme {property.Key} is not found"));
                }
            }

            return securityRequirement;
        }

        private static OpenApiSecuritySchemeReference LoadSecuritySchemeByReference(
            OpenApiDocument? openApiDocument,
            string schemeName)
        {
            return new OpenApiSecuritySchemeReference(schemeName, openApiDocument);
        }
    }
}
