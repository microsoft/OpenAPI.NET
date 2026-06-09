namespace Microsoft.OpenApi;

/// <summary>
/// Compatibility interface for the JSON Schema 2020-12 "contains" keywords support.
/// This interface provides access to the Contains, MaxContains and MinContains properties, which were
/// missed in the initial release of the IOpenApiSchema interface.
///
/// This is a temporary compatibility solution. In the next major version this interface should be
/// merged into IOpenApiSchema.
/// </summary>
public interface IOpenApiSchemaWithContainsProperties
{
    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-contains
    /// An array instance is valid against "contains" if at least one of its elements is valid against this schema.
    /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    IOpenApiSchema? Contains { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation
    /// The number of elements matching the "contains" schema MUST be less than or equal to this value.
    /// </summary>
    uint? MaxContains { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation
    /// The number of elements matching the "contains" schema MUST be greater than or equal to this value.
    /// </summary>
    uint? MinContains { get; }
}
