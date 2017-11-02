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
    public class OpenApiSchema : OpenApiElement, IOpenApiReference, IOpenApiExtension
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

        public IOpenApiAny Default { get; set; }

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

        public IDictionary<string, OpenApiSchema> Properties { get; set; }

        public int? MaxProperties { get; set; }

        public int? MinProperties { get; set; }

        public OpenApiSchema AdditionalProperties { get; set; }

        public OpenApiDiscriminator Discriminator { get; set; }

        public IOpenApiAny Example { get; set; }

        public IList<IOpenApiAny> Enum { get; set; } = new List<IOpenApiAny>();

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

        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
                return;
            }

            writer.WriteStartObject();

            // title
            writer.WriteStringProperty("title", Title);

            // multipleOf
            writer.WriteNumberProperty("multipleOf", MultipleOf);

            // maximum
            writer.WriteNumberProperty("maximum", Maximum);

            // exclusiveMaximum
            writer.WriteBoolProperty("exclusiveMaximum", ExclusiveMaximum, false);

            // minimum
            writer.WriteNumberProperty("minimum", Minimum);

            // exclusiveMinimum
            writer.WriteBoolProperty("exclusiveMinimum", ExclusiveMinimum, false);

            // maxLength
            writer.WriteNumberProperty("maxLength", MaxLength);

            // minLength
            writer.WriteNumberProperty("minLength", MinLength);

            // pattern
            writer.WriteStringProperty("pattern", Pattern);

            // maxItems
            writer.WriteNumberProperty("maxItems", MaxItems);

            // minItems
            writer.WriteNumberProperty("minItems", MinItems);

            // uniqueItems
            writer.WriteBoolProperty("uniqueItems", UniqueItems);

            // maxProperties
            writer.WriteNumberProperty("maxProperties", MaxProperties);

            // minProperties
            writer.WriteNumberProperty("minProperties", MinProperties);

            // required
            writer.WriteList("required", Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteList("enum", Enum, (nodeWriter, s) => nodeWriter.WriteAny(s));

            // type
            writer.WriteStringProperty("type", Type);

            // allOf
            writer.WriteOptionalCollection("allOf", AllOf, (w, s) => s.WriteAsV3(w));

            // anyOf
            writer.WriteOptionalCollection("anyOf", AnyOf, (w, s) => s.WriteAsV3(w));

            // oneOf
            writer.WriteOptionalCollection("oneOf", OneOf, (w, s) => s.WriteAsV3(w));

            // not
            writer.WriteOptionalObject("not", Not, (w, s) => s.WriteAsV3(w));

            // items
            writer.WriteObject("items", Items, (w, s) => s.WriteAsV3(w));

            // properties
            writer.WriteOptionalMap("properties", Properties, (w, s) => s.WriteAsV3(w));

            // additionalProperties
            writer.WriteObject("additionalProperties", AdditionalProperties, (w, s) => s.WriteAsV3(w));

            // description
            writer.WriteStringProperty("description", Description);

            // format
            writer.WriteStringProperty("format", Format);

            // default
            writer.WriteAnyProperty("default", Default);

            // readOnly
            writer.WriteBoolProperty("nullable", Nullable);

            // discriminator
            writer.WriteOptionalObject("discriminator", Discriminator, (w, s) => s.WriteAsV3(w));

            // readOnly
            writer.WriteBoolProperty("readOnly", ReadOnly);

            // writeOnly
            writer.WriteBoolProperty("writeOnly", WriteOnly);

            // xml
            writer.WriteOptionalObject("xml", Xml, (w, s) => s.WriteAsV2(w));

            // externalDocs
            writer.WriteOptionalObject("externalDocs", ExternalDocs, (w, s) => s.WriteAsV3(w));

            // example
            writer.WriteAnyProperty("example", Example);

            // deprecated
            writer.WriteBoolProperty("deprecated", Deprecated);

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSchema"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
                return;
            }

            writer.WriteStartObject();
            WriteAsSchemaProperties(writer);
            writer.WriteEndObject();
        }

        internal void WriteAsItemsProperties(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // type
            writer.WriteStringProperty("type", Type);

            // format
            writer.WriteStringProperty("format", Format);

            // items
            writer.WriteObject("items", Items, (w, s) => s.WriteAsV2(w));

            // collectionFormat

            // default
            writer.WriteAnyProperty("default", Default);

            // maximum
            writer.WriteNumberProperty("maximum", Maximum);

            // exclusiveMaximum
            writer.WriteBoolProperty("exclusiveMaximum", ExclusiveMaximum, false);

            // minimum
            writer.WriteNumberProperty("minimum", Minimum);

            // exclusiveMinimum
            writer.WriteBoolProperty("exclusiveMinimum", ExclusiveMinimum, false);

            // maxLength
            writer.WriteNumberProperty("maxLength", MaxLength);

            // minLength
            writer.WriteNumberProperty("minLength", MinLength);

            // pattern
            writer.WriteStringProperty("pattern", Pattern);

            // maxItems
            writer.WriteNumberProperty("maxItems", MaxItems);

            // minItems
            writer.WriteNumberProperty("minItems", MinItems);

            // enum
            writer.WriteList("enum", Enum, (w, s) => w.WriteAny(s));

            // multipleOf
            writer.WriteNumberProperty("multipleOf", MultipleOf);

            // extensions
            writer.WriteExtensions(Extensions);
        }

        internal void WriteAsSchemaProperties(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // format
            writer.WriteStringProperty("format", Format);

            // title
            writer.WriteStringProperty("title", Title);

            // description
            writer.WriteStringProperty("description", Description);

            // default
            writer.WriteAnyProperty("default", Default);

            // multipleOf
            writer.WriteNumberProperty("multipleOf", MultipleOf);

            // maximum
            writer.WriteNumberProperty("maximum", Maximum);

            // exclusiveMaximum
            writer.WriteBoolProperty("exclusiveMaximum", ExclusiveMaximum, false);

            // minimum
            writer.WriteNumberProperty("minimum", Minimum);

            // exclusiveMinimum
            writer.WriteBoolProperty("exclusiveMinimum", ExclusiveMinimum, false);

            // maxLength
            writer.WriteNumberProperty("maxLength", MaxLength);

            // minLength
            writer.WriteNumberProperty("minLength", MinLength);

            // pattern
            writer.WriteStringProperty("pattern", Pattern);

            // maxItems
            writer.WriteNumberProperty("maxItems", MaxItems);

            // minItems
            writer.WriteNumberProperty("minItems", MinItems);

            // uniqueItems
            writer.WriteBoolProperty("uniqueItems", UniqueItems);

            // maxProperties
            writer.WriteNumberProperty("maxProperties", MaxProperties);

            // minProperties
            writer.WriteNumberProperty("minProperties", MinProperties);

            // required
            writer.WriteList("required", Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteList("enum", Enum, (w, s) => w.WriteAny(s));

            // type
            writer.WriteStringProperty("type", Type);

            // items
            writer.WriteObject("items", Items, (w, s) => s.WriteAsV2(w));

            // allOf
            writer.WriteOptionalCollection("allOf", AllOf, (w, s) => s.WriteAsV2(w));

            // properties
            writer.WriteOptionalMap("properties", Properties, (w, s) => s.WriteAsV2(w));

            // additionalProperties
            writer.WriteObject("additionalProperties", AdditionalProperties, (w, s) => s.WriteAsV2(w));

            // discriminator
            writer.WriteStringProperty("discriminator", Discriminator?.PropertyName);

            // readOnly
            writer.WriteBoolProperty("readOnly", ReadOnly);

            // xml
            writer.WriteOptionalObject("xml", Xml, (w, s) => s.WriteAsV2(w));

            // externalDocs
            writer.WriteOptionalObject("externalDocs", ExternalDocs, (w, s) => s.WriteAsV2(w));

            // example
            writer.WriteAnyProperty("example", Example);

            // extensions
            writer.WriteExtensions(Extensions);
        }
    }
}