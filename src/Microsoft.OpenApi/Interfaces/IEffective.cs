// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// OpenApiElements that implement IEffective indicate that their description is not self-contained.
    /// External elements affect the effective description.
    /// </summary>
    /// <remarks>Currently this will only be used for accessing external references.
    /// In the next major version, this will be the approach accessing all referenced elements.
    /// This will enable us to support merging properties that are peers of the $ref  </remarks>
    /// <typeparam name="T">Type of OpenApi Element that is being referenced.</typeparam>
    public interface IEffective<T>  where T : class,IOpenApiElement
    {
        /// <summary>
        /// Returns a calculated and cloned version of the element.
        /// </summary>
        T GetEffective(OpenApiDocument document);
    }
}
