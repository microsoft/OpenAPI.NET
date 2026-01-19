using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Represents an Extensible Open API element elements can be read from.
/// </summary>
public interface IOpenApiReadOnlyExtensible
{
    /// <summary>
    /// Specification extensions.
    /// </summary>
    IDictionary<string, IOpenApiExtension>? Extensions { get; }

}
