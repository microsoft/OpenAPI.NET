using System.Collections.Generic;

namespace Microsoft.OpenApi.Interfaces;

/// <summary>
/// Represents an Extensible Open API element elements can be rad from.
/// </summary>
public interface IOpenApiReadOnlyExtensible
{
    /// <summary>
    /// Specification extensions.
    /// </summary>
    IDictionary<string, IOpenApiExtension>? Extensions { get; }

}
