using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the request body object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiRequestBody : IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiRequestBody>, IOpenApiReferenceable
{
    /// <summary>
    /// Determines if the request body is required in the request. Defaults to false.
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// REQUIRED. The content of the request body. The key is a media type or media type range and the value describes it.
    /// For requests that match multiple keys, only the most specific key is applicable. e.g. text/plain overrides text/*
    /// </summary>
    public IDictionary<string, OpenApiMediaType>? Content { get; }
    /// <summary>
    /// Converts the request body to a body parameter in preparation for a v2 serialization.
    /// </summary>
    /// <param name="writer">The writer to use to read settings from.</param>
    /// <returns>The converted OpenAPI parameter</returns>
    IOpenApiParameter? ConvertToBodyParameter(IOpenApiWriter writer);
    /// <summary>
    /// Converts the request body to a set of form data parameters in preparation for a v2 serialization.
    /// </summary>
    /// <param name="writer">The writer to use to read settings from</param>
    /// <returns>The converted OpenAPI parameters</returns>
    IEnumerable<IOpenApiParameter>? ConvertToFormDataParameters(IOpenApiWriter writer);
}
