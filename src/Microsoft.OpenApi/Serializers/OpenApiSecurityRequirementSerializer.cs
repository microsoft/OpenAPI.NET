using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public class OpenApiSecurityRequirementSerializer : IOpenApiElementSerializer<OpenApiSecurityRequirement>
    {
        private readonly IOpenApiElementSerializer<OpenApiSecurityScheme> _securitySchemeSerializer;
        public OpenApiSecurityRequirementSerializer(IOpenApiElementSerializer<OpenApiSecurityScheme> securitySchemeSerializer)
        {
            _securitySchemeSerializer = securitySchemeSerializer;
        }

        public void Serialize(OpenApiSecurityRequirement element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            foreach (var securitySchemeAndScopesValuePair in element)
            {
                var securityScheme = securitySchemeAndScopesValuePair.Key;
                var scopes = securitySchemeAndScopesValuePair.Value;

                if (securityScheme.Reference == null)
                {
                    // Reaching this point means the reference to a specific OpenApiSecurityScheme fails.
                    // We are not able to serialize this SecurityScheme/Scopes key value pair since we do not know what
                    // string to output.
                    continue;
                }

                _securitySchemeSerializer.Serialize(securityScheme, writer);

                writer.WriteStartArray();

                foreach (var scope in scopes)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
