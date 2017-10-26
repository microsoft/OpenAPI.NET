// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiOAuthFlows"/> serialization.
    /// </summary>
    internal static class OpenApiOAuthFlowsExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiOAuthFlows oAuthFlows, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (oAuthFlows != null)
            {
                writer.WriteObject("implicit", oAuthFlows.Implicit, (w, o) => o.SerializeV3(w));
                writer.WriteObject("password", oAuthFlows.Password, (w, o) => o.SerializeV3(w));
                writer.WriteObject("clientCredentials", oAuthFlows.ClientCredentials, (w, o) => o.SerializeV3(w));
                writer.WriteObject("authorizationCode", oAuthFlows.AuthorizationCode, (w, o) => o.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiOAuthFlows oAuthFlows, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
