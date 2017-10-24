//---------------------------------------------------------------------
// <copyright file="OpenApiMediaType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi
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
