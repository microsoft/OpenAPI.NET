//---------------------------------------------------------------------
// <copyright file="OpenApiCallback.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi
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
