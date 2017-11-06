// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Any type kind.
    /// </summary>
    public enum AnyTypeKind
    {
        /// <summary>
        /// Primitive.
        /// </summary>
        Primitive,

        /// <summary>
        /// Null.
        /// </summary>
        Null,

        /// <summary>
        /// Array.
        /// </summary>
        Array,

        /// <summary>
        /// Object.
        /// </summary>
        Object
    }

    /// <summary>
    /// Base interface for the Open Api Any.
    /// </summary>
    public interface IOpenApiAny : IOpenApiElement
    {
        /// <summary>
        /// Any type kind.
        /// </summary>
        AnyTypeKind AnyKind { get; }
    }
}
