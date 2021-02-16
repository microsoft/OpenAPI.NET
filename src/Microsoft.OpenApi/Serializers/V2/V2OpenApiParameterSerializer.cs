using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiParameterSerializer : ReferenceOpenApiSerializer<OpenApiParameter>
    {
        private readonly V2OpenApiSchemaSerializer _schemaSerializer;
        public V2OpenApiParameterSerializer(
            V2OpenApiSchemaSerializer schemaSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _schemaSerializer = schemaSerializer;
        }

        public override void SerializeWithoutReference(OpenApiParameter element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // in
            if (element is OpenApiFormDataParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "formData");
            }
            else if (element is OpenApiBodyParameter)
            {
                writer.WriteProperty(OpenApiConstants.In, "body");
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.In, element.In?.GetDisplayName());
            }

            // name
            writer.WriteProperty(OpenApiConstants.Name, element.Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, element.Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, element.Deprecated, false);

            var extensionsClone = new Dictionary<string, IOpenApiExtension>(element.Extensions);

            // schema
            if (element is OpenApiBodyParameter)
            {
                writer.WriteOptionalObject(OpenApiConstants.Schema, element.Schema, (w, s) => _schemaSerializer.Serialize(s, w));
            }
            // In V2 parameter's type can't be a reference to a custom object schema or can't be of type object
            // So in that case map the type as string.
            else
            if (element.Schema?.UnresolvedReference == true || element.Schema?.Type == "object")
            {
                writer.WriteProperty(OpenApiConstants.Type, "string");
            }
            else
            {
                // type
                // format
                // items
                // collectionFormat
                // default
                // maximum
                // exclusiveMaximum
                // minimum
                // exclusiveMinimum
                // maxLength
                // minLength
                // pattern
                // maxItems
                // minItems
                // uniqueItems
                // enum
                // multipleOf
                if (element.Schema != null)
                {
                    _schemaSerializer.WriteAsItemsProperties(element.Schema, writer);

                    if (element.Schema.Extensions != null)
                    {
                        foreach (var key in element.Schema.Extensions.Keys)
                        {
                            // The extension will already have been serialized as part of the call to WriteAsItemsProperties above,
                            // so remove it from the cloned collection so we don't write it again.
                            extensionsClone.Remove(key);
                        }
                    }
                }

                // allowEmptyValue
                writer.WriteProperty(OpenApiConstants.AllowEmptyValue, element.AllowEmptyValue, false);

                if (element.In == ParameterLocation.Query)
                {
                    if (element.Style == ParameterStyle.Form && element.Explode == true)
                    {
                        writer.WriteProperty("collectionFormat", "multi");
                    }
                    else if (element.Style == ParameterStyle.PipeDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "pipes");
                    }
                    else if (element.Style == ParameterStyle.SpaceDelimited)
                    {
                        writer.WriteProperty("collectionFormat", "ssv");
                    }
                }
            }


            // extensions
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
