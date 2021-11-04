// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines a search result model for visited operations.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// An object containing contextual information based on where the walker is currently referencing in an OpenApiDocument.
        /// </summary>
        public CurrentKeys CurrentKeys { get; set; }

        /// <summary>
        /// An Operation object.
        /// </summary>
        public OpenApiOperation Operation { get; set; }
    }
}
