// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node)
        {
            var mapNode = node.CheckMapNode("security");

            var securityRequirement = new OpenApiSecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = LoadSecuritySchemeByReference(
                    mapNode.Context,
                    mapNode.Diagnostic,
                    property.Name);

                var scopes = property.Value.CreateSimpleList(value => value.GetScalarValue());

                if (scheme != null)
                {
                    securityRequirement.Add(scheme, scopes);
                }
                else
                {
                    node.Diagnostic.Errors.Add(
                        new OpenApiError(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
                }
            }

            return securityRequirement;
        }

        private static OpenApiSecurityScheme LoadSecuritySchemeByReference(
            ParsingContext context,
            OpenApiDiagnostic diagnostic,
            string schemeName)
        {
            var securitySchemeObject = new OpenApiSecurityScheme()
            {
                UnresolvedReference = true,
                Reference = new OpenApiReference()
                {
                    Id = schemeName,
                    Type = ReferenceType.SecurityScheme
                }
            };

            return securitySchemeObject;
        }
    }
}