//---------------------------------------------------------------------
// <copyright file="OpenApiServer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer : IOpenApiExtension
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public IDictionary<string, OpenApiServerVariable> Variables { get; set; } = new Dictionary<string, OpenApiServerVariable>();

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
