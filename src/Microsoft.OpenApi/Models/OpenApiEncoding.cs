// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Encoding Object.
    /// </summary>
    public class OpenApiEncoding : IOpenApiExtension
    {
        /// <summary>
        /// The Content-Type for encoding a specific property.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// A map allowing additional information to be provided as headers.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; }

        /// <summary>
        /// Describes how a specific property value will be serialized depending on its type. 
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// Explode
        /// </summary>
        public bool? Explode { get; set; }

        /// <summary>
        /// AllowReserved
        /// </summary>
        public bool? AllowReserved { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
