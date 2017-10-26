// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Extensible Open API element.
    /// </summary>
    public interface IOpenApiExtension : IOpenApiElement
    {
        /// <summary>
        /// Specification extensions.
        /// </summary>
        IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
