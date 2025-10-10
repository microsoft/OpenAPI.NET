// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// OAuth Flow Object.
    /// </summary>
    public class OpenApiOAuthFlow : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The authorization URL to be used for this flow.
        /// Applies to implicit and authorizationCode OAuthFlow.
        /// </summary>
        public Uri? AuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. The token URL to be used for this flow.
        /// Applies to password, clientCredentials, and authorizationCode OAuthFlow.
        /// </summary>
        public Uri? TokenUrl { get; set; }

        /// <summary>
        /// The URL to be used for obtaining refresh tokens.
        /// </summary>
        public Uri? RefreshUrl { get; set; }

        /// <summary>
        /// The URL to be used for device authorization (RFC 8628).
        /// </summary>
        public Uri? DeviceAuthorizationUrl { get; set; }

        /// <summary>
        /// REQUIRED. A map between the scope name and a short description for it.
        /// </summary>
        public IDictionary<string, string>? Scopes { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiOAuthFlow() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiOAuthFlow"/> object
        /// </summary>
        public OpenApiOAuthFlow(OpenApiOAuthFlow oAuthFlow)
        {
            AuthorizationUrl = oAuthFlow?.AuthorizationUrl != null ? new Uri(oAuthFlow.AuthorizationUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            TokenUrl = oAuthFlow?.TokenUrl != null ? new Uri(oAuthFlow.TokenUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            RefreshUrl = oAuthFlow?.RefreshUrl != null ? new Uri(oAuthFlow.RefreshUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            DeviceAuthorizationUrl = oAuthFlow?.DeviceAuthorizationUrl != null ? new Uri(oAuthFlow.DeviceAuthorizationUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            Scopes = oAuthFlow?.Scopes != null ? new Dictionary<string, string>(oAuthFlow.Scopes) : null;
            Extensions = oAuthFlow?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(oAuthFlow.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.2
        /// </summary>
        public void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // authorizationUrl
            writer.WriteProperty(OpenApiConstants.AuthorizationUrl, AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteProperty(OpenApiConstants.TokenUrl, TokenUrl?.ToString());

            // refreshUrl
            writer.WriteProperty(OpenApiConstants.RefreshUrl, RefreshUrl?.ToString());

            // deviceAuthorizationUrl
            // For OpenAPI 3.2, serialize as a regular field
            // For OpenAPI 3.1 and 3.0, serialize as an extension with x-oai- prefix
            if (DeviceAuthorizationUrl != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteProperty(OpenApiConstants.DeviceAuthorizationUrl, DeviceAuthorizationUrl.ToString());
                }
                else
                {
                    writer.WriteProperty("x-oai-deviceAuthorizationUrl", DeviceAuthorizationUrl.ToString());
                }
            }

            // scopes
            writer.WriteRequiredMap(OpenApiConstants.Scopes, Scopes, (w, s) => w.WriteValue(s));

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // OAuthFlow object does not exist in V2.
        }
    }
}
