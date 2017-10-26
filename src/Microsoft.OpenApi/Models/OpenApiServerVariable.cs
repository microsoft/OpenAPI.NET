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
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable : IOpenApiExtension
    {
        public string Description { get; set; }
        public string Default { get; set; }
        public List<string> Enum { get; set; } = new List<string>();

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
