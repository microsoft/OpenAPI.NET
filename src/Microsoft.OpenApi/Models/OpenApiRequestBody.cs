//---------------------------------------------------------------------
// <copyright file="OpenApiRequestBody.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Request Body Object
    /// </summary>
    public class OpenApiRequestBody : IReference
    {
        public OpenApiReference Pointer { get; set; }

        public string Description { get; set; }
        public Boolean Required { get; set; }
        public Dictionary<string, OpenApiMediaType> Content { get; set; }
        public Dictionary<string,string> Extensions { get; set; }
    }
}
