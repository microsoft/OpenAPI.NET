using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    internal class OpenApiSchema
    {
        /// <summary>
        /// Follow JSON Schema definition. Short text providing information about the data.
        /// </summary>
        public string Title { get; set; }

        public string Schema { get; set; }

        public string Id { get; set; }

        public string Comment { get; set; }

        public string Vocabulary { get; set; }

        public string DynamicRef { get; set; }

        public string DynamicAnchor { get; set; }

        public string RecursiveAnchor { get; set; }

        public string RecursiveRef { get; set; }

        public IDictionary<string, OpenApiSchema> Definitions { get; set; }

        public bool UnevaluatedProperties { get; set; }

        public decimal V31ExclusiveMaximum { get; set; }

        public decimal V31ExclusiveMinimum { get; set; }


        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be a string in V2 and V3.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Multiple types via an array are supported in V31.
        /// </summary>
        public string[] TypeArray { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// While relying on JSON Schema's defined formats,
        /// the OAS offers a few additional predefined formats.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? Maximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? ExclusiveMaximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? Minimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// This string SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public decimal? MultipleOf { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided.
        /// Unlike JSON Schema, the value MUST conform to the defined type for the Schema Object defined at the same level.
        /// For example, if type is string, then default can be "foo" but cannot be 1.
        /// </summary>
        public OpenApiAny Default { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "read only".
        /// This means that it MAY be sent as part of a response but SHOULD NOT be sent as part of the request.
        /// If the property is marked as readOnly being true and is in the required list,
        /// the required will take effect on the response only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "write only".
        /// Therefore, it MAY be sent as part of a request but SHOULD NOT be sent as part of the response.
        /// If the property is marked as writeOnly being true and is in the required list,
        /// the required will take effect on the request only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public bool WriteOnly { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> AllOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> OneOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public IList<OpenApiSchema> AnyOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public OpenApiSchema Not { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public ISet<string> Required { get; set; } = new HashSet<string>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be an object and not an array. Inline or referenced schema MUST be of a Schema Object
        /// and not a standard JSON Schema. items MUST be present if the type is array.
        /// </summary>
        public OpenApiSchema Items { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public bool? UniqueItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Property definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced).
        /// </summary>
        public IDictionary<string, OpenApiSchema> Properties { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MaxProperties { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public int? MinProperties { get; set; }

        /// <summary>
        /// Indicates if the schema can contain properties other than those defined by the properties map.
        /// </summary>
        public bool AdditionalPropertiesAllowed { get; set; } = true;

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value can be boolean or object. Inline or referenced schema
        /// MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public OpenApiSchema AdditionalProperties { get; set; }

        /// <summary>
        /// Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.
        /// </summary>
        public OpenApiDiscriminator Discriminator { get; set; }

        /// <summary>
        /// A free-form property to include an example of an instance for this schema.
        /// To represent examples that cannot be naturally represented in JSON or YAML,
        /// a string value can be used to contain the example with escaping where necessary.
        /// </summary>
        public OpenApiAny Example { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public IList<OpenApiAny> Enum { get; set; } = new List<OpenApiAny>();

        /// <summary>
        /// Allows sending a null value for the defined schema. Default value is false.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Additional external documentation for this schema.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// Specifies that a schema is deprecated and SHOULD be transitioned out of usage.
        /// Default value is false.
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        /// This MAY be used only on properties schemas. It has no effect on root schemas.
        /// Adds additional metadata to describe the XML representation of this property.
        /// </summary>
        public OpenApiXml Xml { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiSchema() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiSchema"/> object
        /// </summary>
        public OpenApiSchema(OpenApiSchema schema)
        {
            Title = schema?.Title ?? Title;
            Id = schema?.Id ?? Id;
            Schema = schema?.Schema ?? Schema;
            Comment = schema?.Comment ?? Comment;
            Vocabulary = schema?.Vocabulary ?? Vocabulary;
            DynamicAnchor = schema?.DynamicAnchor ?? DynamicAnchor;
            DynamicRef = schema?.DynamicRef ?? DynamicRef;
            RecursiveAnchor = schema?.RecursiveAnchor ?? RecursiveAnchor;
            RecursiveRef = schema?.RecursiveRef ?? RecursiveRef;
            Definitions = schema?.Definitions != null ? new Dictionary<string, OpenApiSchema>(schema.Definitions) : null;
            UnevaluatedProperties = schema?.UnevaluatedProperties ?? UnevaluatedProperties;
            V31ExclusiveMaximum = schema?.V31ExclusiveMaximum ?? V31ExclusiveMaximum;
            V31ExclusiveMinimum = schema?.V31ExclusiveMinimum ?? V31ExclusiveMinimum;
            Type = schema?.Type ?? Type;
            Format = schema?.Format ?? Format;
            Description = schema?.Description ?? Description;
            Maximum = schema?.Maximum ?? Maximum;
            ExclusiveMaximum = schema?.ExclusiveMaximum ?? ExclusiveMaximum;
            Minimum = schema?.Minimum ?? Minimum;
            ExclusiveMinimum = schema?.ExclusiveMinimum ?? ExclusiveMinimum;
            MaxLength = schema?.MaxLength ?? MaxLength;
            MinLength = schema?.MinLength ?? MinLength;
            Pattern = schema?.Pattern ?? Pattern;
            MultipleOf = schema?.MultipleOf ?? MultipleOf;
            Default = schema?.Default != null ? new(schema?.Default.Node) : null;
            ReadOnly = schema?.ReadOnly ?? ReadOnly;
            WriteOnly = schema?.WriteOnly ?? WriteOnly;
            AllOf = schema?.AllOf != null ? new List<OpenApiSchema>(schema.AllOf) : null;
            OneOf = schema?.OneOf != null ? new List<OpenApiSchema>(schema.OneOf) : null;
            AnyOf = schema?.AnyOf != null ? new List<OpenApiSchema>(schema.AnyOf) : null;
            Not = schema?.Not != null ? new(schema?.Not) : null;
            Required = schema?.Required != null ? new HashSet<string>(schema.Required) : null;
            Items = schema?.Items != null ? new(schema?.Items) : null;
            MaxItems = schema?.MaxItems ?? MaxItems;
            MinItems = schema?.MinItems ?? MinItems;
            UniqueItems = schema?.UniqueItems ?? UniqueItems;
            Properties = schema?.Properties != null ? new Dictionary<string, OpenApiSchema>(schema.Properties) : null;
            MaxProperties = schema?.MaxProperties ?? MaxProperties;
            MinProperties = schema?.MinProperties ?? MinProperties;
            AdditionalPropertiesAllowed = schema?.AdditionalPropertiesAllowed ?? AdditionalPropertiesAllowed;
            AdditionalProperties = schema?.AdditionalProperties != null ? new(schema?.AdditionalProperties) : null;
            Discriminator = schema?.Discriminator != null ? new(schema?.Discriminator) : null;
            Example = schema?.Example != null ? new(schema?.Example.Node) : null;
            Enum = schema?.Enum != null ? new List<OpenApiAny>(schema.Enum) : null;
            Nullable = schema?.Nullable ?? Nullable;
            ExternalDocs = schema?.ExternalDocs != null ? new(schema?.ExternalDocs) : null;
            Deprecated = schema?.Deprecated ?? Deprecated;
            Xml = schema?.Xml != null ? new(schema?.Xml) : null;
            Extensions = schema?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(schema.Extensions) : null;
            UnresolvedReference = schema?.UnresolvedReference ?? UnresolvedReference;
            Reference = schema?.Reference != null ? new(schema?.Reference) : null;
        }
    }
}
