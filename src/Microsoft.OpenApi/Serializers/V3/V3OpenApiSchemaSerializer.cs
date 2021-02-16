using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiSchemaSerializer : IOpenApiReferenceElementSerializer<OpenApiSchema>
    {
        private readonly IOpenApiElementSerializer<OpenApiReference> _referenceSerializer;

        private readonly IOpenApiElementSerializer<OpenApiXml> _xmlSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;

        private readonly IOpenApiElementSerializer<OpenApiDiscriminator> _discriminatorSerializer;

        public V3OpenApiSchemaSerializer(
            IOpenApiElementSerializer<OpenApiXml> xmlSerializer,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer,
            IOpenApiElementSerializer<OpenApiDiscriminator> discriminatorSerializer)
        {
            _referenceSerializer = referenceSerializer;
            _externalDocsSerializer = externalDocsSerializer;
            _xmlSerializer = xmlSerializer;
            _discriminatorSerializer = discriminatorSerializer;
        }

        public void Serialize(OpenApiSchema element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            var settings = writer.GetSettings();

            if (element.Reference != null)
            {
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

            SerializeWithoutReference(element, writer);

            if (element.Reference != null)
            {
                settings.LoopDetector.PopLoop<OpenApiSchema>();
            }
        }

        public void SerializeWithoutReference(OpenApiSchema element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, element.Title);

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
            writer.WriteOptionalCollection(OpenApiConstants.Enum, element.Enum, (nodeWriter, s) => nodeWriter.WriteAny(s));

            // type
            writer.WriteProperty(OpenApiConstants.Type, element.Type);

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, element.AllOf, (w, s) => this.Serialize(s, w));

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, element.AnyOf, (w, s) => this.Serialize(s, w));

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, element.OneOf, (w, s) => this.Serialize(s, w));

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, element.Not, (w, s) => this.Serialize(s, w));

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, element.Items, (w, s) => this.Serialize(s, w));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, element.Properties, (w, s) => this.Serialize(s, w));

            // additionalProperties
            if (element.AdditionalPropertiesAllowed)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    element.AdditionalProperties,
                    (w, s) => this.Serialize(s, w));
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, element.AdditionalPropertiesAllowed);
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // format
            writer.WriteProperty(OpenApiConstants.Format, element.Format);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, element.Default, (w, d) => w.WriteAny(d));

            // nullable
            writer.WriteProperty(OpenApiConstants.Nullable, element.Nullable, false);

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, element.Discriminator, (w, s) => _discriminatorSerializer.Serialize(s, w));

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, element.ReadOnly, false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, element.WriteOnly, false);

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, element.Xml, (w, s) => _xmlSerializer.Serialize(s, w));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, s) => _externalDocsSerializer.Serialize(s, w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, element.Example, (w, e) => w.WriteAny(e));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, element.Deprecated, false);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
