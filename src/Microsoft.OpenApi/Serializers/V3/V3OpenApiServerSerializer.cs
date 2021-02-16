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
    public class V3OpenApiServerSerializer : IOpenApiElementSerializer<OpenApiServer>
    {
        private readonly IOpenApiElementSerializer<OpenApiServerVariable> _serverVariableSerializer;
        public V3OpenApiServerSerializer(IOpenApiElementSerializer<OpenApiServerVariable> serverVariableSerializer)
        {
            _serverVariableSerializer = serverVariableSerializer;
        }

        public void Serialize(OpenApiServer element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // url
            writer.WriteProperty(OpenApiConstants.Url, element.Url);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // variables
            writer.WriteOptionalMap(OpenApiConstants.Variables, element.Variables, (w, v) => _serverVariableSerializer.Serialize(v, w));

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
