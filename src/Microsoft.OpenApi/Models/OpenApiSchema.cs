// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The Schema Object allows the definition of input and output data types.
    /// </summary>
    public class OpenApiSchema : IOpenApiAnnotatable, IOpenApiExtensible, IOpenApiReferenceable
    {
        /// <summary>
        /// Follow JSON Schema definition. Short text providing information about the data.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// $schema, a JSON Schema dialect identifier. Value must be a URI
        /// </summary>
        public virtual string Schema { get; set; }

        /// <summary>
        /// $id - Identifies a schema resource with its canonical URI.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// $comment - reserves a location for comments from schema authors to readers or maintainers of the schema.
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// $vocabulary- used in meta-schemas to identify the vocabularies available for use in schemas described by that meta-schema.
        /// </summary>
        public virtual IDictionary<string, bool> Vocabulary { get; set; }

        /// <summary>
        /// $dynamicRef - an applicator that allows for deferring the full resolution until runtime, at which point it is resolved each time it is encountered while evaluating an instance
        /// </summary>
        public virtual string DynamicRef { get; set; }

        /// <summary>
        /// $dynamicAnchor - used to create plain name fragments that are not tied to any particular structural location for referencing purposes, which are taken into consideration for dynamic referencing.
        /// </summary>
        public virtual string DynamicAnchor { get; set; }

        /// <summary>
        /// $defs - reserves a location for schema authors to inline re-usable JSON Schemas into a more general schema. 
        /// The keyword does not directly affect the validation result
        /// </summary>
        public virtual IDictionary<string, OpenApiSchema> Definitions { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual decimal? V31ExclusiveMaximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual decimal? V31ExclusiveMinimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual bool UnEvaluatedProperties { get; set; }     

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be a string in V2 and V3.
        /// </summary>
        public virtual JsonSchemaType? Type { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://json-schema.org/draft/2020-12/json-schema-validation
        /// </summary>
        public virtual string Const { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// While relying on JSON Schema's defined formats,
        /// the OAS offers a few additional predefined formats.
        /// </summary>
        public virtual string Format { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual decimal? Maximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual bool? ExclusiveMaximum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual decimal? Minimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual bool? ExclusiveMinimum { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MaxLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MinLength { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// This string SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect
        /// </summary>
        public virtual string Pattern { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual decimal? MultipleOf { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// The default value represents what would be assumed by the consumer of the input as the value of the schema if one is not provided.
        /// Unlike JSON Schema, the value MUST conform to the defined type for the Schema Object defined at the same level.
        /// For example, if type is string, then default can be "foo" but cannot be 1.
        /// </summary>
        public virtual JsonNode Default { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "read only".
        /// This means that it MAY be sent as part of a response but SHOULD NOT be sent as part of the request.
        /// If the property is marked as readOnly being true and is in the required list,
        /// the required will take effect on the response only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public virtual bool ReadOnly { get; set; }

        /// <summary>
        /// Relevant only for Schema "properties" definitions. Declares the property as "write only".
        /// Therefore, it MAY be sent as part of a request but SHOULD NOT be sent as part of the response.
        /// If the property is marked as writeOnly being true and is in the required list,
        /// the required will take effect on the request only.
        /// A property MUST NOT be marked as both readOnly and writeOnly being true.
        /// Default value is false.
        /// </summary>
        public virtual bool WriteOnly { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public virtual IList<OpenApiSchema> AllOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public virtual IList<OpenApiSchema> OneOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public virtual IList<OpenApiSchema> AnyOf { get; set; } = new List<OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Inline or referenced schema MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public virtual OpenApiSchema Not { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual ISet<string> Required { get; set; } = new HashSet<string>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value MUST be an object and not an array. Inline or referenced schema MUST be of a Schema Object
        /// and not a standard JSON Schema. items MUST be present if the type is array.
        /// </summary>
        public virtual OpenApiSchema Items { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MaxItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MinItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual bool? UniqueItems { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Property definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced).
        /// </summary>
        public virtual IDictionary<string, OpenApiSchema> Properties { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// PatternProperty definitions MUST be a Schema Object and not a standard JSON Schema (inline or referenced)
        /// Each property name of this object SHOULD be a valid regular expression according to the ECMA 262 r
        /// egular expression dialect. Each property value of this object MUST be an object, and each object MUST 
        /// be a valid Schema Object not a standard JSON Schema.
        /// </summary>
        public virtual IDictionary<string, OpenApiSchema> PatternProperties { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MaxProperties { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual int? MinProperties { get; set; }

        /// <summary>
        /// Indicates if the schema can contain properties other than those defined by the properties map.
        /// </summary>
        public virtual bool AdditionalPropertiesAllowed { get; set; } = true;

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// Value can be boolean or object. Inline or referenced schema
        /// MUST be of a Schema Object and not a standard JSON Schema.
        /// </summary>
        public virtual OpenApiSchema AdditionalProperties { get; set; }

        /// <summary>
        /// Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.
        /// </summary>
        public virtual OpenApiDiscriminator Discriminator { get; set; }

        /// <summary>
        /// A free-form property to include an example of an instance for this schema.
        /// To represent examples that cannot be naturally represented in JSON or YAML,
        /// a string value can be used to contain the example with escaping where necessary.
        /// </summary>
        public virtual JsonNode Example { get; set; }

        /// <summary>
        /// A free-form property to include examples of an instance for this schema. 
        /// To represent examples that cannot be naturally represented in JSON or YAML, 
        /// a list of values can be used to contain the examples with escaping where necessary.
        /// </summary>
        public virtual IList<JsonNode> Examples { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual IList<JsonNode> Enum { get; set; } = new List<JsonNode>();

        /// <summary>
        /// Allows sending a null value for the defined schema. Default value is false.
        /// </summary>
        public virtual bool Nullable { get; set; }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        public virtual bool UnevaluatedProperties { get; set;}

        /// <summary>
        /// Additional external documentation for this schema.
        /// </summary>
        public virtual OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// Specifies that a schema is deprecated and SHOULD be transitioned out of usage.
        /// Default value is false.
        /// </summary>
        public virtual bool Deprecated { get; set; }

        /// <summary>
        /// This MAY be used only on properties schemas. It has no effect on root schemas.
        /// Adds additional metadata to describe the XML representation of this property.
        /// </summary>
        public virtual OpenApiXml Xml { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// This object stores any unrecognized keywords found in the schema.
        /// </summary>
        public virtual IDictionary<string, JsonNode> UnrecognizedKeywords { get; set; } = new Dictionary<string, JsonNode>();

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public virtual bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public virtual OpenApiReference Reference { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object> Annotations { get; set; }

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
            Const = schema?.Const ?? Const;
            Schema = schema?.Schema ?? Schema;
            Comment = schema?.Comment ?? Comment;
            Vocabulary = schema?.Vocabulary != null ? new Dictionary<string, bool>(schema.Vocabulary) : null;
            DynamicAnchor = schema?.DynamicAnchor ?? DynamicAnchor;
            DynamicRef = schema?.DynamicRef ?? DynamicRef;
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
            Default = schema?.Default != null ? JsonNodeCloneHelper.Clone(schema?.Default) : null;
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
            PatternProperties = schema?.PatternProperties != null ? new Dictionary<string, OpenApiSchema>(schema.PatternProperties) : null;
            MaxProperties = schema?.MaxProperties ?? MaxProperties;
            MinProperties = schema?.MinProperties ?? MinProperties;
            AdditionalPropertiesAllowed = schema?.AdditionalPropertiesAllowed ?? AdditionalPropertiesAllowed;
            AdditionalProperties = schema?.AdditionalProperties != null ? new(schema?.AdditionalProperties) : null;
            Discriminator = schema?.Discriminator != null ? new(schema?.Discriminator) : null; 
            Example = schema?.Example != null ? JsonNodeCloneHelper.Clone(schema?.Example) : null;
            Examples = schema?.Examples != null ? new List<JsonNode>(schema.Examples) : null;
            Enum = schema?.Enum != null ? new List<JsonNode>(schema.Enum) : null;
            Nullable = schema?.Nullable ?? Nullable;
            ExternalDocs = schema?.ExternalDocs != null ? new(schema?.ExternalDocs) : null;
            Deprecated = schema?.Deprecated ?? Deprecated;
            Xml = schema?.Xml != null ? new(schema?.Xml) : null;
            Extensions = schema?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(schema.Extensions) : null;
            UnresolvedReference = schema?.UnresolvedReference ?? UnresolvedReference;
            Reference = schema?.Reference != null ? new(schema?.Reference) : null;
            Annotations = schema?.Annotations != null ? new Dictionary<string, object>(schema?.Annotations) : null;
            UnrecognizedKeywords = schema?.UnrecognizedKeywords != null ? new Dictionary<string, JsonNode>(schema?.UnrecognizedKeywords) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

/// <inheritdoc/>

        public void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            writer.WriteStartObject();

            if (version == OpenApiSpecVersion.OpenApi3_1)
            {
                WriteJsonSchemaKeywords(writer);
            }

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, MinProperties);

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (nodeWriter, s) => nodeWriter.WriteAny(s));

            // type
            SerializeTypeProperty(Type, writer, version);

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, callback);

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, AnyOf, callback);

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, OneOf, callback);

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, Not, callback);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, callback);

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, callback);

            // additionalProperties
            if (AdditionalPropertiesAllowed)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    AdditionalProperties,
                    callback);
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // format
            writer.WriteProperty(OpenApiConstants.Format, Format);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // nullable
            if (version is OpenApiSpecVersion.OpenApi3_0)
            {
                writer.WriteProperty(OpenApiConstants.Nullable, Nullable, false);
            }

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, Discriminator, callback);

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, ReadOnly, false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, WriteOnly, false);

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, (w, s) => s.SerializeAsV2(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, callback);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // extensions
            writer.WriteExtensions(Extensions, version);

            // Unrecognized keywords
            if (UnrecognizedKeywords.Any())
            {
                writer.WriteOptionalMap(OpenApiConstants.UnrecognizedKeywords, UnrecognizedKeywords, (w,s) => w.WriteAny(s));
            }

            writer.WriteEndObject();
        }

/// <inheritdoc/>

        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeAsV2(writer: writer, parentRequiredProperties: new HashSet<string>(), propertyName: null);
        }

        internal void WriteJsonSchemaKeywords(IOpenApiWriter writer)
        {
            writer.WriteProperty(OpenApiConstants.Id, Id);
            writer.WriteProperty(OpenApiConstants.DollarSchema, Schema);
            writer.WriteProperty(OpenApiConstants.Comment, Comment);
            writer.WriteProperty(OpenApiConstants.Const, Const);
            writer.WriteOptionalMap(OpenApiConstants.Vocabulary, Vocabulary, (w, s) => w.WriteValue(s));
            writer.WriteOptionalMap(OpenApiConstants.Defs, Definitions, (w, s) => s.SerializeAsV31(w));
            writer.WriteProperty(OpenApiConstants.DynamicRef, DynamicRef);
            writer.WriteProperty(OpenApiConstants.DynamicAnchor, DynamicAnchor);
            writer.WriteProperty(OpenApiConstants.V31ExclusiveMaximum, V31ExclusiveMaximum);
            writer.WriteProperty(OpenApiConstants.V31ExclusiveMinimum, V31ExclusiveMinimum);            
            writer.WriteProperty(OpenApiConstants.UnevaluatedProperties, UnevaluatedProperties, false);
            writer.WriteOptionalCollection(OpenApiConstants.Examples, Examples, (nodeWriter, s) => nodeWriter.WriteAny(s));
            writer.WriteOptionalMap(OpenApiConstants.PatternProperties, PatternProperties, (w, s) => s.SerializeAsV31(w));
        }

        internal void WriteAsItemsProperties(IOpenApiWriter writer)
        {
            // type
            writer.WriteProperty(OpenApiConstants.Type, Type.ToIdentifier());

            // format
            if (string.IsNullOrEmpty(Format))
            {
                Format = AllOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    AnyOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    OneOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format;
            }

            writer.WriteProperty(OpenApiConstants.Format, Format);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, (w, s) => s.SerializeAsV2(w));

            // collectionFormat
            // We need information from style in parameter to populate this.
            // The best effort we can make is to pull this information from the first parameter
            // that leverages this schema. However, that in itself may not be as simple
            // as the schema directly under parameter might be referencing one in the Components,
            // so we will need to do a full scan of the object before we can write the value for
            // this property. This is not supported yet, so we will skip this property at the moment.

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteAny(s));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0 and handles not marking the provided property
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal void SerializeAsV2(
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            parentRequiredProperties ??= new HashSet<string>();

            writer.WriteStartObject();

            // type
            SerializeTypeProperty(Type, writer, OpenApiSpecVersion.OpenApi2_0);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // format
            if (string.IsNullOrEmpty(Format))
            {
                Format = AllOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    AnyOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    OneOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format;
            }

            writer.WriteProperty(OpenApiConstants.Format, Format);

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, MinItems);

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, UniqueItems);

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, MaxProperties);

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, MinProperties);

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, s) => w.WriteAny(s));

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, (w, s) => s.SerializeAsV2(w));

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, (w, s) => s.SerializeAsV2(w));

            // If there isn't already an allOf, and the schema contains a oneOf or anyOf write an allOf with the first
            // schema in the list as an attempt to guess at a graceful downgrade situation.
            if (AllOf == null || AllOf.Count == 0)
            {
                // anyOf (Not Supported in V2)  - Write the first schema only as an allOf.
                writer.WriteOptionalCollection(OpenApiConstants.AllOf, AnyOf?.Take(1), (w, s) => s.SerializeAsV2(w));

                if (AnyOf == null || AnyOf.Count == 0)
                {
                    // oneOf (Not Supported in V2) - Write the first schema only as an allOf.
                    writer.WriteOptionalCollection(OpenApiConstants.AllOf, OneOf?.Take(1), (w, s) => s.SerializeAsV2(w));
                }
            }

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, (w, key, s) =>
                s.SerializeAsV2(w, Required, key));

            // additionalProperties
            if (AdditionalPropertiesAllowed)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    AdditionalProperties,
                    (w, s) => s.SerializeAsV2(w));
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }

            // discriminator
            writer.WriteProperty(OpenApiConstants.Discriminator, Discriminator?.PropertyName);

            // readOnly
            // In V2 schema if a property is part of required properties of parent schema,
            // it cannot be marked as readonly.
            if (!parentRequiredProperties.Contains(propertyName))
            {
                writer.WriteProperty(name: OpenApiConstants.ReadOnly, value: ReadOnly, defaultValue: false);
            }

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, (w, s) => s.SerializeAsV2(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, s) => s.SerializeAsV2(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        private void SerializeTypeProperty(JsonSchemaType? type, IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            if (type is null)
            {
                return;
            }
            if (!HasMultipleTypes(type.Value))
            {
                // check whether nullable is true for upcasting purposes
                if (version is OpenApiSpecVersion.OpenApi3_1 && (Nullable || Extensions.ContainsKey(OpenApiConstants.NullableExtension)))
                {
                    UpCastSchemaTypeToV31(type, writer);
                }
                else
                {
                    writer.WriteProperty(OpenApiConstants.Type, type.Value.ToIdentifier());
                }
            }
            else
            {
                // type
                if (version is OpenApiSpecVersion.OpenApi2_0 || version is OpenApiSpecVersion.OpenApi3_0)
                {
                    DowncastTypeArrayToV2OrV3(type.Value, writer, version);
                }
                else
                {
                    var list = (from JsonSchemaType flag in jsonSchemaTypeValues
                                where type.Value.HasFlag(flag)
                                select flag).ToList();
                    writer.WriteOptionalCollection(OpenApiConstants.Type, list, (w, s) => w.WriteValue(s.ToIdentifier()));
                }
            } 
        }

        private static bool IsPowerOfTwo(int x)
        {
            return x != 0 && (x & (x - 1)) == 0;
        }

        private static bool HasMultipleTypes(JsonSchemaType schemaType)
        {
            var schemaTypeNumeric = (int)schemaType;
            return !IsPowerOfTwo(schemaTypeNumeric) && // Boolean, Integer, Number, String, Array, Object
                    schemaTypeNumeric != (int)JsonSchemaType.Null;
        }

        private void UpCastSchemaTypeToV31(JsonSchemaType? type, IOpenApiWriter writer)
        {
            // create a new array and insert the type and "null" as values
            Type = type | JsonSchemaType.Null;
            var list = (from JsonSchemaType? flag in jsonSchemaTypeValues// Check if the flag is set in 'type' using a bitwise AND operation
                        where Type.Value.HasFlag(flag)
                        select flag.ToIdentifier()).ToList();
            writer.WriteOptionalCollection(OpenApiConstants.Type, list, (w, s) => w.WriteValue(s));
        }

        private static readonly Array jsonSchemaTypeValues = System.Enum.GetValues(typeof(JsonSchemaType));

        private void DowncastTypeArrayToV2OrV3(JsonSchemaType schemaType, IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            /* If the array has one non-null value, emit Type as string
            * If the array has one null value, emit x-nullable as true
            * If the array has two values, one null and one non-null, emit Type as string and x-nullable as true
            * If the array has more than two values or two non-null values, do not emit type
            * */

            var nullableProp = version.Equals(OpenApiSpecVersion.OpenApi2_0)
                ? OpenApiConstants.NullableExtension
                : OpenApiConstants.Nullable;

            if (!HasMultipleTypes(schemaType ^ JsonSchemaType.Null) && (schemaType & JsonSchemaType.Null) == JsonSchemaType.Null) // checks for two values and one is null
            {
                foreach (JsonSchemaType? flag in jsonSchemaTypeValues)
                {
                    // Skip if the flag is not set or if it's the Null flag
                    if (schemaType.HasFlag(flag) && flag != JsonSchemaType.Null)
                    {
                        // Write the non-null flag value to the writer
                        writer.WriteProperty(OpenApiConstants.Type, flag.ToIdentifier());
                    }
                }
                if (!Nullable)
                {
                    writer.WriteProperty(nullableProp, true);
                }
            }
            else if (!HasMultipleTypes(schemaType))
            {
                if (schemaType is JsonSchemaType.Null)
                {
                    writer.WriteProperty(nullableProp, true);
                }
                else
                {
                    writer.WriteProperty(OpenApiConstants.Type, schemaType.ToIdentifier());
                }
            }
        }
    }
}
