// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiExample"/> serialization.
    /// </summary>
    internal static class OpenApiExampleExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiExample example, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (example == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (example.IsReference())
            {
                example.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteStringProperty("summary", example.Summary);
                writer.WriteStringProperty("description", example.Description);
                if (example.Value != null)
                {
                    writer.WritePropertyName("value");
                    writer.WriteRaw(example.Value);
                }
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiExample example, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
