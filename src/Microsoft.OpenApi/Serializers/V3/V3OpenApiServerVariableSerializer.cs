using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiServerVariableSerializer : IOpenApiElementSerializer<OpenApiServerVariable>
    {
        public void Serialize(OpenApiServerVariable element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // default
            writer.WriteProperty(OpenApiConstants.Default, element.Default);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // enums
            writer.WriteOptionalCollection(OpenApiConstants.Enum, element.Enum, (w, s) => w.WriteValue(s));

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
