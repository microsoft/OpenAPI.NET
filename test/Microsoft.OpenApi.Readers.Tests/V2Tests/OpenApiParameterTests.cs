using System.Text.Json.Nodes;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Tests;
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
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "bodyParameter.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            // Body parameter is currently not translated via LoadParameter.
            // This design may be revisited and this unit test may likely change.
            Assert.Null(parameter);
        }

        [Fact]
        public void ParsePathParameterShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "pathParameter.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
                parameter);
        }

        [Fact]
        public void ParseQueryParameterShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "queryParameter.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    Style = ParameterStyle.Form,
                    Explode = true
                },
                parameter);
        }

        [Fact]
        public void ParseParameterWithNullLocationShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithNullLocation.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
                parameter);
        }

        [Fact]
        public void ParseParameterWithNoLocationShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithNoLocation.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
                parameter);
        }

        [Fact]
        public void ParseParameterWithNoSchemaShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithNoSchema.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = false
                },
                parameter);
        }

        [Fact]
        public void ParseParameterWithUnknownLocationShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithUnknownLocation.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
                parameter);
        }

        [Fact]
        public void ParseParameterWithDefaultShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithDefault.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));

            // Assert
            OpenApiTestAssert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float",
                        Default = new JsonNodeExtension(5).Node
                    }
                }, parameter, nameof(JsonNode.Parent));
        }

        [Fact]
        public void ParseParameterWithEnumShouldSucceed()
        {
            // Arrange
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "parameterWithEnum.yaml")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var parameter = OpenApiV2Deserializer.LoadParameter(node, new(), new ParsingContext(new()));
            var expected = new OpenApiParameter
            {
                In = ParameterLocation.Path,
                Name = "username",
                Description = "username to fetch",
                Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Number,
                    Format = "float",
                    Enum =
                    [
                        new JsonNodeExtension(7).Node,
                        new JsonNodeExtension(8).Node,
                        new JsonNodeExtension(9).Node
                    ]
                }
            };

            // Assert
            OpenApiTestAssert.Equivalent(expected, parameter, nameof(JsonNode.Parent));
        }

        [Fact]
        public void ParseFormDataParameterShouldSucceed()
        {
            // Arrange
            var expected = @"{
  ""type"": ""string"",
  ""description"": ""file to upload"",
  ""format"": ""binary""
}";
            JsonNode node;
            using (var stream = Resources.GetStream(Path.Join(SampleFolderPath, "formDataParameter.json")))
            {
                node = TestHelper.CreateYamlJsonNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new(), new ParsingContext(new()));
            var schema = operation.RequestBody?.Content["multipart/form-data"].Schema.Properties["file"];
            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiJsonWriter(writer));
            var json = writer.ToString();

            // Assert
            Assert.Equal("binary", schema.Format);
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), json.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
