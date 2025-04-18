// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Interfaces;

/// <summary>
/// Represents an Extensible Open API element elements can be rad from.
/// </summary>
public interface IOpenApiReadOnlyExtensible
{
    /// <summary>
    /// Specification extensions.
    /// </summary>
    OpenApiExtensionDictionary? Extensions { get; }
}
