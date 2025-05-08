using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiTagsSerialization
    {
        private readonly OpenApiTag _tag;
        private readonly Mock<OpenApiExternalDocs> _externalDocsMock = new() { CallBase = true };

        public OpenApiTagsSerialization()
        {
            _tag = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Tags.ToList()[0];
            _tag.ExternalDocs = _externalDocsMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _tag.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _externalDocsMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _externalDocsMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _tag.SerializeAsV3(writer);

            // Assert - fail if V2 method is called
            _externalDocsMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
            _externalDocsMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
        }
    }
}
