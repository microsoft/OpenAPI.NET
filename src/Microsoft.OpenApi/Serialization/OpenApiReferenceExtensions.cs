// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiReference"/> serialization.
    /// </summary>
    internal static class OpenApiReferenceExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiReference reference, IOpenApiWriter writer)
        {
            // nothing here
        }

        /// <summary>
        /// Serialize <see cref="OpenApiReference"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiReference reference, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
