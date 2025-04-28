using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiComponentsSerializationTests
    {
        private static readonly OpenApiComponents _components = OpenApiDocumentMock.CreateCompleteOpenApiDocument().Components;
        private static readonly Mock<OpenApiSchema> _schemaMock = new() { CallBase = true };
        private static readonly Mock<OpenApiRequestBody> _requestBodyMock = new() { CallBase = true };
        private static readonly Mock<OpenApiResponse> _responseMock = new() { CallBase = true };
        private static readonly Mock<OpenApiParameter> _parameterMock = new() { CallBase = true };
        private static readonly Mock<OpenApiHeader> _headerMock = new() { CallBase = true };
        private static readonly Mock<OpenApiSecurityScheme> _securitySchemeMock = new() { CallBase = true };
        private static readonly Mock<OpenApiLink> _linkMock = new() { CallBase = true };
        private static readonly Mock<OpenApiCallback> _callbackMock = new() { CallBase = true };
        private static readonly Mock<OpenApiExample> _exampleMock = new() { CallBase = true };
        private static readonly Mock<OpenApiPathItem> _pathItemMock = new() { CallBase = true };

        public OpenApiComponentsSerializationTests()
        {
            _components.Schemas["pet"] = _schemaMock.Object;
            _components.RequestBodies["pet"] = _requestBodyMock.Object;
            _components.Responses["200"] = _responseMock.Object;
            _components.Parameters["limit"] = _parameterMock.Object;
            _components.Headers["x-rate-limit"] = _headerMock.Object;
            _components.SecuritySchemes["api_key"] = _securitySchemeMock.Object;
            _components.Links["UserRepositories"] = _linkMock.Object;
            _components.Callbacks["onData"] = _callbackMock.Object;
            _components.Examples["cat"] = _exampleMock.Object;
            _components.PathItems["/pets"] = _pathItemMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _components.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _schemaMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _schemaMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _requestBodyMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _requestBodyMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _responseMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _responseMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _parameterMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _parameterMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _headerMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _headerMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _securitySchemeMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _securitySchemeMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _linkMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _linkMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _callbackMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _callbackMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _exampleMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _exampleMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

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
            _components.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _schemaMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _schemaMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _requestBodyMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _requestBodyMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _responseMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _responseMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _parameterMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _parameterMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _headerMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _headerMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _securitySchemeMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _securitySchemeMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _linkMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _linkMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _callbackMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _callbackMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _exampleMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _exampleMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _pathItemMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _pathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
