// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiPathItem"/> serialization.
    /// </summary>
    internal static class OpenApiPathItemExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiPathItem pathItem, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (pathItem != null)
            {
                writer.WriteStringProperty("summary", pathItem.Summary);
                writer.WriteStringProperty("description", pathItem.Description);
                if (pathItem.Parameters != null && pathItem.Parameters.Count > 0)
                {
                    writer.WritePropertyName("parameters");
                    writer.WriteStartArray();
                    foreach (var parameter in pathItem.Parameters)
                    {
                        parameter.SerializeV3(writer);
                    }
                    writer.WriteEndArray();

                }
                writer.WriteList("servers", pathItem.Servers, (w, s) => s.SerializeV3(w));

                foreach (var operationPair in pathItem.Operations)
                {
                    writer.WritePropertyName(operationPair.Key);
                    operationPair.Value.SerializeV3(writer);
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiPathItem pathItem, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (pathItem != null)
            {
                writer.WriteStringProperty("x-summary", pathItem.Summary);
                writer.WriteStringProperty("x-description", pathItem.Description);
                if (pathItem.Parameters != null && pathItem.Parameters.Count > 0)
                {
                    writer.WritePropertyName("parameters");
                    writer.WriteStartArray();
                    foreach (var parameter in pathItem.Parameters)
                    {
                        parameter.SerializeV2(writer);
                    }
                    writer.WriteEndArray();

                }
                //writer.WriteList("x-servers", pathItem.Servers, WriteServer);

                foreach (var operationPair in pathItem.Operations)
                {
                    writer.WritePropertyName(operationPair.Key);
                    operationPair.Value.SerializeV2(writer);
                }
            }
            writer.WriteEndObject();
        }
    }
}
