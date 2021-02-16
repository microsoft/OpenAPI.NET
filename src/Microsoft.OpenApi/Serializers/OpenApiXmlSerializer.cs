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
    public class OpenApiXmlSerializer : IOpenApiElementSerializer<OpenApiXml>
    {
        private readonly IOpenApiSpecProvider _specProvider;
        public OpenApiXmlSerializer(IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
        }

        public void Serialize(OpenApiXml element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, element.Name);

            // namespace
            writer.WriteProperty(OpenApiConstants.Namespace, element.Namespace?.AbsoluteUri);

            // prefix
            writer.WriteProperty(OpenApiConstants.Prefix, element.Prefix);

            // attribute
            writer.WriteProperty(OpenApiConstants.Attribute, element.Attribute, false);

            // wrapped
            writer.WriteProperty(OpenApiConstants.Wrapped, element.Wrapped, false);

            // extensions
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
