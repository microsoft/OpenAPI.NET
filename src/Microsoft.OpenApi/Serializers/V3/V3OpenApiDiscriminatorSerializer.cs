using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V3OpenApiDiscriminatorSerializer : IOpenApiElementSerializer<OpenApiDiscriminator>
    {
        public void Serialize(OpenApiDiscriminator element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // propertyName
            writer.WriteProperty(OpenApiConstants.PropertyName, element.PropertyName);

            // mapping
            writer.WriteOptionalMap(OpenApiConstants.Mapping, element.Mapping, (w, s) => w.WriteValue(s));

            writer.WriteEndObject();
        }
    }
}
