//---------------------------------------------------------------------
// <copyright file="OpenApiSecurityRequirement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Security Requirement Object
    /// </summary>
    public class OpenApiSecurityRequirement
    {
        public Dictionary<OpenApiSecurityScheme, List<string>> Schemes { get; set; } = new Dictionary<OpenApiSecurityScheme, List<string>>();
    }
}
