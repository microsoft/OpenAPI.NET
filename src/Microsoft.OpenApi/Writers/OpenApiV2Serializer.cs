// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Class to serialize Open API v2.0 document.
    /// </summary>
    internal static class OpenApiV2Serializer
    {
        /// <summary>
        /// Write <see cref="OpenApiDocument"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiDocument document)
        {
            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // add the logic to write v2.0 document.
        }
    }
}
