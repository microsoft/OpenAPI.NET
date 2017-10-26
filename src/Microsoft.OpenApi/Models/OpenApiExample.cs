// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embedded literal example.
        /// </summary>
        public IOpenApiAny Value { get; set; }

        /// <summary>
        /// A URL that points to the literal example. 
        /// </summary>
        public Uri ExternalValue { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }
     }
}
