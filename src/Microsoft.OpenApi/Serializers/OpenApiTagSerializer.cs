using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public class OpenApiTagSerializer : IOpenApiReferenceElementSerializer<OpenApiTag>
    {
        private readonly IOpenApiSpecProvider _specProvider;

        private readonly IOpenApiElementSerializer<OpenApiReference> _referenceSerializer;

        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;
        public OpenApiTagSerializer(
            IOpenApiSpecProvider specProvider,
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiElementSerializer<OpenApiReference> referenceSerializer)
        {
            _specProvider = specProvider;
            _externalDocsSerializer = externalDocsSerializer;
            _referenceSerializer = referenceSerializer;
        }

        public void Serialize(OpenApiTag element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (element.Reference != null)
            {
                _referenceSerializer.Serialize(element.Reference, writer);
                return;
            }

            writer.WriteValue(element.Name);
        }

        public void SerializeWithoutReference(OpenApiTag element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, element.Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, e) => _externalDocsSerializer.Serialize(e, w));

            // extensions.
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
