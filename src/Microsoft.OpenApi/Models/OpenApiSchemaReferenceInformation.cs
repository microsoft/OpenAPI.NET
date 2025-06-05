// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Schema reference information that includes metadata annotations from JSON Schema 2020-12.
    /// This class extends OpenApiReference to provide schema-specific metadata override capabilities.
    /// </summary>
    public class OpenApiSchemaReferenceInformation : OpenApiReference
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
        public OpenApiSchemaReferenceInformation() { }

        /// <summary>
        /// Initializes a copy instance of the <see cref="OpenApiSchemaReferenceInformation"/> object
        /// </summary>
        public OpenApiSchemaReferenceInformation(OpenApiSchemaReferenceInformation reference) : base(reference)
        {
            Utils.CheckArgumentNull(reference);
            Default = reference.Default;
            Title = reference.Title;
            Deprecated = reference.Deprecated;
            ReadOnly = reference.ReadOnly;
            WriteOnly = reference.WriteOnly;
            Examples = reference.Examples;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchemaReferenceInformation"/> to Open Api v3.1.
        /// </summary>
        public override void SerializeAsV31(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            if (Type == ReferenceType.Tag && !string.IsNullOrEmpty(ReferenceV3) && ReferenceV3 is not null)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV3);
                return;
            }

            writer.WriteStartObject();
            
            // summary and description are in 3.1 but not in 3.0
            writer.WriteProperty(OpenApiConstants.Summary, Summary);
            writer.WriteProperty(OpenApiConstants.Description, Description);
            
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

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Sets metadata fields from a JSON node during parsing
        /// </summary>
        internal override void SetMetadataFromMapNode(MapNode mapNode)
        {
            base.SetMetadataFromMapNode(mapNode);
            
            if (mapNode.JsonNode is not JsonObject jsonObject) return;

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

        private static string? GetPropertyValueFromNode(JsonObject jsonObject, string key) =>
            jsonObject.TryGetPropertyValue(key, out var valueNode) && valueNode is JsonValue valueCast && valueCast.TryGetValue<string>(out var strValue) ? strValue : null;
    }
}
