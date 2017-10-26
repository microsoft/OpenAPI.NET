// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiRequestBody"/> serialization.
    /// </summary>
    internal static class OpenApiRequestBodyExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiRequestBody requestBody, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (requestBody == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (requestBody.IsReference())
            {
                requestBody.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("description", requestBody.Description);
                writer.WriteBoolProperty("required", requestBody.Required, false);
                writer.WriteMap("content", requestBody.Content, (w, c) => c.SerializeV3(w));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiRequestBody requestBody, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
