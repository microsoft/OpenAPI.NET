// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    /// <typeparam name="T">The type of the target being referenced</typeparam>
    /// <typeparam name="U">The type of the interface implemented by both the target and the reference type</typeparam>
    /// <typeparam name="V">The type for the reference holding the additional fields and annotations</typeparam>
    public interface IOpenApiReferenceHolder<out T, U, V> : IOpenApiReferenceHolder<V> where T : IOpenApiReferenceable, U where V : OpenApiReference, new()
    {
        /// <summary>
        /// Gets the resolved target object.
        /// </summary>
        U? Target { get; }

        /// <summary>
        /// Gets the recursively resolved target object.
        /// </summary>
        T? RecursiveTarget { get; }

        /// <summary>
        /// Copy the reference as a target element with overrides.
        /// </summary>
        U CopyReferenceAsTargetElementWithOverrides(U source);
    }
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    /// <typeparam name="V">The type for the reference holding the additional fields and annotations</typeparam>
    public interface IOpenApiReferenceHolder<V> : IOpenApiReferenceHolder where V : OpenApiReference, new()
    {
        /// <summary>
        /// Reference object.
        /// </summary>
        V Reference { get; init; }
    }
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    public interface IOpenApiReferenceHolder : IOpenApiSerializable
    {
        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        bool UnresolvedReference { get; }
    }
}
