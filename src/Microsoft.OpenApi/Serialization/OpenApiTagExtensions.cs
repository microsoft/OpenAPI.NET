// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiTag"/> serialization.
    /// </summary>
    internal static class OpenApiTagExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiTag tag, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (tag != null)
            {
                writer.WriteStringProperty("name", tag.Name);
                writer.WriteStringProperty("description", tag.Description);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiTag tag, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (tag != null)
            {
                writer.WriteStringProperty("name", tag.Name);
                writer.WriteStringProperty("description", tag.Description);
            }
            writer.WriteEndObject();
        }
    }
}
