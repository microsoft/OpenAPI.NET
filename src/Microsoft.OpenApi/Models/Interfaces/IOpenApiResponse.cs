using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the response object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiResponse : IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiResponse>, IOpenApiReferenceable
{
    /// <summary>
    /// Maps a header name to its definition.
    /// </summary>
    public OrderedDictionary<string, IOpenApiHeader>? Headers { get; }

    /// <summary>
    /// A map containing descriptions of potential response payloads.
    /// The key is a media type or media type range and the value describes it.
    /// </summary>
    public OrderedDictionary<string, OpenApiMediaType>? Content { get; }

    /// <summary>
    /// A map of operations links that can be followed from the response.
    /// The key of the map is a short name for the link,
    /// following the naming constraints of the names for Component Objects.
    /// </summary>
    public OrderedDictionary<string, IOpenApiLink>? Links { get; }
}
