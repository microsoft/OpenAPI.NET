// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiParameter/";

        [Fact]
        public void ParseBodyParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "bodyParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            // Body parameter is currently not translated via LoadParameter.
            // This design may be revisited and this unit test may likely change.
            parameter.Should().BeNull();
        }

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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    },
                    Style = ParameterStyle.Form,
                    Explode = true
                });
        }

        [Fact]
        public void ParseFormDataParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "formDataParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            // Form data parameter is currently not translated via LoadParameter.
            // This design may be revisited and this unit test may likely change.
            parameter.Should().BeNull();
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "token",
                    Description = "token to be passed as a header",
                    Required = true,
                    Style = ParameterStyle.Simple,

                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int64",
                            Enum = new List<OpenApiAny>
                            {
                                new OpenApiAny(1),
                                new OpenApiAny(2),
                                new OpenApiAny(3),
                                new OpenApiAny(4)
                            }
                        },
                        Default = new OpenApiAny(new JsonArray() {
                            1,
                            2
                        }),
                        Enum = new List<OpenApiAny>
                        {
                            new OpenApiAny(new JsonArray() { 1, 2 }),
                            new OpenApiAny(new JsonArray() { 2, 3 }),
                            new OpenApiAny(new JsonArray() { 3, 4 })
                        }
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Schema.Default.Node[0].Root)
                .Excluding(p => p.Schema.Default.Node[0].Parent)
                .Excluding(p => p.Schema.Default.Node[1].Parent)
                .Excluding(p => p.Schema.Default.Node[1].Root)
                .Excluding(p => p.Schema.Items.Enum[0].Node.Parent)
                .Excluding(p => p.Schema.Items.Enum[1].Node.Parent)
                .Excluding(p => p.Schema.Items.Enum[2].Node.Parent)
                .Excluding(p => p.Schema.Items.Enum[3].Node.Parent)
                .Excluding(p => p.Schema.Enum[0].Node[0].Parent)
                .Excluding(p => p.Schema.Enum[0].Node[0].Root)
                .Excluding(p => p.Schema.Enum[0].Node[1].Parent)
                .Excluding(p => p.Schema.Enum[0].Node[1].Root)
                .Excluding(p => p.Schema.Enum[1].Node[0].Parent)
                .Excluding(p => p.Schema.Enum[1].Node[0].Root)
                .Excluding(p => p.Schema.Enum[1].Node[1].Parent)
                .Excluding(p => p.Schema.Enum[1].Node[1].Root)
                .Excluding(p => p.Schema.Enum[2].Node[0].Parent)
                .Excluding(p => p.Schema.Enum[2].Node[0].Root)
                .Excluding(p => p.Schema.Enum[2].Node[1].Parent)
                .Excluding(p => p.Schema.Enum[2].Node[1].Root)
                );
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
        }

        [Fact]
        public void ParseParameterWithNoSchemaShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithNoSchema.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = false
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
        }

        [Fact]
        public void ParseParameterWithDefaultShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithDefault.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float",
                        Default = new OpenApiAny(5)
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Schema.Default.Node.Parent));
        }

        [Fact]
        public void ParseParameterWithEnumShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithEnum.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float",
                        Enum = 
                        { 
                            new OpenApiAny(7),
                            new OpenApiAny(8),
                            new OpenApiAny(9)
                        }
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Schema.Enum[0].Node.Parent)
                .Excluding(p => p.Schema.Enum[1].Node.Parent)
                .Excluding(p => p.Schema.Enum[2].Node.Parent));
        }
    }
}
