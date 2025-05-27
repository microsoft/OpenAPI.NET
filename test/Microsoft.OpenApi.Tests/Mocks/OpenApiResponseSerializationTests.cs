using System.IO;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiResponseSerializationTests
    {
        private readonly IOpenApiResponse _response;
        private readonly Mock<OpenApiHeader> _headerMock = new() { CallBase = true };
        private readonly Mock<OpenApiMediaType> _mediaTypeMock = new() { CallBase = true };
        private readonly Mock<OpenApiLink> _linkMock = new() { CallBase = true };

        public OpenApiResponseSerializationTests()
        {
            _response = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].Responses["200"];
            _response.Headers["x-rate-limit"] = _headerMock.Object;
            _response.Content["application/json"] = _mediaTypeMock.Object;
            _response.Links["UserRepositories"] = _linkMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _response.SerializeAsV31(writer);

            // Assert
            _headerMock.Verify(h => h.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _headerMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _mediaTypeMock.Verify(m => m.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _mediaTypeMock.Verify(m => m.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _linkMock.Verify(l => l.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _linkMock.Verify(l => l.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _response.SerializeAsV3(writer);

            // Assert
            _headerMock.Verify(h => h.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _headerMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _mediaTypeMock.Verify(m => m.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _mediaTypeMock.Verify(m => m.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _linkMock.Verify(l => l.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _linkMock.Verify(l => l.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);
        }
    }
}
