// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiExtension
    {
        /// <summary>
        /// The schema defining the type used for the request body.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// Example of the media type.
        /// </summary>
        public IOpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; }

        /// <summary>
        /// A map between a property name and its encoding information. 
        /// </summary>
        public IDictionary<string, OpenApiEncoding> Encoding { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
