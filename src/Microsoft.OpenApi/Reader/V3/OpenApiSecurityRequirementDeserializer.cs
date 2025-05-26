// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("security");

            var securityRequirement = new OpenApiSecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = LoadSecuritySchemeByReference(hostDocument, property.Name);

                var scopes = property.Value.CreateSimpleList((n2, p) => n2.GetScalarValue(), hostDocument)
                    .OfType<string>()
                    .ToList();

                if (scheme != null)
                {
                    securityRequirement.Add(scheme, scopes);
                }
                else
                {
                    mapNode.Context.Diagnostic.Errors.Add(
                        new(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
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
