// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
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
            var schema = OpenApiV2Deserializer.LoadSchema(node);

            // Assert
            schema.Should().BeEquivalentTo(
                new OpenApiSchema
                {
                    Type = "number",
                    Format = "float",
                    Default = new OpenApiAny(5)
                }, options => options.IgnoringCyclicReferences()
                .Excluding(schema => schema.Default.Node.Parent));
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
                    Type = "number",
                    Format = "float",
                    Example = new OpenApiAny(5)
                }, options => options.IgnoringCyclicReferences()
                .Excluding(schema => schema.Example.Node.Parent));
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
            schema.Should().BeEquivalentTo(
                new OpenApiSchema
                {
                    Type = "number",
                    Format = "float",
                    Enum = 
                    { 
                        new OpenApiAny(7), 
                        new OpenApiAny(8), 
                        new OpenApiAny(9)
                    }                    
                }, options => options.IgnoringCyclicReferences()
                .Excluding(s => s.Enum[0].Node.Parent)
                .Excluding(s => s.Enum[1].Node.Parent)
                .Excluding(s => s.Enum[2].Node.Parent));
        }
    }
}
