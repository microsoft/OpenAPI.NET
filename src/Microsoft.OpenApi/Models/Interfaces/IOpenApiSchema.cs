using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the schema object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiSchema : IOpenApiDescribedElement, IOpenApiSerializable, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiSchema>
{
    
    /// <summary>
    /// Follow JSON Schema definition. Short text providing information about the data.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// $schema, a JSON Schema dialect identifier. Value must be a URI
    /// </summary>
    public Uri Schema { get; }

    /// <summary>
    /// $id - Identifies a schema resource with its canonical URI.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// $comment - reserves a location for comments from schema authors to readers or maintainers of the schema.
    /// </summary>
    public string Comment { get; }

    /// <summary>
    /// $vocabulary- used in meta-schemas to identify the vocabularies available for use in schemas described by that meta-schema.
    /// </summary>
    public IDictionary<string, bool> Vocabulary { get; }

    /// <summary>
    /// $dynamicRef - an applicator that allows for deferring the full resolution until runtime, at which point it is resolved each time it is encountered while evaluating an instance
    /// </summary>
    public string DynamicRef { get; }

    /// <summary>
    /// $dynamicAnchor - used to create plain name fragments that are not tied to any particular structural location for referencing purposes, which are taken into consideration for dynamic referencing.
    /// </summary>
    public string DynamicAnchor { get; }

    /// <summary>
    /// $defs - reserves a location for schema authors to inline re-usable JSON Schemas into a more general schema. 
    /// The keyword does not directly affect the validation result
    /// </summary>
    public IDictionary<string, IOpenApiSchema> Definitions { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public decimal? ExclusiveMaximum { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public decimal? ExclusiveMinimum { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public bool UnEvaluatedProperties { get; }     

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Value MUST be a string in V2 and V3.
    /// </summary>
    public JsonSchemaType? Type { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation
    /// </summary>
    public string Const { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// While relying on JSON Schema's defined formats,
    /// the OAS offers a few additional predefined formats.
    /// </summary>
    public string Format { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public decimal? Maximum { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public decimal? Minimum { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MaxLength { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MinLength { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// This string SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public decimal? MultipleOf { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided.
    /// Unlike JSON Schema, the value MUST conform to the defined type for the Schema Object defined at the same level.
    /// For example, if type is string, then default can be "foo" but cannot be 1.
    /// </summary>
    public JsonNode Default { get; }

    /// <summary>
    /// Relevant only for Schema "properties" definitions. Declares the property as "read only".
    /// This means that it MAY be sent as part of a response but SHOULD NOT be sent as part of the request.
    /// If the property is marked as readOnly being true and is in the required list,
    /// the required will take effect on the response only.
    /// A property MUST NOT be marked as both readOnly and writeOnly being true.
    /// Default value is false.
    /// </summary>
    public bool ReadOnly { get; }

    /// <summary>
    /// Relevant only for Schema "properties" definitions. Declares the property as "write only".
    /// Therefore, it MAY be sent as part of a request but SHOULD NOT be sent as part of the response.
    /// If the property is marked as writeOnly being true and is in the required list,
    /// the required will take effect on the request only.
    /// A property MUST NOT be marked as both readOnly and writeOnly being true.
    /// Default value is false.
    /// </summary>
    public bool WriteOnly { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    public IList<IOpenApiSchema> AllOf { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    public IList<IOpenApiSchema> OneOf { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    public IList<IOpenApiSchema> AnyOf { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    public IOpenApiSchema Not { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public ISet<string> Required { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Value MUST be an object and not an array. Inline or referenced schema MUST be of a Schema Object
    /// and not a standard JSON Schema. items MUST be present if the type is array.
    /// </summary>
    public IOpenApiSchema Items { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MaxItems { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MinItems { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public bool? UniqueItems { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Property definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced).
    /// </summary>
    public IDictionary<string, IOpenApiSchema> Properties { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// PatternProperty definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced)
    /// Each property name of this object SHOULD be a valid regular expression according to the ECMA 262 r
    /// egular expression dialect. Each property value of this object MUST be an object, and each object MUST 
    /// be a valid Schema Object not a standard JSON Schema.
    /// </summary>
    public IDictionary<string, IOpenApiSchema> PatternProperties { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MaxProperties { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public int? MinProperties { get; }

    /// <summary>
    /// Indicates if the schema can contain properties other than those defined by the properties map.
    /// </summary>
    public bool AdditionalPropertiesAllowed { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// Value can be boolean or object. Inline or referenced schema
    /// MUST be of a Schema Object and not a standard JSON Schema.
    /// </summary>
    public IOpenApiSchema AdditionalProperties { get; }

    /// <summary>
    /// Adds support for polymorphism. The discriminator is an object name that is used to differentiate
    /// between other schemas which may satisfy the payload description.
    /// </summary>
    public OpenApiDiscriminator Discriminator { get; }

    /// <summary>
    /// A free-form property to include an example of an instance for this schema.
    /// To represent examples that cannot be naturally represented in JSON or YAML,
    /// a string value can be used to contain the example with escaping where necessary.
    /// </summary>
    public JsonNode Example { get; }

    /// <summary>
    /// A free-form property to include examples of an instance for this schema. 
    /// To represent examples that cannot be naturally represented in JSON or YAML, 
    /// a list of values can be used to contain the examples with escaping where necessary.
    /// </summary>
    public IList<JsonNode> Examples { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public IList<JsonNode> Enum { get; }

    /// <summary>
    /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
    /// </summary>
    public bool UnevaluatedProperties { get; } 

    /// <summary>
    /// Additional external documentation for this schema.
    /// </summary>
    public OpenApiExternalDocs ExternalDocs { get; }

    /// <summary>
    /// Specifies that a schema is deprecated and SHOULD be transitioned out of usage.
    /// Default value is false.
    /// </summary>
    public bool Deprecated { get; }

    /// <summary>
    /// This MAY be used only on properties schemas. It has no effect on root schemas.
    /// Adds additional metadata to describe the XML representation of this property.
    /// </summary>
    public OpenApiXml Xml { get; }

    /// <summary>
    /// This object stores any unrecognized keywords found in the schema.
    /// </summary>
    public IDictionary<string, JsonNode> UnrecognizedKeywords { get; }

    /// <summary>
    /// Any annotation to attach to the schema to be used by the application.
    /// Annotations are NOT (de)serialized with the schema and can be used for custom properties.
    /// </summary>
    public IDictionary<string, object> Annotations { get; }

    /// <summary>
    /// Follow JSON Schema definition:https://json-schema.org/draft/2020-12/json-schema-validation#section-6.5.4
    /// </summary>
    public IDictionary<string, ISet<string>> DependentRequired { get; }
}
