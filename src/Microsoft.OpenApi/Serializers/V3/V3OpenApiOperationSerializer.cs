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
    public class V3OpenApiOperationSerializer : IOpenApiElementSerializer<OpenApiOperation>
    {
        private readonly IOpenApiElementSerializer<OpenApiCallback> _callbackSerializer;

        private readonly IOpenApiElementSerializer<OpenApiServer> _serverSerializer;

        private readonly IOpenApiElementSerializer<OpenApiParameter> _parameterSerializer;

        private readonly IOpenApiElementSerializer<OpenApiResponses> _responsesSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSecurityRequirement> _securityRequirementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiRequestBody> _requestBodySerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;

        private readonly IOpenApiElementSerializer<OpenApiTag> _tagSerializer;

        public V3OpenApiOperationSerializer(
            IOpenApiElementSerializer<OpenApiCallback> callbackSerializer,
            IOpenApiElementSerializer<OpenApiServer> serverSerializer,
            IOpenApiElementSerializer<OpenApiParameter> parameterSerializer,
            IOpenApiElementSerializer<OpenApiResponses> responsesSerializer,
            IOpenApiElementSerializer<OpenApiSecurityRequirement> securityRequirementSerializer,
            IOpenApiElementSerializer<OpenApiRequestBody> requestBodySerializer,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiElementSerializer<OpenApiTag> tagSerializer)
        {
            _callbackSerializer = callbackSerializer;
            _serverSerializer = serverSerializer;
            _parameterSerializer = parameterSerializer;
            _responsesSerializer = responsesSerializer;
            _securityRequirementSerializer = securityRequirementSerializer;
            _requestBodySerializer = requestBodySerializer;
            _externalDocsSerializer = externalDocsSerializer;
            _tagSerializer = tagSerializer;
        }

        public void Serialize(OpenApiOperation element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteOptionalCollection(
                OpenApiConstants.Tags,
                element.Tags,
                (w, t) =>
                {
                    _tagSerializer.Serialize(t, w);
                });

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, element.Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, e) => _externalDocsSerializer.Serialize(e, w));

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, element.OperationId);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, element.Parameters, (w, p) => _parameterSerializer.Serialize(p, w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.RequestBody, element.RequestBody, (w, r) => _requestBodySerializer.Serialize(r, w));

            // responses
            writer.WriteRequiredObject(OpenApiConstants.Responses, element.Responses, (w, r) => _responsesSerializer.Serialize(r, w));

            // callbacks
            writer.WriteOptionalMap(OpenApiConstants.Callbacks, element.Callbacks, (w, c) => _callbackSerializer.Serialize(c, w));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, element.Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.Security, element.Security, (w, s) => _securityRequirementSerializer.Serialize(s, w));

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, element.Servers, (w, s) => _serverSerializer.Serialize(s, w));

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
