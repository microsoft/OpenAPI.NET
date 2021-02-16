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
    public class OpenApiExternalDocsSerializer : IOpenApiElementSerializer<OpenApiExternalDocs>
    {
        private readonly IOpenApiSpecProvider _specProvider;
        public OpenApiExternalDocsSerializer(IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
        }

        public void Serialize(OpenApiExternalDocs element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // url
            writer.WriteProperty(OpenApiConstants.Url, element.Url?.OriginalString);

            // extensions
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
