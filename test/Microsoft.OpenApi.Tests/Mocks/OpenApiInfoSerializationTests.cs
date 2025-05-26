using System.IO;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiInfoSerializationTests
    {
        private readonly OpenApiInfo _info;
        private readonly Mock<OpenApiContact> _contactMock = new() { CallBase = true };
        private readonly Mock<OpenApiLicense> _licenseMock = new() { CallBase = true };

        public OpenApiInfoSerializationTests()
        {
            _info = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Info;
            _info.Contact = _contactMock.Object;
            _info.License = _licenseMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _info.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _contactMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _contactMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _licenseMock.Verify(l => l.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _licenseMock.Verify(l => l.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _info.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _contactMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _contactMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _licenseMock.Verify(l => l.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _licenseMock.Verify(l => l.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
