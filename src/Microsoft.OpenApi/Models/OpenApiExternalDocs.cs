// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiExternalDocs : IOpenApiExtensible
    {
        /// <summary>
        /// A short description of the target documentation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The URL for the target documentation. Value MUST be in the format of a URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
    }
}
