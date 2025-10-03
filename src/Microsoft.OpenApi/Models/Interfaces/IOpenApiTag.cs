namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the path item object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiTag : IOpenApiReadOnlyExtensible, IOpenApiReadOnlyDescribedElement, IShallowCopyable<IOpenApiTag>, IOpenApiReferenceable
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Additional external documentation for this tag.
    /// </summary>
    public OpenApiExternalDocs? ExternalDocs { get; }

    /// <summary>
    /// A short summary of the tag, used for display purposes.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// The tag that this tag is nested under.
    /// </summary>
    public OpenApiTagReference? Parent { get; }

    /// <summary>
    /// A machine-readable string to categorize what sort of tag it is.
    /// </summary>
    public string? Kind { get; }
}
