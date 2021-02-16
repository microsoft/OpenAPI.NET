using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiSecuritySchemeSerializer : ReferenceOpenApiSerializer<OpenApiSecurityScheme>
    {
        public V2OpenApiSecuritySchemeSerializer(
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _predicate = (element, writer) => element.Reference != null;
        }

        public override void SerializeWithoutReference(OpenApiSecurityScheme element, IOpenApiWriter writer)
        {
            if (element.Type == SecuritySchemeType.Http && element.Scheme != OpenApiConstants.Basic)
            {
                // Bail because V2 does not support non-basic HTTP scheme
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (element.Type == SecuritySchemeType.OpenIdConnect)
            {
                // Bail because V2 does not support OpenIdConnect
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // type
            switch (element.Type)
            {
                case SecuritySchemeType.Http:
                    writer.WriteProperty(OpenApiConstants.Type, OpenApiConstants.Basic);
                    break;

                case SecuritySchemeType.OAuth2:
                    // These properties apply to ouauth2 type only.
                    // flow
                    // authorizationUrl
                    // tokenUrl
                    // scopes
                    writer.WriteProperty(OpenApiConstants.Type, element.Type.GetDisplayName());
                    WriteOAuthFlowForV2(writer, element.Flows);
                    break;

                case SecuritySchemeType.ApiKey:
                    // These properties apply to apiKey type only.
                    // name
                    // in
                    writer.WriteProperty(OpenApiConstants.Type, element.Type.GetDisplayName());
                    writer.WriteProperty(OpenApiConstants.Name, element.Name);
                    writer.WriteProperty(OpenApiConstants.In, element.In.GetDisplayName());
                    break;
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Arbitrarily chooses one <see cref="OpenApiOAuthFlow"/> object from the <see cref="OpenApiOAuthFlows"/>
        /// to populate in V2 security scheme.
        /// </summary>
        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, OpenApiOAuthFlows flows)
        {
            if (flows != null)
            {
                if (flows.Implicit != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Implicit, flows.Implicit);
                }
                else if (flows.Password != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Password, flows.Password);
                }
                else if (flows.ClientCredentials != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Application, flows.ClientCredentials);
                }
                else if (flows.AuthorizationCode != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.AccessCode, flows.AuthorizationCode);
                }
            }
        }

        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, string flowValue, OpenApiOAuthFlow flow)
        {
            // flow
            writer.WriteProperty(OpenApiConstants.Flow, flowValue);

            // authorizationUrl
            writer.WriteProperty(OpenApiConstants.AuthorizationUrl, flow.AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteProperty(OpenApiConstants.TokenUrl, flow.TokenUrl?.ToString());

            // scopes
            writer.WriteOptionalMap(OpenApiConstants.Scopes, flow.Scopes, (w, s) => w.WriteValue(s));
        }
    }
}
