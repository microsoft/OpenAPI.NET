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
    public class V2OpenApiReferenceSerializer : IOpenApiElementSerializer<OpenApiReference>
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
                writer.WriteValue(ReferenceV2(element));
                return;
            }

            if (element.Type == ReferenceType.SecurityScheme)
            {
                // Write the string as property name
                writer.WritePropertyName(ReferenceV2(element));
                return;
            }

            writer.WriteStartObject();

            // $ref
            writer.WriteProperty(OpenApiConstants.DollarRef, ReferenceV2(element));

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets the full reference string for V2.0
        /// </summary>
        public string ReferenceV2(OpenApiReference reference)
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

                return "#/" + GetReferenceTypeNameAsV2(reference.Type.Value) + "/" + reference.Id;
        }

        

        private string GetReferenceTypeNameAsV2(ReferenceType type)
        {
            switch (type)
            {
                case ReferenceType.Schema:
                    return OpenApiConstants.Definitions;

                case ReferenceType.Parameter:
                    return OpenApiConstants.Parameters;

                case ReferenceType.Response:
                    return OpenApiConstants.Responses;

                case ReferenceType.Header:
                    return OpenApiConstants.Headers;

                case ReferenceType.Tag:
                    return OpenApiConstants.Tags;

                case ReferenceType.SecurityScheme:
                    return OpenApiConstants.SecurityDefinitions;

                default:
                    // If the reference type is not supported in V2, simply return null
                    // to indicate that the reference is not pointing to any object.
                    return null;
            }
        }
    }
}
