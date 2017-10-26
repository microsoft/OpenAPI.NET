// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Requirement Object
    /// </summary>
    public class OpenApiSecurityRequirement : IOpenApiElement
    {
        public Dictionary<OpenApiSecurityScheme, List<string>> Schemes { get; set; } = new Dictionary<OpenApiSecurityScheme, List<string>>();
    }
}
