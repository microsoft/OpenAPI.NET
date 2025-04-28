using System.IO;
using System.Net.Http;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiPathItemSerializationTests
    {
        private static readonly IOpenApiPathItem _pathItem = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"];
        private static readonly Mock<OpenApiOperation> _operationMock = new() { CallBase = true };
        private static readonly Mock<OpenApiServer> _serverMock = new() { CallBase = true };
        private static readonly Mock<OpenApiParameter> _parameterMock = new() { CallBase = true };

        public OpenApiPathItemSerializationTests()
        {
            _pathItem.Operations[HttpMethod.Get] = _operationMock.Object;
            _pathItem.Servers[0] = _serverMock.Object ;
            _pathItem.Parameters[0] = _parameterMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _pathItem.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _operationMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _operationMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _serverMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _serverMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _parameterMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _parameterMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _pathItem.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _operationMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _operationMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _serverMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _serverMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _parameterMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _parameterMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
