// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class JsonSchemaTests
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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

            // Assert
            schema.Should().BeEquivalentTo(new JsonSchemaBuilder()
                                .Type(SchemaValueType.Number).Format("float").Default(5).Build(),
                                options => options.IgnoringCyclicReferences());
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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

            // Assert
            schema.Should().BeEquivalentTo(
                new JsonSchemaBuilder()
                    .Type(SchemaValueType.Number)
                    .Format("float")
                    .Example(5)
                    .Build(),
                options => options.IgnoringCyclicReferences());
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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

            // Assert
            var expected = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Number)
                                .Format("float")
                                .Enum(7, 8, 9)
                                .Build();
            schema.Should().BeEquivalentTo(expected,
                                options => options.IgnoringCyclicReferences());
        }
    }
}
