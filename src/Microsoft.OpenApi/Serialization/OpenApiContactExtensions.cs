// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiContact"/> serialization.
    /// </summary>
    internal static class OpenApiContactExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiContact contact, IOpenApiWriter writer)
        {
            contact.SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiContact"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiContact contact, IOpenApiWriter writer)
        {
            contact.SerializeInternal(writer);
        }

        private static void SerializeInternal(this OpenApiContact contact, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (contact != null)
            {
                writer.WriteStringProperty("name", contact.Name);
                writer.WriteStringProperty("url", contact.Url?.OriginalString);
                writer.WriteStringProperty("email", contact.Email);
            }
            writer.WriteEndObject();
        }
    }
}
