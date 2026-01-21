using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Describes an element that has content.
/// </summary>
public interface IOpenApiContentElement
{
    /// <summary>
    /// A map containing descriptions of potential payloads.
    /// The key is a media type or media type range and the value describes it.
    /// </summary>
    IDictionary<string, OpenApiMediaType>? Content { get; set; }
}

/// <summary>
/// Describes an element that has content.
/// </summary>
public interface IOpenApiReadOnlyContentElement
{
    /// <summary>
    /// A map containing descriptions of potential payloads.
    /// The key is a media type or media type range and the value describes it.
    /// </summary>
    IDictionary<string, OpenApiMediaType>? Content { get; }
}
