// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element is writable.
    /// </summary>
    public interface IOpenApiWritable
    {
        /// <summary>
        /// Write Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void WriteAsV3(IOpenApiWriter writer);

        /// <summary>
        /// Write Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void WriteAsV2(IOpenApiWriter writer);
    }
}
