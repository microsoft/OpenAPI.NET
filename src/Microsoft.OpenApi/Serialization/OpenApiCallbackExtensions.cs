// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiCallback"/> serialization.
    /// </summary>
    internal static class OpenApiCallbackExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiCallback callback, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (callback == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (callback.IsReference())
            {
                callback.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                foreach (var item in callback.PathItems)
                {
                    writer.WriteObject<OpenApiPathItem>(item.Key.Expression, item.Value, (w, p) => p.SerializeV3(w));
                }
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiCallback callback, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
