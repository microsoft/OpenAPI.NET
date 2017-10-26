// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiReference, IOpenApiExtension
    {
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; } = new Dictionary<RuntimeExpression, OpenApiPathItem>();

        public OpenApiReference Pointer { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
