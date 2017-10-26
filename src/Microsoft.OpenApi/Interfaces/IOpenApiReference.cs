// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element is referencable.
    /// </summary>
    public interface IOpenApiReference : IOpenApiElement
    {
        /// <summary>
        /// Reference object.
        /// </summary>
        OpenApiReference Pointer { get; set; }
    }
}
