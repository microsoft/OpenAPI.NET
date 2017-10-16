//---------------------------------------------------------------------
// <copyright file="OpenApiCallback.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IReference
    {
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; } = new Dictionary<RuntimeExpression, OpenApiPathItem>();

        public OpenApiReference Pointer { get; set; }

        public Dictionary<string, string> Extensions { get; set; }
    }
}
