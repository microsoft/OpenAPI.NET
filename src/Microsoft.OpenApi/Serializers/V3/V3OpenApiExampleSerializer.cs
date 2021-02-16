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
    public class V3OpenApiExampleSerializer : ReferenceOpenApiSerializer<OpenApiExample>
    {
        public V3OpenApiExampleSerializer(IOpenApiElementSerializer<OpenApiReference> referenceSerializer) : base(referenceSerializer)
        {
        }

        public override void SerializeWithoutReference(OpenApiExample element, IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, element.Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // value
            writer.WriteOptionalObject(OpenApiConstants.Value, element.Value, (w, v) => w.WriteAny(v));

            // externalValue
            writer.WriteProperty(OpenApiConstants.ExternalValue, element.ExternalValue);

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
