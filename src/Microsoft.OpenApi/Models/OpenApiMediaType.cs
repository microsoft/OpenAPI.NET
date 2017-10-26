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
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiExtension
    {
        public OpenApiSchema Schema { get; set; }
        public IDictionary<string, OpenApiExample> Examples { get; set; }
        public string Example { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
