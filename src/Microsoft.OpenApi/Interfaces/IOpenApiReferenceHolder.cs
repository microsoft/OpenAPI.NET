// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    /// <typeparam name="T">Type of the target being referenced</typeparam>
    public interface IOpenApiReferenceHolder<out T> : IOpenApiReferenceHolder where T : IOpenApiReferenceable
    {
        /// <summary>
        /// Gets the resolved target object.
        /// </summary>
        T Target { get; }
    }
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    /// <typeparam name="T">The type of the target being referenced</typeparam>
    /// <typeparam name="V">The type of the interface implemented by both the target and the reference type</typeparam>
    public interface IOpenApiReferenceHolder<out T, V> : IOpenApiReferenceHolder<T> where T : IOpenApiReferenceable, V
    {
        //TODO merge this interface with the previous once all implementations are updated
        /// <summary>
        /// Copy the reference as a target element with overrides.
        /// </summary>
        V CopyReferenceAsTargetElementWithOverrides(V source);
    }
    /// <summary>
    /// A generic interface for OpenApiReferenceable objects that have a target.
    /// </summary>
    public interface IOpenApiReferenceHolder : IOpenApiSerializable
    {
        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        bool UnresolvedReference { get; set; }
        //TODO the UnresolvedReference property setter should be removed and a default implementation that checks whether the target is null for the getter should be provided instead

        /// <summary>
        /// Reference object.
        /// </summary>
        OpenApiReference Reference { get; set; }
    }
}
