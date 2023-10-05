// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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
                    Schema = new()
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
                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
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

                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Type = "integer",
                            Format = "int64",
                            Enum = new List<IOpenApiAny>
                            {
                                new OpenApiLong(1),
                                new OpenApiLong(2),
                                new OpenApiLong(3),
                                new OpenApiLong(4),
                            }
                        },
                        Default = new OpenApiArray
                        {
                            new OpenApiLong(1),
                            new OpenApiLong(2)
                        },
                        Enum = new List<IOpenApiAny>
                        {
                            new OpenApiArray { new OpenApiLong(1), new OpenApiLong(2) },
                            new OpenApiArray { new OpenApiLong(2), new OpenApiLong(3) },
                            new OpenApiArray { new OpenApiLong(3), new OpenApiLong(4) }
                        }
                    }
                });
        }

        [Fact]
        public void ParseHeaderParameterWithIncorrectDataTypeShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerParameterWithIncorrectDataType.yaml")))
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

                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Type = "string",
                            Format = "date-time",
                            Enum = new List<IOpenApiAny>
                            {
                                new OpenApiString("1"),
                                new OpenApiString("2"),
                                new OpenApiString("3"),
                                new OpenApiString("4"),
                            }
                        },
                        Default = new OpenApiArray
                        {
                            new OpenApiString("1"),
                            new OpenApiString("2")
                        },
                        Enum = new List<IOpenApiAny>
                        {
                            new OpenApiArray { new OpenApiString("1"), new OpenApiString("2") },
                            new OpenApiArray { new OpenApiString("2"), new OpenApiString("3") },
                            new OpenApiArray { new OpenApiString("3"), new OpenApiString("4") }
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
            var parameter = OpenApiV2Deserializer.LoadParameter(node);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new()
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
                    Schema = new()
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
                    Schema = new()
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
                    Schema = new()
                    {
                        Type = "number",
                        Format = "float",
                        Default = new OpenApiFloat(5)
                    }
                });
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
                    Schema = new()
                    {
                        Type = "number",
                        Format = "float",
                        Enum =
                        {
                            new OpenApiFloat(7),
                            new OpenApiFloat(8),
                            new OpenApiFloat(9)
                        }
                    }
                });
        }
    }
}
