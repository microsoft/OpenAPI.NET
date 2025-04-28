using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiSecuritySchemeSerializationTests
    {
        private static readonly OpenApiSecurityScheme _securityScheme = (OpenApiSecurityScheme)OpenApiDocumentMock.CreateCompleteOpenApiDocument().Components.SecuritySchemes["api_key"];
        private static readonly Mock<OpenApiOAuthFlows> _authFlowMock = new() { CallBase = true };

        public OpenApiSecuritySchemeSerializationTests()
        {
            _securityScheme.Flows = _authFlowMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _securityScheme.SerializeAsV31(writer);

            // Assert
            _authFlowMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _authFlowMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _securityScheme.SerializeAsV3(writer);

            // Assert
            _authFlowMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _authFlowMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
