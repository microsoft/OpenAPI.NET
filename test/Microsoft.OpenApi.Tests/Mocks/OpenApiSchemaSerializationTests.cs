using System.IO;
using System.Net.Http;
using Microsoft.OpenApi.Writers;
using Moq;
using Xunit;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public class OpenApiSchemaSerializationTests
    {
        private readonly OpenApiSchema _schema;
        private readonly Mock<OpenApiXml> _xmlMock = new() { CallBase = true };

        public OpenApiSchemaSerializationTests()
        {
            _schema = (OpenApiSchema)OpenApiDocumentMock.CreateCompleteOpenApiDocument().Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;
            _schema.Xml = _xmlMock.Object;
        }

        [Fact]
        public void SerializeAsV31_DoesNotCallV3OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _schema.SerializeAsV31(writer);

            // Assert - fail if V2 or V3 methods are called
            _xmlMock.Verify(c => c.SerializeAsV3(It.IsAny<IOpenApiWriter>()), Times.Never, "V3 method should not be called");
            _xmlMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
        }

        [Fact]
        public void SerializeAsV3_DoesNotCallV31OrV2Serialization()
        {
            // Arrange
            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);

            // Act
            _schema.SerializeAsV3(writer);

            // Assert - fail if V2 method is called
            _xmlMock.Verify(c => c.SerializeAsV2(It.IsAny<IOpenApiWriter>()), Times.Never, "V2 method should not be called");
            _xmlMock.Verify(c => c.SerializeAsV31(It.IsAny<IOpenApiWriter>()), Times.Never, "V31 method should not be called");
        }
    }
}
