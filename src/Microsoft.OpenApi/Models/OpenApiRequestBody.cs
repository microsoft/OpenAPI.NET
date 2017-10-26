// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Request Body Object
    /// </summary>
    public class OpenApiRequestBody : IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// A brief description of the request body.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines if the request body is required in the request. Defaults to false.
        /// </summary>
        public bool? Required { get; set; }

        /// <summary>
        /// REQUIRED. The content of the request body.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string,IOpenApiAny> Extensions { get; set; }
    }
}
