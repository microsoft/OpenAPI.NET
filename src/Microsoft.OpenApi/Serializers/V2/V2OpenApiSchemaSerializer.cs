using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiSchemaSerializer : IOpenApiReferenceElementSerializer<OpenApiSchema>
    {
        private readonly IOpenApiElementSerializer<OpenApiReference> _referenceSerializer;

        private readonly IOpenApiElementSerializer<OpenApiXml> _xmlSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;

        public V2OpenApiSchemaSerializer(
            IOpenApiElementSerializer<OpenApiXml> xmlSerializer,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer)
        {
            _referenceSerializer = referenceSerializer;
            _externalDocsSerializer = externalDocsSerializer;
            _xmlSerializer = xmlSerializer;
        }

        public void Serialize(OpenApiSchema element, IOpenApiWriter writer)
        {
            Serialize(element, writer: writer, parentRequiredProperties: new HashSet<string>(), propertyName: null);
        }

        public void SerializeWithoutReference(OpenApiSchema element, IOpenApiWriter writer)
        {
            SerializeWithoutReference(
                element,
                writer: writer,
                parentRequiredProperties: new HashSet<string>(),
                propertyName: null);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference and handles not marking the provided property 
        /// as readonly if its included in the provided list of required properties of parent schema.
        /// </summary>
        /// <param name="writer">The open api writer.</param>
        /// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
        /// <param name="propertyName">The property name that will be serialized.</param>
        internal void SerializeWithoutReference(
            OpenApiSchema element,
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            writer.WriteStartObject();
            WriteAsSchemaProperties(element, writer, parentRequiredProperties, propertyName);
            writer.WriteEndObject();
        }

        internal void Serialize(
            OpenApiSchema element,
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (element.Reference != null)
            {
                var settings = writer.GetSettings();
                if (settings.ReferenceInline != ReferenceInlineSetting.InlineLocalReferences)
                {
                    _referenceSerializer.Serialize(element.Reference, writer);
                    return;
                }

                // If Loop is detected then just Serialize as a reference.
                if (!settings.LoopDetector.PushLoop<OpenApiSchema>(element))
                {
                    settings.LoopDetector.SaveLoop(element);
                    _referenceSerializer.Serialize(element.Reference, writer);
                    return;
                }
            }


            if (parentRequiredProperties == null)
            {
                parentRequiredProperties = new HashSet<string>();
            }

            SerializeWithoutReference(element, writer, parentRequiredProperties, propertyName);
        }

        internal void WriteAsItemsProperties(
            OpenApiSchema element,
            IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // type
            writer.WriteProperty(OpenApiConstants.Type, element.Type);

            // format
            writer.WriteProperty(OpenApiConstants.Format, element.Format);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, element.Items, (w, s) => this.Serialize(s, w));

            // collectionFormat
            // We need information from style in parameter to populate this.
            // The best effort we can make is to pull this information from the first parameter
            // that leverages this schema. However, that in itself may not be as simple
            // as the schema directly under parameter might be referencing one in the Components,
            // so we will need to do a full scan of the object before we can write the value for
            // this property. This is not supported yet, so we will skip this property at the moment.

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, element.Default, (w, d) => w.WriteAny(d));

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, element.Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, element.ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, element.Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, element.ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, element.MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, element.MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, element.Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, element.MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, element.MinItems);

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, element.Enum, (w, s) => w.WriteAny(s));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, element.MultipleOf);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);
        }

        internal void WriteAsSchemaProperties(
            OpenApiSchema element,
            IOpenApiWriter writer,
            ISet<string> parentRequiredProperties,
            string propertyName)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // format
            writer.WriteProperty(OpenApiConstants.Format, element.Format);

            // title
            writer.WriteProperty(OpenApiConstants.Title, element.Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, element.Default, (w, d) => w.WriteAny(d));

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, element.MultipleOf);

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, element.Maximum);

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, element.ExclusiveMaximum);

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, element.Minimum);

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, element.ExclusiveMinimum);

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, element.MaxLength);

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, element.MinLength);

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, element.Pattern);

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, element.MaxItems);

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, element.MinItems);

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, element.UniqueItems);

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, element.MaxProperties);

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, element.MinProperties);

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, element.Required, (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, element.Enum, (w, s) => w.WriteAny(s));

            // type
            writer.WriteProperty(OpenApiConstants.Type, element.Type);

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, element.Items, (w, s) => this.Serialize(s, w));

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, element.AllOf, (w, s) => this.Serialize(s, w));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, element.Properties, (w, key, s) =>
                this.Serialize(s, w, element.Required, key));

            // additionalProperties
            writer.WriteOptionalObject(
                OpenApiConstants.AdditionalProperties,
                element.AdditionalProperties,
                (w, s) => this.Serialize(s, w));

            // discriminator
            writer.WriteProperty(OpenApiConstants.Discriminator, element.Discriminator?.PropertyName);

            // readOnly
            // In V2 schema if a property is part of required properties of parent schema,
            // it cannot be marked as readonly.
            if (!parentRequiredProperties.Contains(propertyName))
            {
                writer.WriteProperty(name: OpenApiConstants.ReadOnly, value: element.ReadOnly, defaultValue: false);
            }

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, element.Xml, (w, s) => _xmlSerializer.Serialize(s, w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, s) => _externalDocsSerializer.Serialize(s, w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, element.Example, (w, e) => w.WriteAny(e));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);
        }
    }
}
