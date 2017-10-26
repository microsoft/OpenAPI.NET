// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiServer"/> serialization.
    /// </summary>
    internal static class OpenApiServerExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiServer server, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (server != null)
            {
                writer.WriteStringProperty("url", server.Url);
                writer.WriteStringProperty("description", server.Description);
                writer.WriteMap("variables", server.Variables, (w, v) => v.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiServer server, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
