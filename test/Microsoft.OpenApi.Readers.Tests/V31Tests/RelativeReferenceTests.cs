using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class RelativeReferenceTests
    {
        private const string SampleFolderPath = "V31Tests/ReferenceSamples";

        [Fact]
        public async Task ParseInlineLocalReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "inlineLocalReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schemaType = actual.Paths["/item"].Operations[HttpMethod.Get].Parameters[0].Schema.Type;

            // Assert
            Assert.Equal(JsonSchemaType.Number, schemaType);
        }

        [Fact]
        public async Task ParseInlineExternalReferenceWorks()
        {
            // Arrange
            var expected = new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "thing",
                    ["description"] = "a thing"
                }
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "inlineExternalReference.yaml"), settings)).Document;
            var exampleValue = actual.Paths["/items"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Examples["item-list"].Value;

            // Assert
            Assert.NotNull(exampleValue);
            Assert.IsType<JsonArray>(exampleValue);
            Assert.Equal(expected.ToJsonString(), exampleValue.ToJsonString());
        }

        [Fact]
        public async Task ParseComponentExternalReferenceWorks()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "componentExternalReference.yaml"), settings)).Document;
            var securitySchemeValue = actual.Components.SecuritySchemes["customapikey"];

            // Assert
            Assert.Equal("x-api-key", securitySchemeValue.Name);
        }

        [Fact]
        public async Task ParseRootInlineJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "rootInlineSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/item"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseSubschemaInlineJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "subschemaInlineSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/items"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema.Items;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseRootComponentJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "rootComponentSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["specialitem"];

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
            Assert.Equal("Item", schema.Title);
        }

        [Fact]
        public async Task ParseSubschemaComponentJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "subschemaComponentSchemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["items"].Items;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseInternalComponentSubschemaJsonSchemaReferenceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "internalComponentsSubschemaReference.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/person/{id}/address"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseExternalComponentSubschemaJsonSchemaReferenceWorks()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalComponentSubschemaReference.yaml"), settings)).Document;
            var schema = actual.Paths["/person/{id}"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseReferenceToInternalComponentUsingDollarIdWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "internalComponentReferenceUsingId.yaml");

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Paths["/person/{id}"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, schema.Type);
        }

        [Fact]
        public async Task ParseLocalReferenceToJsonSchemaResourceWorks()
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "localReferenceToJsonSchemaResource.yaml");
            var stringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(stringWriter);

            // Act
            var actual = (await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings)).Document;
            var schema = actual.Components.Schemas["a"].Properties["b"].Properties["c"].Properties["b"];
            schema.SerializeAsV31(writer);
            var content = stringWriter.ToString();

            // Assert
            Assert.Equal(JsonSchemaType.Object | JsonSchemaType.Null, schema.Type);
        }
    }
}
