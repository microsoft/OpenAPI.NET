using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiDocumentSerializationTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiDocument/";

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task Serialize_DoesNotMutateDom(OpenApiSpecVersion version)
        {
            // Arrange
            var filePath = Path.Combine(SampleFolderPath, "docWith31properties.json");
            var (doc, _) = await OpenApiDocument.LoadAsync(filePath, SettingsFixture.ReaderSettings);

            // Act: Serialize using System.Text.Json
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new HttpMethodOperationDictionaryConverter()
                },
            };
            var originalSerialized = JsonSerializer.Serialize(doc, options);
            Assert.NotNull(originalSerialized); // sanity check

            // Serialize using native OpenAPI writer
            var jsonWriter = new StringWriter();
            var openApiWriter = new OpenApiJsonWriter(jsonWriter);
            switch (version)
            {
                case OpenApiSpecVersion.OpenApi3_1:
                    doc.SerializeAsV31(openApiWriter);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    doc.SerializeAsV3(openApiWriter);
                    break;
                default:
                    doc.SerializeAsV2(openApiWriter);
                    break;
            }

            // Serialize again with STJ after native writer serialization
            var finalSerialized = JsonSerializer.Serialize(doc, options);
            Assert.NotNull(finalSerialized); // sanity check

            // Assert: Ensure no mutation occurred in the DOM after native serialization
            Assert.True(JsonNode.DeepEquals(originalSerialized, finalSerialized), "OpenAPI DOM was mutated by the native serializer.");
        }
    }

    public class HttpMethodOperationDictionaryConverter : JsonConverter<Dictionary<HttpMethod, OpenApiOperation>>
    {
        public override Dictionary<HttpMethod, OpenApiOperation> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<HttpMethod, OpenApiOperation> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key.Method.ToLowerInvariant());
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}
