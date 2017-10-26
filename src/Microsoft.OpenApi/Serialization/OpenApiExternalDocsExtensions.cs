// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiExternalDocs"/> serialization.
    /// </summary>
    internal static class OpenApiExternalDocsExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiExternalDocs externalDocs, IOpenApiWriter writer)
        {
            externalDocs.SerializeInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiExternalDocs externalDocs, IOpenApiWriter writer)
        {
            externalDocs.SerializeInternal(writer);
        }

        private static void SerializeInternal(this OpenApiExternalDocs externalDocs, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (externalDocs != null)
            {
                writer.WriteStringProperty("description", externalDocs.Description);
                writer.WriteStringProperty("url", externalDocs.Url?.OriginalString);
            }
            writer.WriteEndObject();
        }
    }
}
