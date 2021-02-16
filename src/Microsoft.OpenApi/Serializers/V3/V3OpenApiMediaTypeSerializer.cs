using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiMediaTypeSerializer : IOpenApiElementSerializer<OpenApiMediaType>
    {
        private readonly IOpenApiElementSerializer<OpenApiSchema> _schemaSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExample> _exampleSerializer;

        private readonly IOpenApiElementSerializer<OpenApiEncoding> _encodingSerializer;

        public V3OpenApiMediaTypeSerializer(
            IOpenApiElementSerializer<OpenApiSchema> schemaSerializer,
            IOpenApiElementSerializer<OpenApiExample> exampleSerializer,
            IOpenApiElementSerializer<OpenApiEncoding> encodingSerializer)
        {
            _schemaSerializer = schemaSerializer;
            _exampleSerializer = exampleSerializer;
            _encodingSerializer = encodingSerializer;
        }

        public void Serialize(OpenApiMediaType element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, element.Schema, (w, s) => _schemaSerializer.Serialize(s, w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, element.Example, (w, e) => w.WriteAny(e));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, element.Examples, (w, e) => _exampleSerializer.Serialize(e, w));

            // encoding
            writer.WriteOptionalMap(OpenApiConstants.Encoding, element.Encoding, (w, e) => _encodingSerializer.Serialize(e, w));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
