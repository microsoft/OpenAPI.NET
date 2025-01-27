// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Paths object.
    /// </summary>
    public class OpenApiPaths : OpenApiExtensibleDictionary<IOpenApiPathItem>
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiPaths() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiPaths"/> object
        /// </summary>
        /// <param name="paths">The <see cref="OpenApiPaths"/>.</param>
        public OpenApiPaths(OpenApiPaths paths) : base(dictionary: paths) { }
    }
}
