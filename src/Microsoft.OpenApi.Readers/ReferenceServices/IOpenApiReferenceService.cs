// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ReferenceServices
{
    /// <summary>
    /// Interface for Open API Reference parse.
    /// </summary>
    public interface IOpenApiReferenceService
    {
        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        /// <param name="reference">The <see cref="OpenApiReference"/> object.</param>
        /// <param name="referencedObject">The object that is being referenced.</param>
        /// <returns>
        /// If the reference is found, return true and the referenced object in the out parameter.
        /// In the case of tag, it is psosible that the referenced object does not exist. In this case,
        /// a new tag will be returned in the outer parameter and the return value will be false.
        /// If reference is null, no object will be returned and the return value will be false.
        /// </returns>
        bool TryLoadReference(OpenApiReference reference, out IOpenApiReferenceable referencedObject);

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The reference string.</param>
        /// <param name="type">The type of the reference.</param>
        /// <returns>The <see cref="OpenApiReference"/> object or null.</returns>
        OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type);
    }
}