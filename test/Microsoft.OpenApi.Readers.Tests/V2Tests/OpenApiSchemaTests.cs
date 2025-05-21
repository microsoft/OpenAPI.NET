// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Writers;
using Xunit;

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
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schemaWithDefault.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new());

            // Assert
            schema.Should().BeEquivalentTo(new OpenApiSchema
            {
                Type = JsonSchemaType.Number,
                Format = "float",
                Default = 5
            }, options => options.IgnoringCyclicReferences().Excluding(x => x.Default.Parent));
        }

        [Fact]
        public void ParseSchemaWithExampleShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schemaWithExample.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new());

            // Assert
            schema.Should().BeEquivalentTo(
                new OpenApiSchema
                {
                    Type = JsonSchemaType.Number,
                    Format = "float",
                    Example = 5
                }, options => options.IgnoringCyclicReferences().Excluding(x => x.Example.Parent));
        }

        [Fact]
        public void ParseSchemaWithEnumShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schemaWithEnum.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var schema = OpenApiV2Deserializer.LoadSchema(node, new());

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

            schema.Should().BeEquivalentTo(expected, options =>
                       options.IgnoringCyclicReferences()
                              .Excluding((IMemberInfo memberInfo) =>
                                    memberInfo.Path.EndsWith("Parent")));
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

            var path = Path.Combine(SampleFolderPath, "schemaWithNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new(), SettingsFixture.ReaderSettings);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeSchemaWithOnlyNullableShouldSucceed()
        {
            // Arrange
            var expected = @"x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithOnlyNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new(), SettingsFixture.ReaderSettings);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
