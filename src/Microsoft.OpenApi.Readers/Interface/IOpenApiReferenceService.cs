// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface for Open API Reference parse.
    /// </summary>
    public interface IOpenApiReferenceService
    {
        /// <summary>
        /// Load the <see cref="IOpenApiElement"/> from a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The <see cref="OpenApiReference"/> object.</param>
        /// <returns>The referenced object or null.</returns>
        IOpenApiReferenceable LoadReference(OpenApiReference reference);

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="pointer">The reference string.</param>
        /// <param name="type">The type of the reference.</param>
        /// <returns>The <see cref="OpenApiReference"/> object or null.</returns>
        OpenApiReference ConvertToOpenApiReference(string pointer, ReferenceType? type);
    }
}