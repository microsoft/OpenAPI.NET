// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOpenApiReferenceableWithTarget<T> : IOpenApiReferenceable
    {
        /// <summary>
        /// Gets the resolved target object.
        /// </summary>
        T Target { get; }
    }
}
