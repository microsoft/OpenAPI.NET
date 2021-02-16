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
    /// Open API Info Object, it provides the metadata about the Open API.
    /// </summary>
    public class OpenApiInfo : IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The title of the application.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A short description of the application.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The version of the OpenAPI document.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// A URL to the Terms of Service for the API. MUST be in the format of a URL.
        /// </summary>
        public Uri TermsOfService { get; set; }

        /// <summary>
        /// The contact information for the exposed API.
        /// </summary>
        public OpenApiContact Contact { get; set; }

        /// <summary>
        /// The license information for the exposed API.
        /// </summary>
        public OpenApiLicense License { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
    }
}
