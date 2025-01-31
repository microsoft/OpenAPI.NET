using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the path item object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiTag : IOpenApiSerializable, IOpenApiReadOnlyExtensible, IOpenApiReadOnlyDescribedElement
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Additional external documentation for this tag.
    /// </summary>
    public OpenApiExternalDocs ExternalDocs { get; }
}
