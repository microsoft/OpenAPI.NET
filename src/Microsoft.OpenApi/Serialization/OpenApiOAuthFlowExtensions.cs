// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiOAuthFlow"/> serialization.
    /// </summary>
    internal static class OpenApiOAuthFlowExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiOAuthFlow oAuthFlow, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (oAuthFlow != null)
            {
                writer.WriteStringProperty("authorizationUrl", oAuthFlow.AuthorizationUrl?.ToString());
                writer.WriteStringProperty("tokenUrl", oAuthFlow.TokenUrl?.ToString());
                writer.WriteStringProperty("refreshUrl", oAuthFlow.RefreshUrl?.ToString());
                writer.WriteMap("scopes", oAuthFlow.Scopes, (w, s) => w.WriteValue(s));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlow"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiOAuthFlow oAuthFlow, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (oAuthFlow != null)
            {

                writer.WriteStringProperty("authorizationUrl", oAuthFlow.AuthorizationUrl?.ToString());
                writer.WriteStringProperty("tokenUrl", oAuthFlow.TokenUrl?.ToString());
                writer.WriteMap("scopes", oAuthFlow.Scopes, (w, s) => w.WriteValue(s));
            }
            writer.WriteEndObject();
        }
    }
}
