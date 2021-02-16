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
    public class V3OpenApiDocumentSerializer : IOpenApiElementSerializer<OpenApiDocument>
    {
        private readonly IOpenApiElementSerializer<OpenApiInfo> _infoElementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiServer> _serverElementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiPaths> _pathsElementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiComponents> _componentsElementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSecurityRequirement> _securityRequirementElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiTag> _tagElementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsElementSerializer;

        public V3OpenApiDocumentSerializer(
            IOpenApiElementSerializer<OpenApiInfo> infoElementSerializer,
            IOpenApiElementSerializer<OpenApiServer> serverElementSerializer,
            IOpenApiElementSerializer<OpenApiPaths> pathsElementSerializer,
            IOpenApiElementSerializer<OpenApiComponents> componentsElementSerializer,
            IOpenApiElementSerializer<OpenApiSecurityRequirement> securityRequirementElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiTag> tagElementSerializer,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsElementSerializer)
        {
            _infoElementSerializer = infoElementSerializer;
            _serverElementSerializer = serverElementSerializer;
            _pathsElementSerializer = pathsElementSerializer;
            _componentsElementSerializer = componentsElementSerializer;
            _securityRequirementElementSerializer = securityRequirementElementSerializer;
            _tagElementSerializer = tagElementSerializer;
            _externalDocsElementSerializer = externalDocsElementSerializer;
        }

        public void Serialize(OpenApiDocument element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // openapi
            writer.WriteProperty(OpenApiConstants.OpenApi, "3.0.1");

            // info
            writer.WriteRequiredObject(OpenApiConstants.Info, element.Info, (w, i) => _infoElementSerializer.Serialize(i, w));

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, element.Servers, (w, s) => _serverElementSerializer.Serialize(s, w));

            // paths
            writer.WriteRequiredObject(OpenApiConstants.Paths, element.Paths, (w, p) => _pathsElementSerializer.Serialize(p, w));

            // components
            writer.WriteOptionalObject(OpenApiConstants.Components, element.Components, (w, c) => _componentsElementSerializer.Serialize(c, w));

            // security
            writer.WriteOptionalCollection(
                OpenApiConstants.Security,
                element.SecurityRequirements,
                (w, s) => _securityRequirementElementSerializer.Serialize(s, w));

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.Tags, element.Tags, (w, t) => _tagElementSerializer.SerializeWithoutReference(t, w));

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, e) => _externalDocsElementSerializer.Serialize(e, w));

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
