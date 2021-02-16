using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V3OpenApiCallbackSerializer : ReferenceOpenApiSerializer<OpenApiCallback>
    {
        private readonly IOpenApiElementSerializer<OpenApiPathItem> _pathItemSerializer;
        public V3OpenApiCallbackSerializer(
            IOpenApiElementSerializer<OpenApiPathItem> pathItemSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
            _pathItemSerializer = pathItemSerializer;
        }

        public override void SerializeWithoutReference(OpenApiCallback element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // path items
            foreach (var item in element.PathItems)
            {
                writer.WriteRequiredObject(item.Key.Expression, item.Value, (w, p) => _pathItemSerializer.Serialize(p, w));
            }

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
