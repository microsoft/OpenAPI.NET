// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Schema Object.
    /// </summary>
    public class OpenApiSchema : IOpenApiReference, IOpenApiExtension
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public decimal? Maximum { get; set; }
        public bool ExclusiveMaximum { get; set; } = false;
        public decimal? Minimum { get; set; }
        public bool ExclusiveMinimum { get; set; } = false;
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Pattern { get; set; }
        public decimal MultipleOf { get; set; }
        public string Default { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public IList<OpenApiSchema> AllOf { get; set; }
        public IList<OpenApiSchema> OneOf { get; set; }
        public IList<OpenApiSchema> AnyOf { get; set; }
        public OpenApiSchema Not { get; set; }
        public string[] Required { get; set; }
        public OpenApiSchema Items { get; set; }
        public int? MaxItems { get; set; }
        public int? MinItems { get; set; }
        public bool UniqueItems { get; set; }
        public IDictionary<string,OpenApiSchema> Properties { get; set; }
        public int? MaxProperties { get; set; }
        public int? MinProperties { get; set; }
        public bool AdditionalPropertiesAllowed { get; set; }
        public OpenApiSchema AdditionalProperties { get; set; }

        public string Example { get; set; }
        public IList<string> Enum { get; set; } = new List<string>();
        public bool Nullable { get; set; }
        public OpenApiExternalDocs ExternalDocs { get; set; }
        public bool Deprecated { get; set; }

        /// <summary>
        /// Adds additional metadata to describe the XML representation of this property.
        /// </summary>
        public OpenApiXml Xml { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get;
            set;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("title", Title);
                writer.WriteStringProperty("type", Type);
                writer.WriteStringProperty("format", Format);
                writer.WriteStringProperty("description", Description);

                writer.WriteNumberProperty("maxLength", MaxLength);
                writer.WriteNumberProperty("minLength", MinLength);
                writer.WriteStringProperty("pattern", Pattern);
                writer.WriteStringProperty("default", Default);

                writer.WriteList("required", Required, (nodeWriter, s) => nodeWriter.WriteValue(s));

                writer.WriteNumberProperty("maximum", Maximum);
                writer.WriteBoolProperty("exclusiveMaximum", ExclusiveMaximum, false);
                writer.WriteNumberProperty("minimum", Minimum);
                writer.WriteBoolProperty("exclusiveMinimum", ExclusiveMinimum, false);

                if (AdditionalProperties != null)
                {
                    writer.WritePropertyName("additionalProperties");
                    AdditionalProperties.WriteAsV3(writer);
                }

                if (Items != null)
                {
                    writer.WritePropertyName("items");
                    Items.WriteAsV3(writer);
                }
                writer.WriteNumberProperty("maxItems", MaxItems);
                writer.WriteNumberProperty("minItems", MinItems);

                if (Properties != null)
                {
                    writer.WritePropertyName("properties");
                    writer.WriteStartObject();
                    foreach (var prop in Properties)
                    {
                        writer.WritePropertyName(prop.Key);
                        if (prop.Value != null)
                        {
                            prop.Value.WriteAsV3(writer);
                        }
                        else
                        {
                            writer.WriteValue("null");
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteNumberProperty("maxProperties", MaxProperties);
                writer.WriteNumberProperty("minProperties", MinProperties);

                writer.WriteList("enum", Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                SerializeSchemaProperties(writer);
                writer.WriteEndObject();
            }
        }

        public void SerializeSchemaProperties(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStringProperty("title", Title);
            writer.WriteStringProperty("type", Type);
            writer.WriteStringProperty("format", Format);
            writer.WriteStringProperty("description", Description);

            writer.WriteNumberProperty("maxLength", MaxLength);
            writer.WriteNumberProperty("minLength", MinLength);
            writer.WriteStringProperty("pattern", Pattern);
            writer.WriteStringProperty("default", Default);

            writer.WriteList("required", Required, (nodeWriter, s) => nodeWriter.WriteValue(s));

            writer.WriteNumberProperty("maximum", Maximum);
            writer.WriteBoolProperty("exclusiveMaximum", ExclusiveMaximum, false);
            writer.WriteNumberProperty("minimum", Minimum);
            writer.WriteBoolProperty("exclusiveMinimum", ExclusiveMinimum, false);

            if (AdditionalProperties != null)
            {
                writer.WritePropertyName("additionalProperties");
                AdditionalProperties.WriteAsV2(writer);
            }

            if (Items != null)
            {
                writer.WritePropertyName("items");
                Items.WriteAsV2(writer);
            }
            writer.WriteNumberProperty("maxItems", MaxItems);
            writer.WriteNumberProperty("minItems", MinItems);

            if (Properties != null)
            {
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                foreach (var prop in Properties)
                {
                    writer.WritePropertyName(prop.Key);
                    if (prop.Value != null)
                    {
                        prop.Value.WriteAsV2(writer);
                    }
                    else
                    {
                        writer.WriteValue("null");
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteNumberProperty("maxProperties", MaxProperties);
            writer.WriteNumberProperty("minProperties", MinProperties);

            writer.WriteList("enum", Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));
        }
    }
}
