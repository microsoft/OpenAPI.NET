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
    public class V3OpenApiResponseSerializer : ReferenceOpenApiSerializer<OpenApiResponse>
    {
        private readonly IOpenApiElementSerializer<OpenApiHeader> _headerSerializer;

        private readonly IOpenApiElementSerializer<OpenApiMediaType> _mediaTypeSerializer;

        private readonly IOpenApiElementSerializer<OpenApiLink> _linkSerializer;
        public V3OpenApiResponseSerializer(
            IOpenApiElementSerializer<OpenApiHeader> headerSerializer,
            IOpenApiElementSerializer<OpenApiMediaType> mediaTypeSerializer,
            IOpenApiElementSerializer<OpenApiLink> linkSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _headerSerializer = headerSerializer;
            _mediaTypeSerializer = mediaTypeSerializer;
            _linkSerializer = linkSerializer;
        }

        public override void SerializeWithoutReference(OpenApiResponse element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteRequiredProperty(OpenApiConstants.Description, element.Description);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, element.Headers, (w, h) => _headerSerializer.Serialize(h, w));

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, element.Content, (w, c) => _mediaTypeSerializer.Serialize(c, w));

            // links
            writer.WriteOptionalMap(OpenApiConstants.Links, element.Links, (w, l) => _linkSerializer.Serialize(l, w));

            // extension
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
