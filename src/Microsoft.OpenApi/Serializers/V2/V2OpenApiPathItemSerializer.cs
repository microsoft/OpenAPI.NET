using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiPathItemSerializer : IOpenApiElementSerializer<OpenApiPathItem>
    {
        private readonly IOpenApiElementSerializer<OpenApiParameter> _parameterSerializer;

        private readonly IOpenApiElementSerializer<OpenApiOperation> _operationSerializer;
        public V2OpenApiPathItemSerializer(
            IOpenApiElementSerializer<OpenApiOperation> operationSerializer,
            IOpenApiElementSerializer<OpenApiParameter> parameterSerializer)
        {
            _parameterSerializer = parameterSerializer;
            _operationSerializer = operationSerializer;
        }

        public void Serialize(OpenApiPathItem element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // operations except "trace"
            foreach (var operation in element.Operations)
            {
                if (operation.Key != OperationType.Trace)
                {
                    writer.WriteOptionalObject(
                        operation.Key.GetDisplayName(),
                        operation.Value,
                        (w, o) => _operationSerializer.Serialize(o, w));
                }
            }

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, element.Parameters, (w, p) => _parameterSerializer.Serialize(p, w));

            // write "summary" as extensions
            writer.WriteProperty(OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Summary, element.Summary);

            // write "description" as extensions
            writer.WriteProperty(
                OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Description,
                element.Description);

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
