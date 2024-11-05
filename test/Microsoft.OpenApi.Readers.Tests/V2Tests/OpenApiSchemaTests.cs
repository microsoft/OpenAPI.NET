// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Reader.V2;
using Xunit;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using FluentAssertions.Equivalency;

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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

            // Assert
            var expected = new OpenApiSchema
            {
                Type = JsonSchemaType.Number,
                Format = "float",
                Enum = new List<JsonNode>
                {
                    new OpenApiAny(7).Node,
                    new OpenApiAny(8).Node,
                    new OpenApiAny(9).Node
                }
            };

            schema.Should().BeEquivalentTo(expected, options =>
                       options.IgnoringCyclicReferences()
                              .Excluding((IMemberInfo memberInfo) =>
                                    memberInfo.Path.EndsWith("Parent")));
        }
    }
}
