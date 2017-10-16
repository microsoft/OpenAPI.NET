//---------------------------------------------------------------------
// <copyright file="OpenApiServer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public Dictionary<string, OpenApiServerVariable> Variables { get; set; } = new Dictionary<string, OpenApiServerVariable>();

        public Dictionary<string, string> Extensions { get; set; } = new Dictionary<string, string>();
    }
}
