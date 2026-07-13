// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Schema reference information that includes metadata annotations from JSON Schema 2020-12.
/// This class extends OpenApiReference to provide schema-specific metadata override capabilities.
/// </summary>
public class JsonSchemaReference : OpenApiReferenceWithDescription
{
    /// <summary>
    /// A default value which by default SHOULD override that of the referenced component.
    /// If the referenced object-type does not allow a default field, then this field has no effect.
    /// You must use the <see cref="JsonNullSentinel.IsJsonNullSentinel(JsonNode?)"/> method to check whether Default was assigned a null value in the document.
    /// Assign <see cref="JsonNullSentinel.JsonNull"/> to use get null as a serialized value.
    /// </summary>
    public JsonNode? Default { get; set; }

    /// <summary>
    /// A title which by default SHOULD override that of the referenced component.
    /// If the referenced object-type does not allow a title field, then this field has no effect.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Indicates whether the referenced component is deprecated.
    /// If the referenced object-type does not allow a deprecated field, then this field has no effect.
    /// </summary>
    public bool? Deprecated { get; set; }

    /// <summary>
    /// Indicates whether the referenced component is read-only.
    /// If the referenced object-type does not allow a readOnly field, then this field has no effect.
    /// </summary>
    public bool? ReadOnly { get; set; }

    /// <summary>
    /// Indicates whether the referenced component is write-only.
    /// If the referenced object-type does not allow a writeOnly field, then this field has no effect.
    /// </summary>
    public bool? WriteOnly { get; set; }

    /// <summary>
    /// Example values which by default SHOULD override those of the referenced component.
    /// If the referenced object-type does not allow examples, then this field has no effect.
    /// </summary>
    public IList<JsonNode>? Examples { get; set; }

    /// <summary>
    /// Extension data for this schema reference. Only allowed in OpenAPI 3.1 and later.
    /// Extensions are NOT written when serializing for OpenAPI 2.0 or 3.0.
    /// </summary>
    public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

