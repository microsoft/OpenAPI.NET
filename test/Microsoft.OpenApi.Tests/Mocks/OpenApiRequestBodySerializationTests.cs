using System.IO;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiRequestBodySerializationTests
    {
        private readonly IOpenApiRequestBody _requestBody;
        private readonly Mock<OpenApiMediaType> _mediaTypeMock = new() { CallBase = true };

        public OpenApiRequestBodySerializationTests()
        {
            _requestBody = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].RequestBody;
            _requestBody.Content["application/json"] = _mediaTypeMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _requestBody.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _mediaTypeMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _mediaTypeMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _requestBody.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _mediaTypeMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _mediaTypeMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
