// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiDiscriminator"/> serialization.
    /// </summary>
    internal static class OpenApiDiscriminatorExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiDiscriminator discriminator, IOpenApiWriter writer)
        {
            // nothing here
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiDiscriminator discriminator, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
