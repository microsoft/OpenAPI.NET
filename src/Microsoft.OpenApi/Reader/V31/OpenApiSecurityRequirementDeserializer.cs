// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        public static OpenApiSecurityRequirement LoadSecurityRequirement(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("security", context);

            var securityRequirement = new OpenApiSecurityRequirement();

            foreach (var property in jsonObject)
            {
                var scheme = LoadSecuritySchemeByReference(property.Key, hostDocument);

                var scopes = property.Value.CreateSimpleList((n2, _) => n2.GetScalarValue(), hostDocument, context)
                                    .OfType<string>()
                                    .ToList();
                if (scheme != null)
                {
                    securityRequirement.Add(scheme, scopes);
                }
                else
                {
                    context.Diagnostic.Errors.Add(
                        new OpenApiError(context.GetLocation(), $"Scheme {property.Key} is not found"));
                }
            }

            return securityRequirement;
        }

        private static OpenApiSecuritySchemeReference LoadSecuritySchemeByReference(string schemeName, OpenApiDocument? hostDocument)
        {
            return new OpenApiSecuritySchemeReference(schemeName, hostDocument);
        }
    }
}
