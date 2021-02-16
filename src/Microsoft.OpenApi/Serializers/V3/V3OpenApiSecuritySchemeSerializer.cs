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
    public class V3OpenApiSecuritySchemeSerializer : ReferenceOpenApiSerializer<OpenApiSecurityScheme>
    {
        private readonly IOpenApiElementSerializer<OpenApiOAuthFlows> _flowsSerializer;
        public V3OpenApiSecuritySchemeSerializer(
            IOpenApiElementSerializer<OpenApiOAuthFlows> flowsSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _flowsSerializer = flowsSerializer;
            _predicate = (element, writer) => element.Reference != null;
        }

        public override void SerializeWithoutReference(OpenApiSecurityScheme element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // type
            writer.WriteProperty(OpenApiConstants.Type, element.Type.GetDisplayName());

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            switch (element.Type)
            {
                case SecuritySchemeType.ApiKey:
                    // These properties apply to apiKey type only.
                    // name
                    // in
                    writer.WriteProperty(OpenApiConstants.Name, element.Name);
                    writer.WriteProperty(OpenApiConstants.In, element.In.GetDisplayName());
                    break;
                case SecuritySchemeType.Http:
                    // These properties apply to http type only.
                    // scheme
                    // bearerFormat
                    writer.WriteProperty(OpenApiConstants.Scheme, element.Scheme);
                    writer.WriteProperty(OpenApiConstants.BearerFormat, element.BearerFormat);
                    break;
                case SecuritySchemeType.OAuth2:
                    // This property apply to oauth2 type only.
                    // flows
                    writer.WriteOptionalObject(OpenApiConstants.Flows, element.Flows, (w, o) => _flowsSerializer.Serialize(o, w));
                    break;
                case SecuritySchemeType.OpenIdConnect:
                    // This property apply to openIdConnect only.
                    // openIdConnectUrl
                    writer.WriteProperty(OpenApiConstants.OpenIdConnectUrl, element.OpenIdConnectUrl?.ToString());
                    break;
            }

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
