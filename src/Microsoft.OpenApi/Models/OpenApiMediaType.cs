//---------------------------------------------------------------------
// <copyright file="OpenApiMediaType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType
    {
        public OpenApiSchema Schema { get; set; }
        public Dictionary<string, OpenApiExample> Examples { get; set; }
        public string Example { get; set; }

        public Dictionary<string, string> Extensions { get; set; }
    }
}
