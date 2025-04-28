using System.IO;
using System.Net.Http;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiHeaderSerializationTests
    {
        private static readonly OpenApiHeader _header = (OpenApiHeader)OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Headers["x-rate-limit"];
        private static readonly Mock<OpenApiSchema> _schemaMock = new() { CallBase = true };
        private static readonly Mock<OpenApiExample> _exampleMock = new() { CallBase = true };
        private static readonly Mock<OpenApiMediaType> _mediaTypeMock = new() { CallBase = true };

        public OpenApiHeaderSerializationTests()
        {
            _header.Schema = _schemaMock.Object;
            _header.Examples["cat"] = _exampleMock.Object;
            _header.Content["application/json"] = _mediaTypeMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _header.SerializeAsV31(writer);

            // Assert
            _schemaMock.Verify(h => h.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _schemaMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _exampleMock.Verify(h => h.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _exampleMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _mediaTypeMock.Verify(h => h.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never);
            _mediaTypeMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _header.SerializeAsV3(writer);

            // Assert
            _schemaMock.Verify(h => h.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _schemaMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _exampleMock.Verify(h => h.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _exampleMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);

            _mediaTypeMock.Verify(h => h.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never);
            _mediaTypeMock.Verify(h => h.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never);
        }
    }
}
