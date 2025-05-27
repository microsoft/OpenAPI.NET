using System.IO;
using System.Net.Http;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiCallbackSerializationTests
    {
        private readonly OpenApiCallback _callback;
        private readonly Mock<OpenApiPathItem> _pathItemMock = new() { CallBase = true };

        public OpenApiCallbackSerializationTests()
        {
            _callback = (OpenApiCallback)OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].Callbacks["onData"];
            _callback.PathItems[RuntimeExpression.Build("{$request.body#/callbackUrl}")] = _pathItemMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _callback.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _pathItemMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _pathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _callback.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _pathItemMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _pathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
