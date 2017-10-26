// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiLicense"/> serialization.
    /// </summary>
    internal static class OpenApiLicenseExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiLicense license, IOpenApiWriter writer)
        {
            license.SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiLicense license, IOpenApiWriter writer)
        {
            license.SerializeInternal(writer);
        }

        private static void SerializeInternal(this OpenApiLicense license, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (license != null)
            {
                writer.WriteStringProperty("name", license.Name);
                writer.WriteStringProperty("url", license.Url?.OriginalString);
            }
            writer.WriteEndObject();
        }
    }
}
