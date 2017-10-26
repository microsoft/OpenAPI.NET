// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer : IOpenApiExtension
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public IDictionary<string, OpenApiServerVariable> Variables { get; set; } = new Dictionary<string, OpenApiServerVariable>();

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
