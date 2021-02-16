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
    public class V2OpenApiHeaderSerializer : ReferenceOpenApiSerializer<OpenApiHeader>
    {
        private readonly V2OpenApiSchemaSerializer _schemaSerializer;
        public V2OpenApiHeaderSerializer(
            V2OpenApiSchemaSerializer schemaSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _schemaSerializer = schemaSerializer;
        }

        public override void SerializeWithoutReference(OpenApiHeader element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, element.Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, element.Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, element.AllowEmptyValue, false);

            // style
            writer.WriteProperty(OpenApiConstants.Style, element.Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, element.Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, element.AllowReserved, false);

            // schema
            if (element.Schema != null)
            {
                _schemaSerializer.WriteAsItemsProperties(element.Schema, writer);
            }
           
            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, element.Example, (w, s) => w.WriteAny(s));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
