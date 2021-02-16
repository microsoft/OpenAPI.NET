using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiResponseSerializer : ReferenceOpenApiSerializer<OpenApiResponse>
    {
        private readonly IOpenApiReferenceElementSerializer<OpenApiHeader> _headerSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSchema> _schemaSerializer;
        public V2OpenApiResponseSerializer(
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer,
            IOpenApiReferenceElementSerializer<OpenApiHeader> headerSerializer,
            IOpenApiElementSerializer<OpenApiSchema> schemaSerializer) : base(referenceSerializer)
        {
            _headerSerializer = headerSerializer;
            _schemaSerializer = schemaSerializer;
        }

        public override void SerializeWithoutReference(OpenApiResponse element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteRequiredProperty(OpenApiConstants.Description, element.Description);

            var extensionsClone = new Dictionary<string, IOpenApiExtension>(element.Extensions);

            if (element.Content != null)
            {
                var mediatype = element.Content.FirstOrDefault();
                if (mediatype.Value != null)
                {
                    // schema
                    writer.WriteOptionalObject(
                        OpenApiConstants.Schema,
                        mediatype.Value.Schema,
                        (w, s) => _schemaSerializer.Serialize(s, w));

                    // examples
                    if (element.Content.Values.Any(m => m.Example != null))
                    {
                        writer.WritePropertyName(OpenApiConstants.Examples);
                        writer.WriteStartObject();

                        foreach (var mediaTypePair in element.Content)
                        {
                            if (mediaTypePair.Value.Example != null)
                            {
                                writer.WritePropertyName(mediaTypePair.Key);
                                writer.WriteAny(mediaTypePair.Value.Example);
                            }
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteExtensions(mediatype.Value.Extensions, OpenApiSpecVersion.OpenApi2_0);

                    foreach (var key in mediatype.Value.Extensions.Keys)
                    {
                        // The extension will already have been serialized as part of the call above,
                        // so remove it from the cloned collection so we don't write it again.
                        extensionsClone.Remove(key);
                    }
                }
            }

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, element.Headers, (w, h) => _headerSerializer.Serialize(h, w));

            // extension
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
