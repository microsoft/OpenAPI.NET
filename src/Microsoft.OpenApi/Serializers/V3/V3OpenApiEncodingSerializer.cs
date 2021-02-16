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
    public class V3OpenApiEncodingSerializer : IOpenApiElementSerializer<OpenApiEncoding>
    {
        private readonly IOpenApiElementSerializer<OpenApiHeader> _headerElementSerializer;
        public V3OpenApiEncodingSerializer(IOpenApiElementSerializer<OpenApiHeader> headerElementSerializer)
        {
            _headerElementSerializer = headerElementSerializer;
        }

        public void Serialize(OpenApiEncoding element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();

            // contentType
            writer.WriteProperty(OpenApiConstants.ContentType, element.ContentType);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, element.Headers, (w, h) => _headerElementSerializer.Serialize(h, w));

            // style
            writer.WriteProperty(OpenApiConstants.Style, element.Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, element.Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, element.AllowReserved, false);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
