//---------------------------------------------------------------------
// <copyright file="OpenApiHeader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object 
    /// </summary>
    public class OpenApiHeader : IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Deprecated { get; set; }
        public bool AllowEmptyValue { get; set; }
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
        public OpenApiSchema Schema { get; set; }
        public string Example { get; set; }
        public IList<OpenApiExample> Examples { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
