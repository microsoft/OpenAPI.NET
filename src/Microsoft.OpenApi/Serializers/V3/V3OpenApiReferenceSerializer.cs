using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiReferenceSerializer : IOpenApiElementSerializer<OpenApiReference>
    {
        public void Serialize(OpenApiReference element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (element.Type == ReferenceType.Tag)
            {
                // Write the string value only
                writer.WriteValue(ReferenceV3(element));
                return;
            }

            if (element.Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV3(element));
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV3(element));

            writer.WriteEndObject();
        }

        public string ReferenceV3(OpenApiReference reference)
        {
            if (reference.IsExternal)
            {
                return reference.GetExternalReference();
            }

            if (!reference.Type.HasValue)
            {
                throw Error.ArgumentNull(nameof(Type));
            }

            if (reference.Type == ReferenceType.Tag)
            {
                return reference.Id;
            }

            if (reference.Type == ReferenceType.SecurityScheme)
            {
                return reference.Id;
            }

            return "#/components/" + reference.Type.GetDisplayName() + "/" + reference.Id;
        }
    }
}
