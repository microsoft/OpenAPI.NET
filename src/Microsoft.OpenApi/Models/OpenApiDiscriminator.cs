// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : OpenApiElement
    {
        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            // nothing here
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
