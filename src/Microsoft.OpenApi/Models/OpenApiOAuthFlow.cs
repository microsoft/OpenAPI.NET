// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// OAuth Flow Object.
    /// </summary>
    public class OpenApiOAuthFlow : IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The authorization URL to be used for this flow.
        /// Applies to implicit and authorizationCode OAuthFlow.
        /// </summary>
        public Uri AuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. The token URL to be used for this flow.
        /// Applies to password, clientCredentials, and authorizationCode OAuthFlow.
        /// </summary>
        public Uri TokenUrl { get; set; }

        /// <summary>
        /// The URL to be used for obtaining refresh tokens.
        /// </summary>
        public Uri RefreshUrl { get; set; }

        /// <summary>
        /// REQUIRED. A map between the scope name and a short description for it.
        /// </summary>
        public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
    }
}
