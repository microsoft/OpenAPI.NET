using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Describes an element that has a description.
/// </summary>
public interface IOpenApiDescribedElement : IOpenApiElement
{
    /// <summary>
    /// Long description for the example.
    /// CommonMark syntax MAY be used for rich text representation.
    /// </summary>
    public string Description { get; set; }
}

/// <summary>
/// Describes an element that has a description.
/// </summary>
public interface IOpenApiReadOnlyDescribedElement : IOpenApiElement
{
    /// <summary>
    /// Long description for the example.
    /// CommonMark syntax MAY be used for rich text representation.
    /// </summary>
    public string Description { get; }
}