    /// <summary>
    /// A <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-id-keyword">$id</see> which by default SHOULD override that of the referenced component.
    /// Named SchemaId to avoid collision with the inherited reference identifier (BaseOpenApiReference.Id).
    /// </summary>
    public string? SchemaId { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-schema-keyword">$schema</see> dialect URI which by default SHOULD override that of the referenced component.
    /// </summary>
    public Uri? Schema { get; set; }

    /// <summary>
    /// A <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-comments-with-comment">$comment</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-vocabulary-keyword">$vocabulary</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public IDictionary<string, bool>? Vocabulary { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-dynamicref-keyword">$dynamicRef</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? DynamicRef { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-dynamicanchor-keyword">$dynamicAnchor</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? DynamicAnchor { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-schema-re-use-with-defs">$defs</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? Definitions { get; set; }

    /// <summary>
    /// The <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-the-anchor-keyword">$anchor</see> which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? Anchor { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? ExclusiveMaximum { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? ExclusiveMinimum { get; set; }

    /// <summary>
    /// <see href="https://json-schema.org/draft/2020-12/json-schema-validation#name-type">Schema type</see> override. Named SchemaType to avoid collision with <see cref="BaseOpenApiReference.Type"/>.
    /// </summary>
    public JsonSchemaType? SchemaType { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? Const { get; set; } = OpenApiUnsetValues.UnsetString;

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? Maximum { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? Minimum { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public decimal? MultipleOf { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IList<IOpenApiSchema>? AllOf { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IList<IOpenApiSchema>? OneOf { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IList<IOpenApiSchema>? AnyOf { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? Not { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public ISet<string>? Required { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? Items { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MaxItems { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MinItems { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public bool? UniqueItems { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? Contains { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public uint? MaxContains { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public uint? MinContains { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? Properties { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? PatternProperties { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MaxProperties { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public int? MinProperties { get; set; }

    /// <summary>
    /// Indicates if the schema can contain properties other than those defined by the properties map.
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-additionalproperties">JSON Schema definition</see>.
    /// </summary>
    public bool? AdditionalPropertiesAllowed { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-additionalproperties">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? AdditionalProperties { get; set; }

    /// <summary>
    /// Adds support for polymorphism.
    /// Follow <see href="https://spec.openapis.org/oas/v3.1.1.html#discriminator-object">OpenAPI definition</see>.
    /// </summary>
    public OpenApiDiscriminator? Discriminator { get; set; }

    /// <summary>
    /// A free-form property to include an example of an instance for this schema.
    /// Follow <see href="https://spec.openapis.org/oas/v3.1.1.html#schema-object">OpenAPI Schema Object definition</see>.
    /// </summary>
    public JsonNode? Example { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation">JSON Schema definition</see>.
    /// </summary>
    public IList<JsonNode>? Enum { get; set; }

    /// <summary>
    /// Indicates whether unevaluated properties are allowed.
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-unevaluatedproperties">JSON Schema definition</see>.
    /// </summary>
    public bool? UnevaluatedProperties { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-unevaluatedproperties">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? UnevaluatedPropertiesSchema { get; set; }

    /// <summary>
    /// Additional external documentation for this schema.
    /// Follow <see href="https://spec.openapis.org/oas/v3.1.1.html#external-documentation-object">OpenAPI definition</see>.
    /// </summary>
    public OpenApiExternalDocs? ExternalDocs { get; set; }

    /// <summary>
    /// This MAY be used only on properties schemas.
    /// Follow <see href="https://spec.openapis.org/oas/v3.1.1.html#xml-object">OpenAPI definition</see>.
    /// </summary>
    public OpenApiXml? Xml { get; set; }

    /// <summary>
    /// This object stores any unrecognized keywords found in the schema.
    /// </summary>
    public IDictionary<string, JsonNode>? UnrecognizedKeywords { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation#section-6.5.4">JSON Schema definition</see>.
    /// </summary>
    public IDictionary<string, HashSet<string>>? DependentRequired { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation#name-contentencoding">JSON Schema definition</see>.
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation#name-contentmediatype">JSON Schema definition</see>.
    /// </summary>
    public string? ContentMediaType { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-validation#name-contentschema">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? ContentSchema { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-propertynames">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? PropertyNames { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-dependentschemas">JSON Schema definition</see>.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? DependentSchemas { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-if">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? If { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-then">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? Then { get; set; }

    /// <summary>
    /// Follow <see href="https://json-schema.org/draft/2020-12/json-schema-core#name-else">JSON Schema definition</see>.
    /// </summary>
    public IOpenApiSchema? Else { get; set; }

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public JsonSchemaReference() { }

    /// <summary>
    /// Initializes a copy instance of the <see cref="JsonSchemaReference"/> object
    /// </summary>
    public JsonSchemaReference(JsonSchemaReference reference) : base(reference)
    {
        Utils.CheckArgumentNull(reference);
        Default = reference.Default;
        Title = reference.Title;
        Deprecated = reference.Deprecated;
        ReadOnly = reference.ReadOnly;
        WriteOnly = reference.WriteOnly;
        Examples = reference.Examples;
        Extensions = reference.Extensions != null ? new Dictionary<string, IOpenApiExtension>(reference.Extensions) : null;
        SchemaId = reference.SchemaId;
        Schema = reference.Schema;
        Comment = reference.Comment;
        Vocabulary = reference.Vocabulary != null ? new Dictionary<string, bool>(reference.Vocabulary) : null;
        DynamicRef = reference.DynamicRef;
        DynamicAnchor = reference.DynamicAnchor;
        Definitions = reference.Definitions != null ? new Dictionary<string, IOpenApiSchema>(reference.Definitions) : null;
        Anchor = reference.Anchor;
        ExclusiveMaximum = reference.ExclusiveMaximum;
        ExclusiveMinimum = reference.ExclusiveMinimum;
        SchemaType = reference.SchemaType;
        Const = reference.Const;
        Format = reference.Format;
        Maximum = reference.Maximum;
        Minimum = reference.Minimum;
        MaxLength = reference.MaxLength;
        MinLength = reference.MinLength;
        Pattern = reference.Pattern;
        MultipleOf = reference.MultipleOf;
        AllOf = reference.AllOf != null ? [.. reference.AllOf] : null;
        OneOf = reference.OneOf != null ? [.. reference.OneOf] : null;
        AnyOf = reference.AnyOf != null ? [.. reference.AnyOf] : null;
        Not = reference.Not;
        Required = reference.Required != null ? new HashSet<string>(reference.Required) : null;
        Items = reference.Items;
        MaxItems = reference.MaxItems;
        MinItems = reference.MinItems;
        UniqueItems = reference.UniqueItems;
        Contains = reference.Contains;
        MaxContains = reference.MaxContains;
        MinContains = reference.MinContains;
        Properties = reference.Properties != null ? new Dictionary<string, IOpenApiSchema>(reference.Properties) : null;
        PatternProperties = reference.PatternProperties != null ? new Dictionary<string, IOpenApiSchema>(reference.PatternProperties) : null;
        MaxProperties = reference.MaxProperties;
        MinProperties = reference.MinProperties;
        AdditionalPropertiesAllowed = reference.AdditionalPropertiesAllowed;
        AdditionalProperties = reference.AdditionalProperties;
        Discriminator = reference.Discriminator;
        Example = reference.Example;
        Enum = reference.Enum != null ? [.. reference.Enum] : null;
        UnevaluatedProperties = reference.UnevaluatedProperties;
        UnevaluatedPropertiesSchema = reference.UnevaluatedPropertiesSchema;
        ExternalDocs = reference.ExternalDocs;
        Xml = reference.Xml;
        UnrecognizedKeywords = reference.UnrecognizedKeywords != null ? new Dictionary<string, JsonNode>(reference.UnrecognizedKeywords) : null;
        DependentRequired = reference.DependentRequired != null ? new Dictionary<string, HashSet<string>>(reference.DependentRequired) : null;
        ContentEncoding = reference.ContentEncoding;
        ContentMediaType = reference.ContentMediaType;
        ContentSchema = reference.ContentSchema;
        PropertyNames = reference.PropertyNames;
        DependentSchemas = reference.DependentSchemas != null ? new Dictionary<string, IOpenApiSchema>(reference.DependentSchemas) : null;
        If = reference.If;
        Then = reference.Then;
        Else = reference.Else;
    }

    /// <inheritdoc/>
    protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
    {
        SerializeAdditionalV3XProperties(writer, (w, e) => e.SerializeAsV31(w), base.SerializeAdditionalV31Properties);
    }
    /// <inheritdoc/>
    protected override void SerializeAdditionalV32Properties(IOpenApiWriter writer)
    {
        SerializeAdditionalV3XProperties(writer, (w, e) => e.SerializeAsV32(w), base.SerializeAdditionalV32Properties);
    }
    private void SerializeAdditionalV3XProperties(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> serializeCallback, Action<IOpenApiWriter> baseSerializer)
    {
        if (Type != ReferenceType.Schema) throw new InvalidOperationException(
            $"JsonSchemaReference can only be serialized for ReferenceType.Schema, but was {Type}.");

        baseSerializer(writer);

        // JSON Schema 2020-12 keyword siblings (preserved per OAS 3.1+ / JSON Schema 2020-12 semantics)
        writer.WriteProperty(OpenApiConstants.Id, SchemaId);
        writer.WriteProperty(OpenApiConstants.DollarSchema, Schema?.ToString());
        writer.WriteProperty(OpenApiConstants.Comment, Comment);
        writer.WriteOptionalMap(OpenApiConstants.Vocabulary, Vocabulary, (w, s) => w.WriteValue(s));
        writer.WriteOptionalMap(OpenApiConstants.Defs, Definitions, serializeCallback);
        writer.WriteProperty(OpenApiConstants.Anchor, Anchor);
        writer.WriteProperty(OpenApiConstants.DynamicRef, DynamicRef);
        writer.WriteProperty(OpenApiConstants.DynamicAnchor, DynamicAnchor);

        writer.WriteRequiredProperty(OpenApiConstants.Const, Const);
        WriteSchemaType(writer, OpenApiConstants.Type, SchemaType, allowMultipleTypes: true);
        writer.WriteProperty(OpenApiConstants.Format, Format);
        writer.WriteProperty(OpenApiConstants.MultipleOf, MultipleOf);
        WriteRawProperty(writer, OpenApiConstants.Maximum, Maximum);
        WriteRawProperty(writer, OpenApiConstants.ExclusiveMaximum, ExclusiveMaximum);
        WriteRawProperty(writer, OpenApiConstants.Minimum, Minimum);
        WriteRawProperty(writer, OpenApiConstants.ExclusiveMinimum, ExclusiveMinimum);
        writer.WriteProperty(OpenApiConstants.MaxLength, MaxLength);
        writer.WriteProperty(OpenApiConstants.MinLength, MinLength);
        writer.WriteProperty(OpenApiConstants.Pattern, Pattern);
        writer.WriteProperty(OpenApiConstants.MaxItems, MaxItems);
        writer.WriteProperty(OpenApiConstants.MinItems, MinItems);
        writer.WriteProperty(OpenApiConstants.UniqueItems, UniqueItems);
        writer.WriteProperty(OpenApiConstants.MaxProperties, MaxProperties);
        writer.WriteProperty(OpenApiConstants.MinProperties, MinProperties);
        writer.WriteOptionalCollection(OpenApiConstants.Required, Required, (w, s) =>
        {
            if (!string.IsNullOrEmpty(s))
            {
                w.WriteValue(s!);
            }
        });
        writer.WriteOptionalCollection(OpenApiConstants.Enum, Enum, (w, e) => w.WriteAny(e));
        writer.WriteOptionalCollection(OpenApiConstants.AllOf, AllOf, serializeCallback);
        writer.WriteOptionalCollection(OpenApiConstants.AnyOf, AnyOf, serializeCallback);
        writer.WriteOptionalCollection(OpenApiConstants.OneOf, OneOf, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Not, Not, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Items, Items, serializeCallback);
        writer.WriteOptionalMap(OpenApiConstants.Properties, Properties, serializeCallback);
        writer.WriteOptionalMap(OpenApiConstants.PatternProperties, PatternProperties, serializeCallback);
        if (AdditionalProperties is not null)
        {
            writer.WriteOptionalObject(OpenApiConstants.AdditionalProperties, AdditionalProperties, serializeCallback);
        }
        else if (AdditionalPropertiesAllowed.HasValue)
        {
            writer.WriteProperty(OpenApiConstants.AdditionalProperties, AdditionalPropertiesAllowed.Value);
        }
        writer.WriteOptionalObject(OpenApiConstants.Discriminator, Discriminator, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));
        if (UnevaluatedPropertiesSchema is not null)
        {
            writer.WriteOptionalObject(OpenApiConstants.UnevaluatedProperties, UnevaluatedPropertiesSchema, serializeCallback);
        }
        else if (UnevaluatedProperties.HasValue)
        {
            writer.WriteProperty(OpenApiConstants.UnevaluatedProperties, UnevaluatedProperties.Value);
        }
        writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Xml, Xml, serializeCallback);
        writer.WriteOptionalMap(OpenApiConstants.DependentRequired, DependentRequired, (w, s) => w.WriteValue(s));
        writer.WriteOptionalObject(OpenApiConstants.Contains, Contains, serializeCallback);
        writer.WriteProperty(OpenApiConstants.MaxContains, MaxContains);
        writer.WriteProperty(OpenApiConstants.MinContains, MinContains);
        writer.WriteProperty(OpenApiConstants.ContentEncoding, ContentEncoding);
        writer.WriteProperty(OpenApiConstants.ContentMediaType, ContentMediaType);
        writer.WriteOptionalObject(OpenApiConstants.ContentSchema, ContentSchema, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.PropertyNames, PropertyNames, serializeCallback);
        writer.WriteOptionalMap(OpenApiConstants.DependentSchemas, DependentSchemas, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.If, If, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Then, Then, serializeCallback);
        writer.WriteOptionalObject(OpenApiConstants.Else, Else, serializeCallback);

        // Additional schema metadata annotations in 3.1
        writer.WriteOptionalObject(OpenApiConstants.Default, Default, (w, d) => w.WriteAny(d));
        writer.WriteProperty(OpenApiConstants.Title, Title);
        if (Deprecated.HasValue)
        {
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated.Value, false);
        }
        if (ReadOnly.HasValue)
        {
            writer.WriteProperty(OpenApiConstants.ReadOnly, ReadOnly.Value, false);
        }
        if (WriteOnly.HasValue)
        {
            writer.WriteProperty(OpenApiConstants.WriteOnly, WriteOnly.Value, false);
        }
        if (Examples != null && Examples.Any())
        {
            writer.WriteOptionalCollection(OpenApiConstants.Examples, Examples, (w, e) => w.WriteAny(e));
        }
        writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_1);
    }

    private static void WriteRawProperty(IOpenApiWriter writer, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            writer.WritePropertyName(name);
            writer.WriteRaw(value!);
        }
    }

    private static void WriteSchemaType(IOpenApiWriter writer, string name, JsonSchemaType? schemaType, bool allowMultipleTypes)
    {
        if (!schemaType.HasValue)
        {
            return;
        }

        var values = schemaType.Value.ToIdentifiers();
        if (values is null || values.Length == 0)
        {
            return;
        }

        if (allowMultipleTypes && values.Length > 1)
        {
            writer.WriteOptionalCollection(name, values, (w, s) =>
            {
                if (!string.IsNullOrEmpty(s))
                {
                    w.WriteValue(s!);
                }
            });
        }
        else
        {
            writer.WriteProperty(name, values[0]);
        }
    }

    /// <inheritdoc/>
    [Obsolete("Use ApplySchemaMetadata instead.")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
    {
        //TODO remove this method in next major release
        // no-op: we're using ApplySchemaMetadata
    }

    internal void ApplySchemaMetadata(OpenApiSchema schema, JsonObject jsonObject)
    {
        Title = schema.Title;
        Description = schema.Description;
        Default = schema.Default;
        if (jsonObject.ContainsKey(OpenApiConstants.Deprecated))
        {
            Deprecated = schema.Deprecated;
        }
        if (jsonObject.ContainsKey(OpenApiConstants.ReadOnly))
        {
            ReadOnly = schema.ReadOnly;
        }
        if (jsonObject.ContainsKey(OpenApiConstants.WriteOnly))
        {
            WriteOnly = schema.WriteOnly;
        }
        Examples = schema.Examples;
        Extensions = schema.Extensions;
        SchemaId = schema.Id;
        Schema = schema.Schema;
        Comment = schema.Comment;
        if (schema.Vocabulary is { Count: > 0 })
        {
            Vocabulary = schema.Vocabulary;
        }
        DynamicRef = schema.DynamicRef;
        DynamicAnchor = schema.DynamicAnchor;
        if (schema.Definitions is { Count: > 0 })
        {
            Definitions = schema.Definitions;
        }
        Anchor = schema.Anchor;
        ExclusiveMaximum = schema.ExclusiveMaximum;
        ExclusiveMinimum = schema.ExclusiveMinimum;
        SchemaType = schema.Type;
        Const = schema.Const;
        Format = schema.Format;
        Maximum = schema.Maximum;
        Minimum = schema.Minimum;
        MaxLength = schema.MaxLength;
        MinLength = schema.MinLength;
        Pattern = schema.Pattern;
        MultipleOf = schema.MultipleOf;
        AllOf = schema.AllOf;
        OneOf = schema.OneOf;
        AnyOf = schema.AnyOf;
        Not = schema.Not;
        Required = schema.Required;
        Items = schema.Items;
        MaxItems = schema.MaxItems;
        MinItems = schema.MinItems;
        UniqueItems = schema.UniqueItems;
        Contains = schema.Contains;
        MaxContains = schema.MaxContains;
        MinContains = schema.MinContains;
        Properties = schema.Properties;
        PatternProperties = schema.PatternProperties;
        MaxProperties = schema.MaxProperties;
        MinProperties = schema.MinProperties;
        if (jsonObject.TryGetPropertyValue(OpenApiConstants.AdditionalProperties, out var additionalPropertiesNode) &&
            additionalPropertiesNode is JsonValue)
        {
            AdditionalPropertiesAllowed = schema.AdditionalPropertiesAllowed;
        }
        AdditionalProperties = schema.AdditionalProperties;
        Discriminator = schema.Discriminator;
        Example = schema.Example;
        Enum = schema.Enum;
        if (jsonObject.TryGetPropertyValue(OpenApiConstants.UnevaluatedProperties, out var unevaluatedPropertiesNode) &&
            unevaluatedPropertiesNode is JsonValue)
        {
            UnevaluatedProperties = schema.UnevaluatedProperties;
        }
        UnevaluatedPropertiesSchema = schema.UnevaluatedPropertiesSchema;
        ExternalDocs = schema.ExternalDocs;
        Xml = schema.Xml;
        UnrecognizedKeywords = schema.UnrecognizedKeywords;
        DependentRequired = schema.DependentRequired;
        ContentEncoding = schema.ContentEncoding;
        ContentMediaType = schema.ContentMediaType;
        ContentSchema = schema.ContentSchema;
        PropertyNames = schema.PropertyNames;
        DependentSchemas = schema.DependentSchemas;
        If = schema.If;
        Then = schema.Then;
        Else = schema.Else;
    }
}
