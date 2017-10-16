//---------------------------------------------------------------------
// <copyright file="OpenApiLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Link Object.
    /// </summary>
    public class OpenApiLink :  IReference
    {
        public string Href { get; set; }
        public string OperationId { get; set; }
        public Dictionary<string, RuntimeExpression> Parameters { get; set; }
        public RuntimeExpression RequestBody { get; set; }

        public string Description { get; set; }
        public Dictionary<string, string> Extensions { get; set; }

        public OpenApiReference Pointer { get; set; }


        
    }
}
