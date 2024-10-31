// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Reader.V3;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiOperationTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiOperation/";

        private static readonly OpenApiOperation _basicOperation = new OpenApiOperation
        {
            Summary = "Updates a pet in the store",
            Description = "",
            OperationId = "updatePet",
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["application/xml"] = new OpenApiMediaType()
                    }
                }
            }
        };

        private static readonly OpenApiOperation _operationWithFormData =
            new OpenApiOperation
            {
                Summary = "Updates a pet in the store with form data",
                Description = "",
                OperationId = "updatePetWithForm",
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "petId",
                        In = ParameterLocation.Path,
                        Description = "ID of pet that needs to be updated",
                        Required = true,
                        Schema = new()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                        {
                            Schema = new()
                            {
                                Type = JsonSchemaType.Object,
                                Properties =
                                {
                                    ["name"] = new()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = JsonSchemaType.String
                                    },
                                    ["status"] = new()
                                    {
                                        Description = "Updated status of the pet",
                                        Type = JsonSchemaType.String
                                    }
                                },
                                Required = new HashSet<string>
                                {
                                    "name"
                                }
                            }
                        },
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                             Schema = new()
                            {
                                Type = JsonSchemaType.Object,
                                Properties =
                                {
                                    ["name"] = new()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = JsonSchemaType.String
                                    },
                                    ["status"] = new()
                                    {
                                        Description = "Updated status of the pet",
                                        Type = JsonSchemaType.String
                                    }
                                },
                                Required = new HashSet<string>
                                {
                                    "name"
                                }
                            }
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Pet updated.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType(),
                            ["application/xml"] = new OpenApiMediaType()
                        }

                    },
                    ["405"] = new OpenApiResponse
                    {
                        Description = "Invalid input",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType(),
                            ["application/xml"] = new OpenApiMediaType()
                        }
                    }
                }
            };

        private static readonly OpenApiOperation _operationWithBody = new OpenApiOperation
        {
            Summary = "Updates a pet in the store with request body",
            Description = "",
            OperationId = "updatePetWithBody",
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new()
                    {
                        Type = JsonSchemaType.String
                    }
                },
            },
            RequestBody = new OpenApiRequestBody
            {
                Description = "Pet to update with",
                Required = true,
                Content =
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new()
                        {
                            Type = JsonSchemaType.Object
                        }
                    }
                },
                Extensions = {
                    [OpenApiConstants.BodyName] = new OpenApiAny("petObject")
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["application/xml"] = new OpenApiMediaType()
                    }
                },
                ["405"] = new OpenApiResponse
                {
                    Description = "Invalid input",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["application/xml"] = new OpenApiMediaType()
                    }

                }
            },
        };

        [Fact]
        public void ParseBasicOperationShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicOperation.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_basicOperation);
        }

        [Fact]
        public void ParseBasicOperationTwiceShouldYieldSameObject()
        {
            // Arrange
            MapNode node;
            using (var stream = new MemoryStream(
                Encoding.Default.GetBytes(_basicOperation.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0))))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_basicOperation);
        }

        [Fact]
        public void ParseOperationWithBodyShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithBody.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_operationWithBody, options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public void ParseOperationWithBodyTwiceShouldYieldSameObject()
        {
            // Arrange
            MapNode node;
            using (var stream = new MemoryStream(
                Encoding.Default.GetBytes(_operationWithBody.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0))))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_operationWithBody, options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public void ParseOperationWithResponseExamplesShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithResponseExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(
                new OpenApiOperation()
                {
                    Responses = new OpenApiResponses()
                    {
                        { "200", new OpenApiResponse()
                        {
                            Description = "An array of float response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType()
                                {
                                    Schema = new()
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new()
                                        {
                                            Type = JsonSchemaType.Number,
                                            Format = "float"
                                        }
                                    },
                                    Example = new JsonArray()
                                    {
                                        5.0,
                                        6.0,
                                        7.0
                                    }
                                },
                                ["application/xml"] = new OpenApiMediaType()
                                {
                                    Schema = new()
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new()
                                        {
                                            Type = JsonSchemaType.Number,
                                            Format = "float"
                                        }
                                    }
                                }
                            }
                        }}
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[0].Parent)
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[0].Root)
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[1].Parent)
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[1].Root)
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[2].Parent)
                .Excluding(o => o.Responses["200"].Content["application/json"].Example[2].Root));
        }

        [Fact]
        public void ParseOperationWithEmptyProducesArraySetsResponseSchemaIfExists()
        {
            // Arrange
            MapNode node;
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithEmptyProducesArrayInResponse.json"));
            node = TestHelper.CreateYamlMapNode(stream);

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);
            var expected = @"{
  ""produces"": [
    ""application/octet-stream""
  ],
  ""responses"": {
    ""200"": {
      ""description"": ""OK"",
      ""schema"": {
        ""type"": ""string"",
        ""description"": ""The content of the file."",
        ""format"": ""binary"",
        ""x-ms-summary"": ""File Content""
      }
    }
  }
}";

            var stringBuilder = new StringBuilder();
            var jsonWriter = new OpenApiJsonWriter(new StringWriter(stringBuilder));
            operation.SerializeAsV2(jsonWriter);

            // Assert
            var actual = stringBuilder.ToString();
            actual.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());            
        }

        [Fact]
        public void ParseOperationWithBodyAndEmptyConsumesSetsRequestBodySchemaIfExists()
        {
            // Arrange
            MapNode node;
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithBodyAndEmptyConsumes.yaml"));
            node = TestHelper.CreateYamlMapNode(stream);

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_operationWithBody, options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public void ParseV2ResponseWithExamplesExtensionWorks()
        {            
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "opWithResponseExamplesExtension.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);
            var actual = operation.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            var expected = @"summary: Get all pets
responses:
  '200':
    description: Successful response
    content:
      application/json:
        schema:
          type: array
          items:
            type: object
            properties:
              name:
                type: string
              age:
                type: integer
        examples:
          example1:
            summary: Example - List of Pets
            value:
              - name: Buddy
                age: 2
              - name: Whiskers
                age: 1
          example2:
            summary: Example - Playful Cat
            value:
              name: Whiskers
              age: 1";

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void LoadV3ExamplesInResponseAsExtensionsWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "v3OperationWithResponseExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV3Deserializer.LoadOperation(node);
            var actual = operation.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            var expected = @"summary: Get all pets
produces:
  - application/json
responses:
  '200':
    description: Successful response
    schema:
      type: array
      items:
        type: object
        properties:
          name:
            type: string
          age:
            type: integer
    x-examples:
      example1:
        summary: Example - List of Pets
        value:
          - name: Buddy
            age: 2
          - name: Whiskers
            age: 1
      example2:
        summary: Example - Playful Cat
        value:
          name: Whiskers
          age: 1";

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void LoadV2OperationWithBodyParameterExamplesWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "opWithBodyParameterExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);
            var actual = operation.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            var expected = @"summary: Get all pets
requestBody:
  content:
    application/json:
      schema:
        type: array
        items:
          type: object
          properties:
            name:
              type: string
            age:
              type: integer
      examples:
        example1:
          summary: Example - List of Pets
          value:
            - name: Buddy
              age: 2
            - name: Whiskers
              age: 1
        example2:
          summary: Example - Playful Cat
          value:
            name: Whiskers
            age: 1
  required: true
  x-bodyName: body
responses: { }";

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void LoadV3ExamplesInRequestBodyParameterAsExtensionsWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "v3OperationWithBodyParameterExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV3Deserializer.LoadOperation(node);
            var actual = operation.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            var expected = @"summary: Get all pets
consumes:
  - application/json
parameters:
  - in: body
    name: body
    required: true
    schema:
      type: array
      items:
        type: object
        properties:
          name:
            type: string
          age:
            type: integer
    x-examples:
      example1:
        summary: Example - List of Pets
        value:
          - name: Buddy
            age: 2
          - name: Whiskers
            age: 1
      example2:
        summary: Example - Playful Cat
        value:
          name: Whiskers
          age: 1
responses: { }";

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
