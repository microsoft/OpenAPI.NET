namespace Microsoft.OpenApi;
/// <summary>
/// Describes an element that has a summary.
/// </summary>
public interface IOpenApiSummarizedElement : IOpenApiElement
{
    /// <summary>
    /// Short description for the example.
    /// </summary>
    public string? Summary { get; set; }
}
