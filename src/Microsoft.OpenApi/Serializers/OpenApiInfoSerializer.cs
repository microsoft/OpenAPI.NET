using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public class OpenApiInfoSerializer : IOpenApiElementSerializer<OpenApiInfo>
    {
        private readonly IOpenApiSpecProvider _specProvider;

        private readonly IOpenApiElementSerializer<OpenApiContact> _contactSerializer;

        private readonly IOpenApiElementSerializer<OpenApiLicense> _licenseSerializer;

        public OpenApiInfoSerializer(
            IOpenApiElementSerializer<OpenApiContact> contactSerializer,
            IOpenApiElementSerializer<OpenApiLicense> licenseSerializer,
            IOpenApiSpecProvider specProvider)
        {
            _specProvider = specProvider;
            _contactSerializer = contactSerializer;
            _licenseSerializer = licenseSerializer;
        }

        public void Serialize(OpenApiInfo element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, element.Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // termsOfService
            writer.WriteProperty(OpenApiConstants.TermsOfService, element.TermsOfService?.OriginalString);

            // contact object
            writer.WriteOptionalObject(OpenApiConstants.Contact, element.Contact, (w, c) => _contactSerializer.Serialize(c, w));

            // license object
            writer.WriteOptionalObject(OpenApiConstants.License, element.License, (w, l) => _licenseSerializer.Serialize(l, w));

            // version
            writer.WriteProperty(OpenApiConstants.Version, element.Version);

            // specification extensions
            writer.WriteExtensions(element.Extensions, _specProvider.OpenApiSpec);

            writer.WriteEndObject();
        }
    }
}
