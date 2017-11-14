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
        /// <returns>The <see cref="OpenApiReference"/> object or null.</returns>
        OpenApiReference FromString(string pointer);

        /// <summary>
        /// Convert the <see cref="OpenApiReference"/> to raw string.
        /// </summary>
        /// <param name="reference">The reference object.</param>
        /// <returns>The reference string.</returns>
        string ToString(OpenApiReference reference);
    }
}
