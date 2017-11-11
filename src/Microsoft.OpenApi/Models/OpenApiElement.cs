// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Open API element.
    /// </summary>
    public abstract class OpenApiElement : IOpenApiElement
    {
        /// <summary>
        /// Write Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal abstract void WriteAsV3(IOpenApiWriter writer);

        /// <summary>
        /// Write Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal abstract void WriteAsV2(IOpenApiWriter writer);
    }
}