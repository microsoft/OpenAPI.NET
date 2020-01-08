// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element capable of resolving references.
    /// </summary>
    public interface IOpenApiReferenceResolver
    {
        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        IOpenApiReferenceable ResolveReference(OpenApiReference reference);
    }
}
