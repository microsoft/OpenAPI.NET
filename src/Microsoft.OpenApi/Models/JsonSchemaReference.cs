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
    /// A $id which by default SHOULD override that of the referenced component.
    /// Named SchemaId to avoid collision with the inherited reference identifier (BaseOpenApiReference.Id).
    /// </summary>
    public string? SchemaId { get; set; }

    /// <summary>
    /// A $comment which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// The $vocabulary which by default SHOULD override that of the referenced component.
    /// </summary>
    public IDictionary<string, bool>? Vocabulary { get; set; }

    /// <summary>
    /// The $dynamicRef which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? DynamicRef { get; set; }

    /// <summary>
    /// The $dynamicAnchor which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? DynamicAnchor { get; set; }

    /// <summary>
    /// The $defs which by default SHOULD override that of the referenced component.
    /// </summary>
    public IDictionary<string, IOpenApiSchema>? Definitions { get; set; }

    /// <summary>
    /// The $anchor which by default SHOULD override that of the referenced component.
    /// </summary>
    public string? Anchor { get; set; }

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
        Comment = reference.Comment;
        Vocabulary = reference.Vocabulary != null ? new Dictionary<string, bool>(reference.Vocabulary) : null;
        DynamicRef = reference.DynamicRef;
        DynamicAnchor = reference.DynamicAnchor;
        Definitions = reference.Definitions != null ? new Dictionary<string, IOpenApiSchema>(reference.Definitions) : null;
        Anchor = reference.Anchor;
    }

    /// <inheritdoc/>
    protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
    {
        SerializeAdditionalV3XProperties(writer, OpenApiSpecVersion.OpenApi3_1, base.SerializeAdditionalV31Properties);
    }
    /// <inheritdoc/>
    protected override void SerializeAdditionalV32Properties(IOpenApiWriter writer)
    {
        SerializeAdditionalV3XProperties(writer, OpenApiSpecVersion.OpenApi3_2, base.SerializeAdditionalV32Properties);
    }
    private void SerializeAdditionalV3XProperties(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter> baseSerializer)
    {
        if (Type != ReferenceType.Schema) throw new InvalidOperationException(
            $"JsonSchemaReference can only be serialized for ReferenceType.Schema, but was {Type}.");

        baseSerializer(writer);

        // JSON Schema 2020-12 keyword siblings (preserved per OAS 3.1+ / JSON Schema 2020-12 semantics)
        writer.WriteProperty(OpenApiConstants.Id, SchemaId);
        writer.WriteProperty(OpenApiConstants.Comment, Comment);
        writer.WriteOptionalMap(OpenApiConstants.Vocabulary, Vocabulary, (w, s) => w.WriteValue(s));
        if (version == OpenApiSpecVersion.OpenApi3_1)
        {
            writer.WriteOptionalMap(OpenApiConstants.Defs, Definitions, (w, s) => s.SerializeAsV31(w));
        }
        else
        {
            writer.WriteOptionalMap(OpenApiConstants.Defs, Definitions, (w, s) => s.SerializeAsV32(w));
        }
        writer.WriteProperty(OpenApiConstants.Anchor, Anchor);
        writer.WriteProperty(OpenApiConstants.DynamicRef, DynamicRef);
        writer.WriteProperty(OpenApiConstants.DynamicAnchor, DynamicAnchor);

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

    /// <inheritdoc/>
    protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
    {
        base.SetAdditional31MetadataFromMapNode(jsonObject);

        var title = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Title);
        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
        }

        // Boolean properties
        if (jsonObject.TryGetPropertyValue(OpenApiConstants.Deprecated, out var deprecatedNode) && deprecatedNode is JsonValue deprecatedValue && deprecatedValue.TryGetValue<bool>(out var deprecated))
        {
            Deprecated = deprecated;
        }

        if (jsonObject.TryGetPropertyValue(OpenApiConstants.ReadOnly, out var readOnlyNode) && readOnlyNode is JsonValue readOnlyValue && readOnlyValue.TryGetValue<bool>(out var readOnly))
        {
            ReadOnly = readOnly;
        }

        if (jsonObject.TryGetPropertyValue(OpenApiConstants.WriteOnly, out var writeOnlyNode) && writeOnlyNode is JsonValue writeOnlyValue && writeOnlyValue.TryGetValue<bool>(out var writeOnly))
        {
            WriteOnly = writeOnly;
        }

        // Default value
        if (jsonObject.TryGetPropertyValue(OpenApiConstants.Default, out var defaultNode))
        {
            Default = defaultNode;
        }

        // Examples
        if (jsonObject.TryGetPropertyValue(OpenApiConstants.Examples, out var examplesNode) && examplesNode is JsonArray examplesArray)
        {
            Examples = examplesArray.OfType<JsonNode>().ToList();
        }

        // Extensions (properties starting with "x-")
        foreach (var property in jsonObject
                    .Where(static p => p.Key.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase)
                            && p.Value is not null))
        {
            var extensionValue = property.Value!;
            Extensions ??= new Dictionary<string, IOpenApiExtension>(StringComparer.OrdinalIgnoreCase);
            Extensions[property.Key] = new JsonNodeExtension(extensionValue.DeepClone());
        }

        // JSON Schema 2020-12 keyword siblings ($defs is parsed separately in the deserializer
        // because it requires LoadSchema for nested schema materialization)
        SchemaId = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Id) ?? SchemaId;
        Comment = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Comment) ?? Comment;
        DynamicRef = GetPropertyValueFromNode(jsonObject, OpenApiConstants.DynamicRef) ?? DynamicRef;
        DynamicAnchor = GetPropertyValueFromNode(jsonObject, OpenApiConstants.DynamicAnchor) ?? DynamicAnchor;
        Anchor = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Anchor) ?? Anchor;

        if (jsonObject.TryGetPropertyValue(OpenApiConstants.Vocabulary, out var vocabNode) && vocabNode is JsonObject vocabObj)
        {
            Vocabulary = new Dictionary<string, bool>();
            foreach (var kvp in vocabObj)
            {
                if (kvp.Value is JsonValue v && v.TryGetValue<bool>(out var b))
                {
                    Vocabulary[kvp.Key] = b;
                }
            }
        }
    }
}
