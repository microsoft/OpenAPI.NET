using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiHeaderSerializer : ReferenceOpenApiSerializer<OpenApiHeader>
    {
        private readonly IOpenApiElementSerializer<OpenApiExample> _exampleSerializer;

        private readonly IOpenApiElementSerializer<OpenApiMediaType> _mediaTypeSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSchema> _schemaSerializer;
        public V3OpenApiHeaderSerializer(
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer,
            IOpenApiElementSerializer<OpenApiExample> exampleSerializer,
            IOpenApiElementSerializer<OpenApiMediaType> mediaTypeSerializer,
            IOpenApiElementSerializer<OpenApiSchema> schemaSerializer) : base(referenceSerializer)
        {
            _exampleSerializer = exampleSerializer;
            _mediaTypeSerializer = mediaTypeSerializer;
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
            writer.WriteOptionalObject(OpenApiConstants.Schema, element.Schema, (w, s) => _schemaSerializer.Serialize(s, w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, element.Example, (w, s) => w.WriteAny(s));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, element.Examples, (w, e) => _exampleSerializer.Serialize(e, w));

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, element.Content, (w, c) => _mediaTypeSerializer.Serialize(c, w));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
