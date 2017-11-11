// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Base interface for all the types that represent Open API Any.
    /// </summary>
    public interface IOpenApiAny : IOpenApiElement
    {
        /// <summary>
        /// Type of an <see cref="IOpenApiAny"/>.
        /// </summary>
        AnyType AnyType { get; }
    }
}