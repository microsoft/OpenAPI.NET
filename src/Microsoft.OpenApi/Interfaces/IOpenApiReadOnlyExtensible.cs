﻿using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Represents an Extensible Open API element elements can be rad from.
/// </summary>
public interface IOpenApiReadOnlyExtensible
{
    /// <summary>
    /// Specification extensions.
    /// </summary>
    Dictionary<string, IOpenApiExtension>? Extensions { get; }

}
