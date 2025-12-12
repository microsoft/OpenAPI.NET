// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The Schema Object allows the definition of input and output data types.
    /// </summary>
    public class OpenApiSchema : IOpenApiExtensible, IOpenApiSchema, IMetadataContainer
    {
        /// <inheritdoc />
        public string? Title { get; set; }

        /// <inheritdoc />
        public Uri? Schema { get; set; }

        /// <inheritdoc />
        public string? Id { get; set; }

        /// <inheritdoc />
        public string? Comment { get; set; }

        /// <inheritdoc />
        public IDictionary<string, bool>? Vocabulary { get; set; }

        /// <inheritdoc />
        public string? DynamicRef { get; set; }

        /// <inheritdoc />
        public string? DynamicAnchor { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IOpenApiSchema>? Definitions { get; set; }

        private string? _exclusiveMaximum;
        /// <inheritdoc />
        public string? ExclusiveMaximum
        {
            get
            {
                if (!string.IsNullOrEmpty(_exclusiveMaximum))
                {
                    return _exclusiveMaximum;
                }
                if (IsExclusiveMaximum == true && !string.IsNullOrEmpty(_maximum))
                {
                    return _maximum;
                }
                return null;
            }
            set
            {
                _exclusiveMaximum = value;
                IsExclusiveMaximum = value != null;
            }
        }

        /// <summary>
        /// Compatibility property for OpenAPI 3.0 or earlier serialization of the exclusive maximum value.
        /// </summary>
        /// DO NOT CHANGE THE VISIBILITY OF THIS PROPERTY TO PUBLIC
        internal bool? IsExclusiveMaximum { get; set; }

        private string? _exclusiveMinimum;
        /// <inheritdoc />
        public string? ExclusiveMinimum
        {
            get
            {
                if (!string.IsNullOrEmpty(_exclusiveMinimum))
                {
                    return _exclusiveMinimum;
                }
                if (IsExclusiveMinimum == true && !string.IsNullOrEmpty(_minimum))
                {
                    return _minimum;
                }
                return null;
            }
            set
            {
                _exclusiveMinimum = value;
                IsExclusiveMinimum = value != null;
            }
        }

        /// <summary>
        /// Compatibility property for OpenAPI 3.0 or earlier serialization of the exclusive minimum value.
        /// </summary>
        /// DO NOT CHANGE THE VISIBILITY OF THIS PROPERTY TO PUBLIC
        internal bool? IsExclusiveMinimum { get; set; }

        /// <inheritdoc />
        public JsonSchemaType? Type { get; set; }

        // x-nullable is filtered out by deserializers, but keep the check here in case it gets added from user code.
        private bool IsNullable =>
            (Type.HasValue && Type.Value.HasFlag(JsonSchemaType.Null)) ||
            Extensions is not null &&
            Extensions.TryGetValue(OpenApiConstants.NullableExtension, out var nullExtRawValue) &&
            nullExtRawValue is JsonNodeExtension { Node: JsonNode jsonNode } &&
            jsonNode.GetValueKind() is JsonValueKind.True;

        /// <inheritdoc />
        public string? Const { get; set; }

        /// <inheritdoc />
        public string? Format { get; set; }

        /// <inheritdoc />
        public string? Description { get; set; }

        private string? _maximum;
        /// <inheritdoc />
        public string? Maximum
        {
            get
            {
                if (IsExclusiveMaximum == true)
                {
                    return null;
                }
                return _maximum;
            }
            set
            {
                _maximum = value;
            }
        }

        private string? _minimum;

        /// <inheritdoc />
        public string? Minimum
        {
            get
            {
                if (IsExclusiveMinimum == true)
                {
                    return null;
                }
                return _minimum;
            }
            set
            {
                _minimum = value;
            }
        }

        /// <inheritdoc />
        public int? MaxLength { get; set; }

        /// <inheritdoc />
        public int? MinLength { get; set; }

        /// <inheritdoc />
        public string? Pattern { get; set; }

        /// <inheritdoc />
        public decimal? MultipleOf { get; set; }

        /// <inheritdoc />
        public JsonNode? Default { get; set; }

        /// <inheritdoc />
        public bool ReadOnly { get; set; }

        /// <inheritdoc />
        public bool WriteOnly { get; set; }

        /// <inheritdoc />
        public IList<IOpenApiSchema>? AllOf { get; set; }

        /// <inheritdoc />
        public IList<IOpenApiSchema>? OneOf { get; set; }

        /// <inheritdoc />
        public IList<IOpenApiSchema>? AnyOf { get; set; }

        /// <inheritdoc />
        public IOpenApiSchema? Not { get; set; }

        /// <inheritdoc />
        public ISet<string>? Required { get; set; }

        /// <inheritdoc />
        public IOpenApiSchema? Items { get; set; }

        /// <inheritdoc />
        public int? MaxItems { get; set; }

        /// <inheritdoc />
        public int? MinItems { get; set; }

        /// <inheritdoc />
        public bool? UniqueItems { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IOpenApiSchema>? Properties { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IOpenApiSchema>? PatternProperties { get; set; }

        /// <inheritdoc />
        public int? MaxProperties { get; set; }

        /// <inheritdoc />
        public int? MinProperties { get; set; }

        /// <inheritdoc />
        public bool AdditionalPropertiesAllowed { get; set; } = true;

        /// <inheritdoc />
        public IOpenApiSchema? AdditionalProperties { get; set; }

        /// <inheritdoc />
        public OpenApiDiscriminator? Discriminator { get; set; }

        /// <inheritdoc />
        public JsonNode? Example { get; set; }

        /// <inheritdoc />
        public IList<JsonNode>? Examples { get; set; }

        /// <inheritdoc />
        public IList<JsonNode>? Enum { get; set; }

        /// <inheritdoc />
        public bool UnevaluatedProperties { get; set; }

        /// <inheritdoc />
        public OpenApiExternalDocs? ExternalDocs { get; set; }

        /// <inheritdoc />
        public bool Deprecated { get; set; }

        /// <inheritdoc />
        public OpenApiXml? Xml { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <inheritdoc />
        public IDictionary<string, JsonNode>? UnrecognizedKeywords { get; set; }

        /// <inheritdoc />
        public IDictionary<string, object>? Metadata { get; set; }

        /// <inheritdoc />
        public IDictionary<string, HashSet<string>>? DependentRequired { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiSchema() { }

        /// <summary>
        /// Initializes a copy of <see cref="IOpenApiSchema"/> object
        /// </summary>
        /// <param name="schema">The schema object to copy from.</param>
        internal OpenApiSchema(IOpenApiSchema schema)
        {
            Utils.CheckArgumentNull(schema);
            Title = schema.Title ?? Title;
            Id = schema.Id ?? Id;
            Const = schema.Const ?? Const;
            Schema = schema.Schema ?? Schema;
            Comment = schema.Comment ?? Comment;
            Vocabulary = schema.Vocabulary != null ? new Dictionary<string, bool>(schema.Vocabulary) : null;
            DynamicAnchor = schema.DynamicAnchor ?? DynamicAnchor;
            DynamicRef = schema.DynamicRef ?? DynamicRef;
            Definitions = schema.Definitions != null ? new Dictionary<string, IOpenApiSchema>(schema.Definitions) : null;
            UnevaluatedProperties = schema.UnevaluatedProperties;
            ExclusiveMaximum = schema.ExclusiveMaximum ?? ExclusiveMaximum;
            ExclusiveMinimum = schema.ExclusiveMinimum ?? ExclusiveMinimum;
            if (schema is OpenApiSchema eMSchema)
            {
                IsExclusiveMaximum = eMSchema.IsExclusiveMaximum;
                IsExclusiveMinimum = eMSchema.IsExclusiveMinimum;
            }
            Type = schema.Type ?? Type;
            Format = schema.Format ?? Format;
            Description = schema.Description ?? Description;
            Maximum = schema.Maximum ?? Maximum;
            Minimum = schema.Minimum ?? Minimum;
            MaxLength = schema.MaxLength ?? MaxLength;
            MinLength = schema.MinLength ?? MinLength;
            Pattern = schema.Pattern ?? Pattern;
            MultipleOf = schema.MultipleOf ?? MultipleOf;
            Default = schema.Default != null ? JsonNodeCloneHelper.Clone(schema.Default) : null;
            ReadOnly = schema.ReadOnly;
            WriteOnly = schema.WriteOnly;
            AllOf = schema.AllOf != null ? [.. schema.AllOf] : null;
            OneOf = schema.OneOf != null ? [.. schema.OneOf] : null;
            AnyOf = schema.AnyOf != null ? [.. schema.AnyOf] : null;
            Not = schema.Not?.CreateShallowCopy();
            Required = schema.Required != null ? new HashSet<string>(schema.Required) : null;
            Items = schema.Items?.CreateShallowCopy();
            MaxItems = schema.MaxItems ?? MaxItems;
            MinItems = schema.MinItems ?? MinItems;
            UniqueItems = schema.UniqueItems ?? UniqueItems;
            Properties = schema.Properties != null ? new Dictionary<string, IOpenApiSchema>(schema.Properties) : null;
            PatternProperties = schema.PatternProperties != null ? new Dictionary<string, IOpenApiSchema>(schema.PatternProperties) : null;
            MaxProperties = schema.MaxProperties ?? MaxProperties;
            MinProperties = schema.MinProperties ?? MinProperties;
            AdditionalPropertiesAllowed = schema.AdditionalPropertiesAllowed;
            AdditionalProperties = schema.AdditionalProperties?.CreateShallowCopy();
            Discriminator = schema.Discriminator != null ? new(schema.Discriminator) : null;
            Example = schema.Example != null ? JsonNodeCloneHelper.Clone(schema.Example) : null;
            Examples = schema.Examples != null ? [.. schema.Examples] : null;
            Enum = schema.Enum != null ? [.. schema.Enum] : null;
            ExternalDocs = schema.ExternalDocs != null ? new(schema.ExternalDocs) : null;
            Deprecated = schema.Deprecated;
            Xml = schema.Xml != null ? new(schema.Xml) : null;
            Extensions = schema.Extensions != null ? new Dictionary<string, IOpenApiExtension>(schema.Extensions) : null;
            Metadata = schema is IMetadataContainer { Metadata: not null } mContainer ? new Dictionary<string, object>(mContainer.Metadata) : null;
            UnrecognizedKeywords = schema.UnrecognizedKeywords != null ? new Dictionary<string, JsonNode>(schema.UnrecognizedKeywords) : null;
            DependentRequired = schema.DependentRequired != null ? new Dictionary<string, HashSet<string>>(schema.DependentRequired) : null;
        }

        /// <inheritdoc />
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <inheritdoc />
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        private static void SerializeBounds(IOpenApiWriter writer, OpenApiSpecVersion version, string propertyName, string exclusivePropertyName, string isExclusivePropertyName, string? value, string? exclusiveValue, bool? isExclusiveValue)
        {
            if (version >= OpenApiSpecVersion.OpenApi3_1)
            {
                if (!string.IsNullOrEmpty(exclusiveValue) && exclusiveValue is not null)
                {
                    // was explicitly set in the document or object model
                    writer.WritePropertyName(exclusivePropertyName);
                    writer.WriteRaw(exclusiveValue);
                }
                else if (isExclusiveValue == true && !string.IsNullOrEmpty(value) && value is not null)
                {
                    // came from parsing an old document
                    writer.WritePropertyName(exclusivePropertyName);
                    writer.WriteRaw(value);
                }
                else if (!string.IsNullOrEmpty(value) && value is not null)
                {
                    // was explicitly set in the document or object model
                    writer.WritePropertyName(propertyName);
                    writer.WriteRaw(value);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(exclusiveValue) && exclusiveValue is not null)
                {
                    // was explicitly set in a new document being downcast or object model
                    writer.WritePropertyName(propertyName);
                    writer.WriteRaw(exclusiveValue);
                    writer.WriteProperty(isExclusivePropertyName, true);
                }
                else if (!string.IsNullOrEmpty(value) && value is not null)
                {
                    // came from parsing an old document, we're just mirroring the information
                    writer.WritePropertyName(propertyName);
                    writer.WriteRaw(value);
                    if (isExclusiveValue.HasValue)
                        writer.WriteProperty(isExclusivePropertyName, isExclusiveValue.Value);
                }
            }
        }

        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
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
            // exclusiveMaximum
            SerializeBounds(writer, version, OpenApiConstants.Maximum, OpenApiConstants.ExclusiveMaximum, OpenApiConstants.V31ExclusiveMaximum, Maximum, ExclusiveMaximum, IsExclusiveMaximum);

            // minimum
            // exclusiveMinimum
            SerializeBounds(writer, version, OpenApiConstants.Minimum, OpenApiConstants.ExclusiveMinimum, OpenApiConstants.V31ExclusiveMinimum, Minimum, ExclusiveMinimum, IsExclusiveMinimum);

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
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) =>
            {
                if (!string.IsNullOrEmpty(s) && s is not null)
                {
                    w.WriteValue(s);
                }
            });

            // enum
            var enumValue = Enum is not { Count: > 0 }
                && !string.IsNullOrEmpty(Const)
                && version < OpenApiSpecVersion.OpenApi3_1
                ? new List<JsonNode> { JsonValue.Create(Const)! }
                : Enum;
            writer.WriteOptionalCollection(OpenApiConstants.Enum, enumValue, (nodeWriter, s) => nodeWriter.WriteAny(s));

            // Handle oneOf/anyOf with null type for v3.0 downcast
            IList<IOpenApiSchema>? effectiveOneOf = OneOf;
            IList<IOpenApiSchema>? effectiveAnyOf = AnyOf;
            bool hasNullInComposition = false;
            JsonSchemaType? inferredType = null;

            if (version == OpenApiSpecVersion.OpenApi3_0)
            {
                (effectiveOneOf, var inferredOneOf, var nullInOneOf) = ProcessCompositionForNull(OneOf);
                hasNullInComposition |= nullInOneOf;
                inferredType = inferredOneOf ?? inferredType;
                (effectiveAnyOf, var inferredAnyOf, var nullInAnyOf) = ProcessCompositionForNull(AnyOf);
                hasNullInComposition |= nullInAnyOf;
                inferredType = inferredAnyOf ?? inferredType;
            }

            // type
            SerializeTypeProperty(writer, version, inferredType);

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, callback);

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, effectiveAnyOf, callback);

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, effectiveOneOf, callback);

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, Not, callback);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, Items, callback);

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, callback);

            // additionalProperties
            if (AdditionalProperties is not null && version >= OpenApiSpecVersion.OpenApi3_0)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    AdditionalProperties,
                    callback);
            }
            // true is the default in earlier versions 3, no need to write it out
            // boolean value is only supported for version 3 and earlier (version 2 is implemented in the other serialize method, the condition is a failsafe)
            else if (!AdditionalPropertiesAllowed && version <= OpenApiSpecVersion.OpenApi3_0)
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }
            // not having anything is the same as having it set to true (v2/v3) or an empty schema (v3.1+)

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // format
            writer.WriteProperty(OpenApiConstants.Format, Format);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // nullable
            if (version == OpenApiSpecVersion.OpenApi3_0)
            {
                SerializeNullable(writer, version, hasNullInComposition);
            }

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, Discriminator, callback);

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, ReadOnly, false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, WriteOnly, false);

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, callback);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, callback);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // extensions
            writer.WriteExtensions(Extensions, version);

            // Unrecognized keywords
            if (UnrecognizedKeywords is not null && UnrecognizedKeywords.Any())
            {
                writer.WriteOptionalMap(OpenApiConstants.UnrecognizedKeywords, UnrecognizedKeywords, (w, s) => w.WriteAny(s));
            }

            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeAsV2(writer: writer, parentRequiredProperties: new HashSet<string>(), propertyName: null);
        }

        internal void WriteJsonSchemaKeywords(IOpenApiWriter writer)
        {
            writer.WriteProperty(OpenApiConstants.Id, Id);
            writer.WriteProperty(OpenApiConstants.DollarSchema, Schema?.ToString());
            writer.WriteProperty(OpenApiConstants.Comment, Comment);
            writer.WriteProperty(OpenApiConstants.Const, Const);
            writer.WriteOptionalMap(OpenApiConstants.Vocabulary, Vocabulary, (w, s) => w.WriteValue(s));
            writer.WriteOptionalMap(OpenApiConstants.Defs, Definitions, (w, s) => s.SerializeAsV31(w));
            writer.WriteProperty(OpenApiConstants.DynamicRef, DynamicRef);
            writer.WriteProperty(OpenApiConstants.DynamicAnchor, DynamicAnchor);
            writer.WriteProperty(OpenApiConstants.UnevaluatedProperties, UnevaluatedProperties, false);
            writer.WriteOptionalCollection(OpenApiConstants.Examples, Examples, (nodeWriter, s) => nodeWriter.WriteAny(s));
            writer.WriteOptionalMap(OpenApiConstants.PatternProperties, PatternProperties, (w, s) => s.SerializeAsV31(w));
            writer.WriteOptionalMap(OpenApiConstants.DependentRequired, DependentRequired, (w, s) => w.WriteValue(s));
        }

        internal void WriteAsItemsProperties(IOpenApiWriter writer)
        {
            // type
            writer.WriteProperty(OpenApiConstants.Type, (Type & ~JsonSchemaType.Null)?.ToFirstIdentifier());

            // format
            WriteFormatProperty(writer);

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
            // exclusiveMaximum
            SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Maximum, OpenApiConstants.ExclusiveMaximum, OpenApiConstants.V31ExclusiveMaximum, Maximum, ExclusiveMaximum, IsExclusiveMaximum);

            // minimum
            // exclusiveMinimum
            SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Minimum, OpenApiConstants.ExclusiveMinimum, OpenApiConstants.V31ExclusiveMinimum, Minimum, ExclusiveMinimum, IsExclusiveMinimum);

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

        private void WriteFormatProperty(IOpenApiWriter writer)
        {
            var formatToWrite = Format;
            if (string.IsNullOrEmpty(formatToWrite))
            {
                formatToWrite = AllOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    AnyOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format ??
                    OneOf?.FirstOrDefault(static x => !string.IsNullOrEmpty(x.Format))?.Format;
            }

            writer.WriteProperty(OpenApiConstants.Format, formatToWrite);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0 and handles not marking the provided property
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        private void SerializeAsV2(
            IOpenApiWriter writer,
            ISet<string>? parentRequiredProperties,
            string? propertyName)
        {
            parentRequiredProperties ??= new HashSet<string>();

            writer.WriteStartObject();

            // type
            SerializeTypeProperty(writer, OpenApiSpecVersion.OpenApi2_0);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // format
            WriteFormatProperty(writer);

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);

            // maximum
            // exclusiveMaximum
            SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Maximum, OpenApiConstants.ExclusiveMaximum, OpenApiConstants.V31ExclusiveMaximum, Maximum, ExclusiveMaximum, IsExclusiveMaximum);

            // minimum
            // exclusiveMinimum
            SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Minimum, OpenApiConstants.ExclusiveMinimum, OpenApiConstants.V31ExclusiveMinimum, Minimum, ExclusiveMinimum, IsExclusiveMinimum);

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
            writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) =>
            {
                if (!string.IsNullOrEmpty(s) && s is not null)
                {
                    w.WriteValue(s);
                }
            });

            // enum
            var enumValue = Enum is not { Count: > 0 } && !string.IsNullOrEmpty(Const)
                ? new List<JsonNode> { JsonValue.Create(Const)! }
                : Enum;
            writer.WriteOptionalCollection(OpenApiConstants.Enum, enumValue, (nodeWriter, s) => nodeWriter.WriteAny(s));

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
            {
                if (s is OpenApiSchema oais)
                    oais.SerializeAsV2(w, Required, key);
                else
                    s.SerializeAsV2(w);
            });

            // additionalProperties
            // a schema cannot be serialized in v2
            // true is the default, no need to write it out
            if (!AdditionalPropertiesAllowed)
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed);
            }

            // discriminator
            writer.WriteProperty(OpenApiConstants.Discriminator, Discriminator?.PropertyName);

            // readOnly
            // In V2 schema if a property is part of required properties of parent schema,
            // it cannot be marked as readonly.
            if (!parentRequiredProperties.Contains(propertyName ?? string.Empty))
            {
                writer.WriteProperty(name: OpenApiConstants.ReadOnly, value: ReadOnly, defaultValue: false);
            }

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, (w, s) => s.SerializeAsV2(w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, s) => s.SerializeAsV2(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // x-nullable extension
            SerializeNullable(writer, OpenApiSpecVersion.OpenApi2_0);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        private void SerializeTypeProperty(IOpenApiWriter writer, OpenApiSpecVersion version, JsonSchemaType? inferredType = null)
        {
            // Use inferred type from oneOf/anyOf if provided and original type is not set
            var typeToUse = inferredType ?? Type;

            if (typeToUse is null)
            {
                return;
            }

            var unifiedType = IsNullable ? typeToUse.Value | JsonSchemaType.Null : typeToUse.Value;
            var typeWithoutNull = unifiedType & ~JsonSchemaType.Null;

            switch (version)
            {
                case OpenApiSpecVersion.OpenApi2_0 or OpenApiSpecVersion.OpenApi3_0:
                    if (typeWithoutNull != 0 && !HasMultipleTypes(typeWithoutNull))
                    {
                        writer.WriteProperty(OpenApiConstants.Type, typeWithoutNull.ToFirstIdentifier());
                    }
                    break;
                default:
                    WriteUnifiedSchemaType(unifiedType, writer);
                    break;
            }
        }

        private static bool IsPowerOfTwo(int x)
        {
            return x != 0 && (x & (x - 1)) == 0;
        }

        private static bool HasMultipleTypes(JsonSchemaType schemaType)
        {
            var schemaTypeNumeric = (int)schemaType;
            return !IsPowerOfTwo(schemaTypeNumeric);
        }

        private static void WriteUnifiedSchemaType(JsonSchemaType type, IOpenApiWriter writer)
        {
            var array = (from JsonSchemaType flag in jsonSchemaTypeValues
                         where type.HasFlag(flag)
                         select flag.ToFirstIdentifier()).ToArray();
            if (array.Length > 1)
            {
                writer.WriteOptionalCollection(OpenApiConstants.Type, array, (w, s) =>
                {
                    if (!string.IsNullOrEmpty(s) && s is not null)
                    {
                        w.WriteValue(s);
                    }
                });
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.Type, array[0]);
            }
        }

        private void SerializeNullable(IOpenApiWriter writer, OpenApiSpecVersion version, bool hasNullInComposition = false)
        {
            if (IsNullable || hasNullInComposition)
            {
                switch (version)
                {
                    case OpenApiSpecVersion.OpenApi2_0:
                        writer.WriteProperty(OpenApiConstants.NullableExtension, true);
                        break;
                    case OpenApiSpecVersion.OpenApi3_0:
                        writer.WriteProperty(OpenApiConstants.Nullable, true);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a composition (oneOf or anyOf) for null types, filtering out null schemas and inferring common type.
        /// </summary>
        /// <param name="composition">The list of schemas in the composition.</param>
        /// <returns>A tuple with the effective list, inferred type, and whether null is present in composition.</returns>
        private static (IList<IOpenApiSchema>? effective, JsonSchemaType? inferredType, bool hasNullInComposition)
            ProcessCompositionForNull(IList<IOpenApiSchema>? composition)
        {
            if (composition is null || !composition.Any(s => s.Type is JsonSchemaType.Null))
            {
                // Nothing to patch
                return (composition, null, false);
            }

            var nonNullSchemas = composition
                .Where(s => s.Type is null or not JsonSchemaType.Null)
                .ToList();

            if (nonNullSchemas.Count > 0)
            {
                JsonSchemaType commonType = 0;

                foreach (var schema in nonNullSchemas)
                {
                    commonType |= schema.Type.GetValueOrDefault() & ~JsonSchemaType.Null;
                }

                if (System.Enum.IsDefined(commonType))
                {
                    // Single common type
                    return (nonNullSchemas, commonType, true);
                }
                else
                {
                    return (nonNullSchemas, null, true);
                }

            }
            else
            {
                return (null, null, true);
            }
        }

#if NET5_0_OR_GREATER
        private static readonly Array jsonSchemaTypeValues = System.Enum.GetValues<JsonSchemaType>();
#else
        private static readonly Array jsonSchemaTypeValues = System.Enum.GetValues(typeof(JsonSchemaType));
#endif

        /// <inheritdoc/>
        public IOpenApiSchema CreateShallowCopy()
        {
            return new OpenApiSchema(this);
        }
    }
}
