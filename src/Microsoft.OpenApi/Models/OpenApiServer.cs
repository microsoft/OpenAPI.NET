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
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer : IOpenApiExtension
    {
        /// <summary>
        /// A URL to the target host.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// An optional string describing the host designated by the URL.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A map between a variable name and its value. 
        /// </summary>
        public IDictionary<string, OpenApiServerVariable> Variables { get; set; } = new Dictionary<string, OpenApiServerVariable>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
