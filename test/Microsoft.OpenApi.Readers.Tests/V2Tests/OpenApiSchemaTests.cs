// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Tests;
using Xunit;

#pragma warning disable CS0618

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiSchema/";

        [Fact]
        public void ParseSchemaWithDefaultShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "schemaWithDefault.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new(), new ParsingContext(new()));

            // Assert
            OpenApiTestAssert.Equivalent(new OpenApiSchema
            {
                Type = JsonSchemaType.Number,
                Format = "float",
                Default = 5
            }, schema, nameof(JsonNode.Parent));
        }

        [Fact]
        public void ParseSchemaWithExampleShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "schemaWithExample.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new(), new ParsingContext(new()));

            // Assert
            OpenApiTestAssert.Equivalent(
                new OpenApiSchema
                {
                    Type = JsonSchemaType.Number,
                    Format = "float",
                    Example = 5
                }, schema, nameof(JsonNode.Parent));
        }

        [Fact]
        public void ParseSchemaWithEnumShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "schemaWithEnum.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new(), new ParsingContext(new()));

            // Assert
            var expected = new OpenApiSchema
            {
                Type = JsonSchemaType.Number,
                Format = "float",
                Enum =
                [
                    new JsonNodeExtension(7).Node,
                    new JsonNodeExtension(8).Node,
                    new JsonNodeExtension(9).Node
                ]
            };

            OpenApiTestAssert.Equivalent(expected, schema, nameof(JsonNode.Parent));
        }

        [Fact]
        public void PropertiesReferenceShouldWork()
        {
            var workingDocument = new OpenApiDocument()
            {
                Components = new OpenApiComponents(),
            };
            const string referenceId = "targetSchema";
            var targetSchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["prop1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };
            workingDocument.Components.Schemas = new Dictionary<string, IOpenApiSchema>()
            {
                [referenceId] = targetSchema
            };
            workingDocument.Workspace.RegisterComponents(workingDocument);
            var referenceSchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["propA"] = new OpenApiSchemaReference(referenceId, workingDocument),
                }
            };

            using var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);
            referenceSchema.SerializeAsV2(writer);

            var json = textWriter.ToString();
            var expected = JsonNode.Parse(
                """
                {
                    "type": "object",
                    "properties":
                    {
                        "propA":
                        {
                            "$ref": "#/definitions/targetSchema"
                        }
                    }
                }
                """
            );
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(json), expected));
        }

        [Fact]
        public async Task SerializeSchemaWithNullableShouldSucceed()
        {
            // Arrange
            var expected = @"type: string
x-nullable: true";

            var path = Path.Join(SampleFolderPath, "schemaWithNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new(), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeSchemaWithOnlyNullableShouldSucceed()
        {
            // NOTE: x-nullable extension has no effect on the schema if type is not specified, so it is omitted in the serialized output.

            // Arrange
            var expected = @"{ }";

            var path = Path.Join(SampleFolderPath, "schemaWithOnlyNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new(), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        private static OpenApiSchema LoadV2Schema(string json)
            => Assert.IsType<OpenApiSchema>(
                OpenApiV2Deserializer.LoadSchema(JsonNode.Parse(json), new(), new ParsingContext(new())));

        private static string SerializeAsV2(OpenApiSchema schema)
        {
            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiJsonWriter(writer));
            return writer.ToString();
        }

        // The object model represents the latest version of the spec, so v2 binary descriptions
        // are normalized on read and reconstructed on write.
        // https://spec.openapis.org/oas/v3.2.0.html#migrating-binary-descriptions-from-oas-3-0
        [Fact]
        public void ParseSchemaWithByteFormatNormalizesToContentEncoding()
        {
            var schema = LoadV2Schema("""{ "type": "string", "format": "byte" }""");

            Assert.Equal(JsonSchemaType.String, schema.Type);
            Assert.Equal("base64", schema.ContentEncoding);
            Assert.Null(schema.Format);
        }

        [Fact]
        public void ParseSchemaWithBinaryFormatNormalizesToContentMediaType()
        {
            var schema = LoadV2Schema("""{ "type": "string", "format": "binary" }""");

            Assert.Null(schema.Type);
            Assert.Equal("application/octet-stream", schema.ContentMediaType);
            Assert.Null(schema.Format);
        }

        [Fact]
        public void ParseSchemaWithContentEncodingExtensionAssignsContentProperties()
        {
            var schema = LoadV2Schema("""
                {
                  "type": "string",
                  "x-jsonschema-contentEncoding": "base64",
                  "x-jsonschema-contentMediaType": "image/png",
                  "x-jsonschema-contentSchema": { "type": "array" }
                }
                """);

            Assert.Equal("base64", schema.ContentEncoding);
            Assert.Equal("image/png", schema.ContentMediaType);
            Assert.Equal(JsonSchemaType.Array, schema.ContentSchema?.Type);
            Assert.Empty(schema.Extensions ?? new Dictionary<string, IOpenApiExtension>());
        }

        [Theory]
        [InlineData("""{ "type": "string", "format": "byte" }""")]
        [InlineData("""{ "type": "string", "format": "binary" }""")]
        public void SchemaWithBinaryDescriptionRoundTripsThroughV2(string original)
        {
            var schema = LoadV2Schema(original);

            var serialized = SerializeAsV2(schema);
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(original), JsonNode.Parse(serialized)));

            // Reading our own output must produce an equivalent model.
            var reparsed = LoadV2Schema(serialized);
            Assert.Equal(schema.Type, reparsed.Type);
            Assert.Equal(schema.Format, reparsed.Format);
            Assert.Equal(schema.ContentEncoding, reparsed.ContentEncoding);
            Assert.Equal(schema.ContentMediaType, reparsed.ContentMediaType);
        }
    }
}
