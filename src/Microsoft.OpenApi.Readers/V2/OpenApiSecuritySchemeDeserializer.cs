// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        #region SecuritySchemeObject

        private static FixedFieldMap<OpenApiSecurityScheme> SecuritySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
        {
            { "type", (o,n) => o.Type = (SecuritySchemeType)Enum.Parse(typeof(SecuritySchemeType), n.GetScalarValue())  },
            { "description", (o,n) => o.Description = n.GetScalarValue() },
            { "name", (o,n) => o.Name = n.GetScalarValue() },
            { "in", (o,n) => o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), n.GetScalarValue()) },
            { "scheme", (o,n) => o.Scheme = n.GetScalarValue() },
            { "bearerFormat", (o,n) => o.BearerFormat = n.GetScalarValue() },
            { "openIdConnectUrl", (o,n) => o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute) },
            { "flows", (o,n) => o.Flows = LoadOAuthFlows(n) }
        };

        private static PatternFieldMap<OpenApiSecurityScheme> SecuritySchemePatternFields = new PatternFieldMap<OpenApiSecurityScheme>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
        {
            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, SecuritySchemeFixedFields, SecuritySchemePatternFields);
            }

            return securityScheme;
        }

        #endregion
    }
}
