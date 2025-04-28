using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiDocumentSerializationTests
    {
        // test for PathItems, servers
        private static readonly OpenApiDocument _document = OpenApiDocumentMock.CreateCompleteOpenApiDocument();
        private static readonly Mock<OpenApiPathItem> _pathItemMock = new() { CallBase = true };
        private static readonly Mock<OpenApiPathItem> _webhookPathItemMock = new() { CallBase = true };
        private static readonly Mock<OpenApiServer> _serverMock = new() { CallBase = true };
        private static readonly Mock<OpenApiTag> _tagMock = new() { CallBase = true };
        private static readonly Mock<OpenApiSecurityRequirement> _securityMock = new() { CallBase = true };
        private static readonly Mock<OpenApiComponents> _componentsMock = new() { CallBase = true };

        public OpenApiDocumentSerializationTests()
        {
            _document.Paths["/pets"] = _pathItemMock.Object;
            _document.Webhooks["pets"] = _webhookPathItemMock.Object;
            _document.Servers[0] = _serverMock.Object;
            _document.Tags.ToList()[0] = _tagMock.Object;
            _document.Security[0] = _securityMock.Object;
            _document.Components = _componentsMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _document.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _pathItemMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _pathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _webhookPathItemMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _webhookPathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _tagMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _tagMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _serverMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _serverMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _securityMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _securityMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _componentsMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _componentsMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _document.SerializeAsV3(writer);

            // Assert - fail if V2 or V3 methods are called
            _pathItemMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _pathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _serverMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _serverMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _webhookPathItemMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _webhookPathItemMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _tagMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _tagMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _securityMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _securityMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");

            _componentsMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
            _componentsMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }
    }
}
