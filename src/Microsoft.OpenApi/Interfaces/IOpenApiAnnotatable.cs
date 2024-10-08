// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element that can be annotated with
    /// non-serializable properties in a property bag. 
    /// </summary>
    public interface IOpenApiAnnotatable
    {
        /// <summary>
        /// A collection of properties associated with the current OpenAPI element.
        /// </summary>
        IDictionary<string, object> Annotations { get; set; }
    }
}
