using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Describes an element that has a summary and description.
/// </summary>
public interface IOpenApiDescribedElement : IOpenApiElement
{
    /// <summary>
    /// Short description for the example.
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// Long description for the example.
    /// CommonMark syntax MAY be used for rich text representation.
    /// </summary>
    public string Description { get; set; }
}
