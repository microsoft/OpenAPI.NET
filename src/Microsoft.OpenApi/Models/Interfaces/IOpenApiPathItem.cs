
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the path item object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiPathItem : IOpenApiDescribedElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiPathItem>, IOpenApiReferenceable
{
    /// <summary>
    /// Gets the definition of operations on this path.
    /// </summary>
    public Dictionary<HttpMethod, OpenApiOperation>? Operations { get; }

    /// <summary>
    /// An alternative server array to service all operations in this path.
    /// </summary>
    public IList<OpenApiServer>? Servers { get; }

    /// <summary>
    /// A list of parameters that are applicable for all the operations described under this path.
    /// These parameters can be overridden at the operation level, but cannot be removed there.
    /// </summary>
    public IList<IOpenApiParameter>? Parameters { get; }
}
