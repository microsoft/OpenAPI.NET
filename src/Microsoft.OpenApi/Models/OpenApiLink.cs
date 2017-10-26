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
    /// Link Object.
    /// </summary>
    public class OpenApiLink :  IOpenApiReference, IOpenApiExtension
    {
        public string Href { get; set; }
        public string OperationId { get; set; }
        public Dictionary<string, RuntimeExpression> Parameters { get; set; }
        public RuntimeExpression RequestBody { get; set; }

        public string Description { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer { get; set; }
    }
}
