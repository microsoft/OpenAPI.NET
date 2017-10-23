//---------------------------------------------------------------------
// <copyright file="OpenApiSecuritySchema.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Security Scheme Object.
    /// </summary>
    public class OpenApiSecurityScheme : IOpenApiReference, IOpenApiExtension
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string In { get; set; }
        public string Scheme { get; set; }
        public string BearerFormat { get; set; }
        public Uri OpenIdConnectUrl { get; set; }
        public string Flow { get; set; }
        public Uri AuthorizationUrl { get; set; }
        public Uri TokenUrl { get; set; }
        public IDictionary<string,string> Scopes { get; set; }

        /// <summary>
        /// REQUIRED. An object containing configuration information for the flow types supported.
        /// </summary>
        public OpenApiOAuthFlows Flows { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }
    }
}
