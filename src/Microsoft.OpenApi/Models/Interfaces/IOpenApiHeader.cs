
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the headers object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiHeader : IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiHeader>, IOpenApiReferenceable
{
    /// <summary>
    /// Determines whether this header is mandatory.
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// Specifies that a header is deprecated and SHOULD be transitioned out of usage.
    /// </summary>
    public bool Deprecated { get; }

    /// <summary>
    /// Sets the ability to pass empty-valued headers.
    /// </summary>
    public bool AllowEmptyValue { get; }

    /// <summary>
    /// Describes how the header value will be serialized depending on the type of the header value.
    /// </summary>
    public ParameterStyle? Style { get; }

    /// <summary>
    /// When this is true, header values of type array or object generate separate parameters
    /// for each value of the array or key-value pair of the map.
    /// </summary>
    public bool Explode { get; }

    /// <summary>
    /// Determines whether the header value SHOULD allow reserved characters, as defined by RFC3986.
    /// </summary>
    public bool AllowReserved { get; }

    /// <summary>
    /// The schema defining the type used for the request body.
    /// </summary>
    public IOpenApiSchema? Schema { get; }

    /// <summary>
    /// Example of the media type.
    /// </summary>
    public JsonNode? Example { get; }

    /// <summary>
    /// Examples of the media type.
    /// </summary>
    public OrderedDictionary<string, IOpenApiExample>? Examples { get; }

    /// <summary>
    /// A map containing the representations for the header.
    /// </summary>
    public OrderedDictionary<string, OpenApiMediaType>? Content { get; }

}
