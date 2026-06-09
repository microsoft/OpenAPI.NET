using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Compatibility interface for schema properties that cannot be added to <see cref="IOpenApiSchema"/>
/// in the current major version without a breaking change.
/// This interface provides access to those properties in contexts where callers need a typed model surface.
/// </summary>
/// <remarks>
/// TODO: Remove this interface in the next major version and merge its content into IOpenApiSchema.
/// </remarks>
public interface IOpenApiSchemaMissingProperties
{
    /// <summary>
    /// $anchor - identifies a plain-name location-independent fragment within the schema resource.
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-anchor
    /// </summary>
    public string? Anchor { get; }

    /// <summary>
    /// Indicates whether unevaluated properties are allowed. When false, no unevaluated properties are permitted.
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-unevaluatedproperties
    /// Only serialized when false and <see cref="UnevaluatedPropertiesSchema"/> is null.
    /// </summary>
    /// <remarks>
    /// NOTE: This property differs from the naming pattern of AdditionalPropertiesAllowed for binary compatibility reasons.
    /// In the next major version, this will be renamed to UnevaluatedPropertiesAllowed.
    /// TODO: Rename to UnevaluatedPropertiesAllowed in the next major version.
    /// </remarks>
    public bool UnevaluatedProperties { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-unevaluatedproperties
    /// This is a schema that unevaluated properties must validate against.
    /// When serialized, this takes precedence over the <see cref="UnevaluatedProperties"/> boolean property.
    /// </summary>
    /// <remarks>
    /// NOTE: This property differs from the naming pattern of AdditionalProperties/AdditionalPropertiesAllowed
    /// for binary compatibility reasons. In the next major version:
    /// - This property will be renamed to UnevaluatedProperties
    /// - The current boolean UnevaluatedProperties property will be renamed to UnevaluatedPropertiesAllowed
    ///
    /// TODO: Rename this property to UnevaluatedProperties in the next major version.
    /// </remarks>
    public IOpenApiSchema? UnevaluatedPropertiesSchema { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation#name-contentencoding
    /// contentEncoding - identifies the encoding of string content.
    /// </summary>
    public string? ContentEncoding { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation#name-contentmediatype
    /// contentMediaType - identifies the media type of string content.
    /// </summary>
    public string? ContentMediaType { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation#name-contentschema
    /// contentSchema - provides a schema that describes the decoded string content.
    /// </summary>
    public IOpenApiSchema? ContentSchema { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-propertynames
    /// propertyNames - provides a schema that validates property names.
    /// </summary>
    public IOpenApiSchema? PropertyNames { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-dependentschemas
    /// dependentSchemas - maps property names to schemas that are applied when that property is present.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? DependentSchemas { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-if
    /// if - applies a conditional schema that determines whether <see cref="Then"/> or <see cref="Else"/> should be evaluated.
    /// </summary>
    public IOpenApiSchema? If { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-then
    /// then - applies when <see cref="If"/> evaluates successfully.
    /// </summary>
    public IOpenApiSchema? Then { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-else
    /// else - applies when <see cref="If"/> does not evaluate successfully.
    /// </summary>
    public IOpenApiSchema? Else { get; }
}
