// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi;
/// <summary>
/// Represents an Open API element that can be annotated with
/// non-serializable properties in a property bag. 
/// </summary>
public interface IMetadataContainer
{
    /// <summary>
    /// A collection of properties associated with the current OpenAPI element to be used by the application.
    /// Metadata are NOT (de)serialized with the schema and can be used for custom properties.
    /// </summary>
    IDictionary<string, object>? Metadata { get; set; }
}
