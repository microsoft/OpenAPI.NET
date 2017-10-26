// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiMediaType"/> serialization.
    /// </summary>
    internal static class OpenApiMediaTypeExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiMediaType mediaType, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (mediaType != null)
            {
                writer.WriteObject("schema", mediaType.Schema, (w, s) => s.SerializeV3(w));
                writer.WriteObject("example", mediaType.Example, (w, e) => w.WriteRaw(e));
                writer.WriteMap("examples", mediaType.Examples, (w, e) => e.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiMediaType mediaType, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
