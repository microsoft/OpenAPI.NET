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
    public class V3OpenApiRequestBodySerializer : ReferenceOpenApiSerializer<OpenApiRequestBody>
    {
        private readonly IOpenApiElementSerializer<OpenApiMediaType> _mediaTypeSerializer;

        public V3OpenApiRequestBodySerializer(
            IOpenApiElementSerializer<OpenApiMediaType> mediaTypeSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _mediaTypeSerializer = mediaTypeSerializer;
            _predicate = (element, writer) => element.Reference != null;
        }

        public override void SerializeWithoutReference(OpenApiRequestBody element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // content
            writer.WriteRequiredMap(OpenApiConstants.Content, element.Content, (w, c) => _mediaTypeSerializer.Serialize(c, w));

            // required
            writer.WriteProperty(OpenApiConstants.Required, element.Required, false);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
