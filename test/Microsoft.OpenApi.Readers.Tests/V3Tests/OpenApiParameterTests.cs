// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiParameter/";

        [Fact]
        public void ParsePathParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "pathParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseQueryParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "queryParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String)),
                    Style = ParameterStyle.Form,
                    Explode = true
                });
        }

        [Fact]
        public void ParseQueryParameterWithObjectTypeShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "queryParameterWithObjectType.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "freeForm",
                    Schema31 = new JsonSchemaBuilder()
                    .Type(SchemaValueType.Object)
                    .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Integer)),
                    Style = ParameterStyle.Form
                });
        }

        [Fact]
        public void ParseQueryParameterWithObjectTypeAndContentShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "queryParameterWithObjectTypeAndContent.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "coordinates",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema31 = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("lat", "long")
                                .Properties(
                                    ("lat", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Number)
                                    ),
                                    ("long", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Number)
                                    )
                                )
                        }
                    }
                });
        }

        [Fact]
        public void ParseHeaderParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "token",
                    Description = "token to be passed as a header",
                    Required = true,
                    Style = ParameterStyle.Simple,

                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(new JsonSchemaBuilder()
                            .Type(SchemaValueType.Integer)
                            .Format("int64"))
                });
        }

        [Fact]
        public void ParseParameterWithNullLocationShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithNullLocation.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithNoLocationShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithNoLocation.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithUnknownLocationShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithUnknownLocation.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithExampleShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithExample.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Example = new OpenApiAny((float)5.0),
                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Number)
                        .Format("float")
                }, options => options.IgnoringCyclicReferences().Excluding(p => p.Example.Node.Parent));
        }
        
        [Fact]
        public void ParseParameterWithExamplesShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Examples =
                    {
                        ["example1"] = new OpenApiExample()
                        {
                            Value = new OpenApiAny(5.0)
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value = new OpenApiAny((float)7.5)
                        }
                    },
                    Schema31 = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Number)
                        .Format("float")
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Examples["example1"].Value.Node.Parent)
                .Excluding(p => p.Examples["example2"].Value.Node.Parent));
        }
    }
}
