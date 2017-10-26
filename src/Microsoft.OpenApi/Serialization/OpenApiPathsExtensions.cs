// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiPaths"/> serialization.
    /// </summary>
    internal static class OpenApiPathsExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiPaths"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiPaths paths, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            //writer.WriteStartObject();
            if (paths != null)
            {
                foreach (var pathItem in paths)
                {
                    writer.WritePropertyName(pathItem.Key);
                    pathItem.Value.SerializeV3(writer);
                }
            }
            //writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPaths"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV2(this OpenApiPaths paths, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            //writer.WriteStartObject();
            if (paths != null)
            {
                foreach (var pathItem in paths)
                {
                    writer.WritePropertyName(pathItem.Key);
                    pathItem.Value.SerializeV2(writer);
                }
            }
            //writer.WriteEndObject();
        }
    }
}
