//---------------------------------------------------------------------
// <copyright file="OpenApiServerVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable
    {
        public string Description { get; set; }
        public string Default { get; set; }
        public List<string> Enum { get; set; } = new List<string>();

        public Dictionary<string, string> Extensions { get; set; } = new Dictionary<string, string>();
    }
}
