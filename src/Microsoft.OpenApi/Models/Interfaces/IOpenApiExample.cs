using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the example object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiExample : IOpenApiDescribedElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiExample>, IOpenApiReferenceable
{
    /// <summary>
    /// Embedded literal example. The value field and externalValue field are mutually
    /// exclusive. To represent examples of media types that cannot naturally represented
    /// in JSON or YAML, use a string value to contain the example, escaping where necessary.
    /// You must use the <see cref="JsonNullSentinel.IsJsonNullSentinel(JsonNode?)"/> method to check whether Default was assigned a null value in the document.
    /// Assign <see cref="JsonNullSentinel.JsonNull"/> to use get null as a serialized value.
    /// </summary>
    public JsonNode? Value { get; }

    /// <summary>
    /// A URL that points to the literal example.
    /// This provides the capability to reference examples that cannot easily be
    /// included in JSON or YAML documents.
    /// The value field and externalValue field are mutually exclusive.
    /// </summary>
    public string? ExternalValue { get; }
}
