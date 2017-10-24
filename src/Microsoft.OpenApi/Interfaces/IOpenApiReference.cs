//---------------------------------------------------------------------
// <copyright file="IOpenApiReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents an Open API element is referencable.
    /// </summary>
    public interface IOpenApiReference : IOpenApiElement
    {
        /// <summary>
        /// Reference object.
        /// </summary>
        OpenApiReference Pointer { get; set; }
    }
}
