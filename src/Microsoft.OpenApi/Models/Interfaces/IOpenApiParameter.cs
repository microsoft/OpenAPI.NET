using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the parameter object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiParameter : IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiParameter>, IOpenApiReferenceable
{
    /// <summary>
    /// REQUIRED. The name of the parameter. Parameter names are case sensitive.
    /// If in is "path", the name field MUST correspond to the associated path segment from the path field in the Paths Object.
    /// If in is "header" and the name field is "Accept", "Content-Type" or "Authorization", the parameter definition SHALL be ignored.
    /// For all other cases, the name corresponds to the parameter name used by the in property.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// REQUIRED. The location of the parameter.
    /// Possible values are "query", "header", "path" or "cookie".
    /// </summary>
    public ParameterLocation? In { get; }

    /// <summary>
    /// Determines whether this parameter is mandatory.
    /// If the parameter location is "path", this property is REQUIRED and its value MUST be true.
    /// Otherwise, the property MAY be included and its default value is false.
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// Specifies that a parameter is deprecated and SHOULD be transitioned out of usage.
    /// </summary>
    public bool Deprecated { get; }

    /// <summary>
    /// Sets the ability to pass empty-valued parameters.
    /// This is valid only for query parameters and allows sending a parameter with an empty value.
    /// Default value is false.
    /// If style is used, and if behavior is n/a (cannot be serialized),
    /// the value of allowEmptyValue SHALL be ignored.
    /// </summary>
    public bool AllowEmptyValue { get; }

    /// <summary>
    /// Describes how the parameter value will be serialized depending on the type of the parameter value.
    /// Default values (based on value of in): for query - form; for path - simple; for header - simple;
    /// for cookie - form.
    /// </summary>
    public ParameterStyle? Style { get; }

    /// <summary>
    /// When this is true, parameter values of type array or object generate separate parameters
    /// for each value of the array or key-value pair of the map.
    /// For other types of parameters this property has no effect.
    /// When style is form, the default value is true.
    /// For all other styles, the default value is false.
    /// </summary>
    public bool Explode { get; }

    /// <summary>
    /// Determines whether the parameter value SHOULD allow reserved characters,
    /// as defined by RFC3986 :/?#[]@!$&amp;'()*+,;= to be included without percent-encoding.
    /// This property only applies to parameters with an in value of query.
    /// The default value is false.
    /// </summary>
    public bool AllowReserved { get; }

    /// <summary>
    /// The schema defining the type used for the parameter.
    /// </summary>
    public IOpenApiSchema? Schema { get; }

    /// <summary>
    /// Examples of the media type. Each example SHOULD contain a value
    /// in the correct format as specified in the parameter encoding.
    /// The examples object is mutually exclusive of the example object.
    /// Furthermore, if referencing a schema which contains an example,
    /// the examples value SHALL override the example provided by the schema.
    /// </summary>
    public IDictionary<string, IOpenApiExample>? Examples { get; }

    /// <summary>
    /// Example of the media type. The example SHOULD match the specified schema and encoding properties
    /// if present. The example object is mutually exclusive of the examples object.
    /// Furthermore, if referencing a schema which contains an example,
    /// the example value SHALL override the example provided by the schema.
    /// To represent examples of media types that cannot naturally be represented in JSON or YAML,
    /// a string value can contain the example with escaping where necessary.
    /// You must use the <see cref="JsonNullSentinel.IsJsonNullSentinel(JsonNode?)"/> method to check whether Default was assigned a null value in the document.
    /// Assign <see cref="JsonNullSentinel.JsonNull"/> to use get null as a serialized value.
    /// </summary>
    public JsonNode? Example { get; }

    /// <summary>
    /// A map containing the representations for the parameter.
    /// The key is the media type and the value describes it.
    /// The map MUST only contain one entry.
    /// For more complex scenarios, the content property can define the media type and schema of the parameter.
    /// A parameter MUST contain either a schema property, or a content property, but not both.
    /// When example or examples are provided in conjunction with the schema object,
    /// the example MUST follow the prescribed serialization strategy for the parameter.
    /// </summary>
    public IDictionary<string, IOpenApiMediaType>? Content { get; }    
}
