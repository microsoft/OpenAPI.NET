// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
    public class OpenApiOAuthFlow : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// REQUIRED. The authorization URL to be used for this flow.
        /// </summary>
        public Uri AuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. The token URL to be used for this flow.
        /// </summary>
        public Uri TokenUrl { get; set; }

        /// <summary>
        /// The URL to be used for obtaining refresh tokens.
        /// </summary>
        public Uri RefreshUrl { get; set; }

        /// <summary>
        /// REQUIRED. A map between the scope name and a short description for it.
        /// </summary>
        public IDictionary<string, string> Scopes { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("authorizationUrl", AuthorizationUrl?.ToString());
            writer.WriteStringProperty("tokenUrl", TokenUrl?.ToString());
            writer.WriteStringProperty("refreshUrl", RefreshUrl?.ToString());
            writer.WriteMap("scopes", Scopes, (w, s) => w.WriteValue(s));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // authorizationUrl
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocAuthorizationUrl, AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTokenUrl, TokenUrl?.ToString());

            // scopes
            writer.WriteMap(OpenApiConstants.OpenApiDocScopes, Scopes, (w, s) => w.WriteValue(s));
        }
    }
}
