//---------------------------------------------------------------------
// <copyright file="IOpenApiExtension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OpenApi.Any;
using System.Collections.Generic;

namespace Microsoft.OpenApi
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
