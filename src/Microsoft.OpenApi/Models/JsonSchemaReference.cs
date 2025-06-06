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
    }

    /// <inheritdoc/>
    protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
    {
        if (Type != ReferenceType.Schema) throw new InvalidOperationException(
            $"JsonSchemaReference can only be serialized for ReferenceType.Schema, but was {Type}.");

        base.SerializeAdditionalV31Properties(writer);        
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
            Examples = new List<JsonNode>();
            foreach (var example in examplesArray)
            {
                if (example != null)
                {
                    Examples.Add(example);
                }
            }
        }
    }
}
