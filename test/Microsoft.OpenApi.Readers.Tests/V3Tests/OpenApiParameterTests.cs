// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
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
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

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
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalProperties = new OpenApiSchema
                        {
                            Type = "integer"
                        }
                    },
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
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Required =
                                {
                                    "lat",
                                    "long"
                                },
                                Properties =
                                {
                                    ["lat"] = new OpenApiSchema
                                    {
                                        Type = "number"
                                    },
                                    ["long"] = new OpenApiSchema
                                    {
                                        Type = "number"
                                    }
                                }
                            }
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

                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int64",
                        }
                    }
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
            var parameter = OpenApiV3Deserializer.LoadParameter(node);

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
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
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
                    Example = new OpenApiFloat(5),
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float"
                    }
                });
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
                        ["example1"] = new OpenApiExample
                        {
                            Value = new OpenApiFloat(5),
                        },
                        ["example2"] = new OpenApiExample
                        {
                            Value = new OpenApiFloat((float)7.5),
                        }
                    },
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float"
                    }
                });
        }
    }
}
