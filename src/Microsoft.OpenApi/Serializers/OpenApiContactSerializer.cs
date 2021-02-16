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
    public class OpenApiContactSerializer : IOpenApiElementSerializer<OpenApiContact>
    {
        private readonly IOpenApiSpecProvider _specProvider;
        public OpenApiContactSerializer(IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
        }
        public void Serialize(OpenApiContact element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, element.Name);

            // url
            writer.WriteProperty(OpenApiConstants.Url, element.Url?.OriginalString);

            // email
            writer.WriteProperty(OpenApiConstants.Email, element.Email);

            // extensions
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
