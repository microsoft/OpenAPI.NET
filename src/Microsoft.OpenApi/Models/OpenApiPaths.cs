// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Paths object.
    /// </summary>
    public class OpenApiPaths : OpenApiExtensibleDictionary<OpenApiPathItem>
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiPaths() {}

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiPaths"/> object
        /// </summary>
        public OpenApiPaths(OpenApiPaths paths) {}

    }
}
