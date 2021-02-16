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
    public class OpenApiLicenseSerializer : IOpenApiElementSerializer<OpenApiLicense>
    {
        private readonly IOpenApiSpecProvider _specProvider;
        public OpenApiLicenseSerializer(IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
        }
        public void Serialize(OpenApiLicense element, IOpenApiWriter writer)
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

            // specification extensions
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
