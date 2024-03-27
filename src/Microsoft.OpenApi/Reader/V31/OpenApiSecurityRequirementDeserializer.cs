// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("security");

            var securityRequirement = new OpenApiSecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = LoadSecuritySchemeByReference(property.Name, hostDocument);

                var scopes = property.Value.CreateSimpleList((value, p) => value.GetScalarValue());

                if (scheme != null)
                {
                    securityRequirement.Add(scheme, scopes);
                }
                else
                {
                    mapNode.Context.Diagnostic.Errors.Add(
                        new OpenApiError(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
                }
            }

            return securityRequirement;
        }

        private static OpenApiSecurityScheme LoadSecuritySchemeByReference(string schemeName, OpenApiDocument hostDocument)
        {
            var securitySchemeObject = new OpenApiSecuritySchemeReference(schemeName, hostDocument);
            return securitySchemeObject;
        }
    }
}
