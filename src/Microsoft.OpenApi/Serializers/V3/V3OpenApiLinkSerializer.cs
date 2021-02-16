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
    public class V3OpenApiLinkSerializer : ReferenceOpenApiSerializer<OpenApiLink>
    {
        private readonly IOpenApiElementSerializer<OpenApiServer> _serverSerializer;
        public V3OpenApiLinkSerializer(
            IOpenApiElementSerializer<OpenApiServer> serverSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _serverSerializer = serverSerializer;
        }

        public override void SerializeWithoutReference(OpenApiLink element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // operationRef
            writer.WriteProperty(OpenApiConstants.OperationRef, element.OperationRef);

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, element.OperationId);

            // parameters
            writer.WriteOptionalMap(OpenApiConstants.Parameters, element.Parameters, (w, p) => p.WriteValue(w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.RequestBody, element.RequestBody, (w, r) => r.WriteValue(w));

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // server
            writer.WriteOptionalObject(OpenApiConstants.Server, element.Server, (w, s) => _serverSerializer.Serialize(s, w));

            writer.WriteEndObject();
        }
    }
}
