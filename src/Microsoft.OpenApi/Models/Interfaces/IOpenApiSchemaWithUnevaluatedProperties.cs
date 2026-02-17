namespace Microsoft.OpenApi;

/// <summary>
/// Compatibility interface for UnevaluatedProperties schema support.
/// This interface provides access to the UnevaluatedPropertiesSchema property, which represents
/// the schema for unevaluated properties as defined in JSON Schema draft 2020-12.
/// 
/// NOTE: This is a temporary compatibility solution. In the next major version:
/// - This interface will be merged into IOpenApiSchema
/// - The UnevaluatedPropertiesSchema property will be renamed to UnevaluatedProperties
/// - The current UnevaluatedProperties boolean property will be renamed to UnevaluatedPropertiesAllowed
/// </summary>
/// <remarks>
/// TODO: Remove this interface in the next major version and merge its content into IOpenApiSchema.
/// </remarks>
public interface IOpenApiSchemaWithUnevaluatedProperties
{
    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-core#name-unevaluatedproperties
    /// This is a schema that unevaluated properties must validate against.
    /// When serialized, this takes precedence over the UnevaluatedProperties boolean property.
    /// </summary>
    /// <remarks>
    /// NOTE: This property differs from the naming pattern of AdditionalProperties/AdditionalPropertiesAllowed 
    /// for binary compatibility reasons. In the next major version:
    /// - This property will be renamed to UnevaluatedProperties
    /// - The current boolean UnevaluatedProperties property will be renamed to UnevaluatedPropertiesAllowed
    /// 
    /// TODO: Rename this property to UnevaluatedProperties in the next major version.
    /// </remarks>
    IOpenApiSchema? UnevaluatedPropertiesSchema { get; }
}
