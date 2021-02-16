using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V3OpenApiPathItemSerializer : IOpenApiElementSerializer<OpenApiPathItem>
    {
        private readonly IOpenApiElementSerializer<OpenApiParameter> _parameterSerializer;

        private readonly IOpenApiElementSerializer<OpenApiOperation> _operationSerializer;

        private readonly IOpenApiElementSerializer<OpenApiServer> _serverSerializer;
        public V3OpenApiPathItemSerializer(
            IOpenApiElementSerializer<OpenApiServer> serverSerializer,
            IOpenApiElementSerializer<OpenApiOperation> operationSerializer,
            IOpenApiElementSerializer<OpenApiParameter> parameterSerializer)
        {
            _parameterSerializer = parameterSerializer;
            _operationSerializer = operationSerializer;
            _serverSerializer = serverSerializer;
        }

        public void Serialize(OpenApiPathItem element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, element.Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // operations
            foreach (var operation in element.Operations)
            {
                writer.WriteOptionalObject(
                    operation.Key.GetDisplayName(),
                    operation.Value,
                    (w, o) => _operationSerializer.Serialize(o, w));
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, element.Servers, (w, s) => _serverSerializer.Serialize(s, w));

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, element.Parameters, (w, p) => _parameterSerializer.Serialize(p, w));

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
