using System.IO;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiMediaTypeSerializationTests
    {
        private readonly OpenApiMediaType _mediaType;
        private readonly Mock<OpenApiSchema> _schemaMock = new() { CallBase = true };
        private readonly Mock<OpenApiEncoding> _encodingMock = new() { CallBase = true };
        private readonly Mock<OpenApiExample> _exampleMock = new() { CallBase = true };

        public OpenApiMediaTypeSerializationTests()
        {
            _mediaType = (OpenApiMediaType)OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"];
            _mediaType.Schema = _schemaMock.Object;
            _mediaType.Examples["cat"] = _exampleMock.Object;
            _mediaType.Examples["example"] = _exampleMock.Object;
            _mediaType.Encoding["encoding"] = _encodingMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _mediaType.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _schemaMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _schemaMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _encodingMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _encodingMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _exampleMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _exampleMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _mediaType.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _schemaMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _schemaMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _encodingMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _encodingMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _exampleMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _exampleMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
