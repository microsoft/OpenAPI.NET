// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Microsoft.OpenApi.Readers.V3;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiOperationTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiOperation/";

        private static readonly OpenApiOperation _basicOperation = new()
        {
            Summary = "Updates a pet in the store",
            Description = "",
            OperationId = "updatePet",
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new()
                    {
                        Type = "string"
                    }
                }
            },
            Responses = new()
            {
                ["200"] = new()
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new(),
                        ["application/xml"] = new()
                    }
                }
            }
        };

        private static readonly OpenApiOperation _operationWithFormData =
            new()
            {
                Summary = "Updates a pet in the store with form data",
                Description = "",
                OperationId = "updatePetWithForm",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "petId",
                        In = ParameterLocation.Path,
                        Description = "ID of pet that needs to be updated",
                        Required = true,
                        Schema = new()
                        {
                            Type = "string"
                        }
                    }
                },
                RequestBody = new()
                {
                    Content =
                    {
                        ["application/x-www-form-urlencoded"] = new()
                        {
                            Schema = new()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["name"] = new()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = "string"
                                    },
                                    ["status"] = new()
                                    {
                                        Description = "Updated status of the pet",
                                        Type = "string"
                                    }
                                },
                                Required = new HashSet<string>
                                {
                                    "name"
                                }
                            }
                        },
                        ["multipart/form-data"] = new()
                        {
                            Schema = new()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["name"] = new()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = "string"
                                    },
                                    ["status"] = new()
                                    {
                                        Description = "Updated status of the pet",
                                        Type = "string"
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
                Responses = new()
                {
                    ["200"] = new()
                    {
                        Description = "Pet updated.",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new(),
                            ["application/xml"] = new()
                        }

                    },
                    ["405"] = new()
                    {
                        Description = "Invalid input",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new(),
                            ["application/xml"] = new()
                        }
                    }
                }
            };

        private static readonly OpenApiOperation _operationWithBody = new()
        {
            Summary = "Updates a pet in the store with request body",
            Description = "",
            OperationId = "updatePetWithBody",
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new()
                    {
                        Type = "string"
                    }
                },
            },
            RequestBody = new()
            {
                Description = "Pet to update with",
                Required = true,
                Content =
                {
                    ["application/json"] = new()
                    {
                        Schema = new()
                        {
                            Type = "object"
                        }
                    }
                },
                Extensions = {
                    [OpenApiConstants.BodyName] = new OpenApiString("petObject")
                }
            },
            Responses = new()
            {
                ["200"] = new()
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new(),
                        ["application/xml"] = new()
                    }
                },
                ["405"] = new()
                {
                    Description = "Invalid input",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new(),
                        ["application/xml"] = new()
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
        public void ParseOperationWithFormDataShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithFormData.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_operationWithFormData);
        }

        [Fact]
        public void ParseOperationWithFormDataTwiceShouldYieldSameObject()
        {
            // Arrange
            MapNode node;
            using (var stream = new MemoryStream(
                Encoding.Default.GetBytes(_operationWithFormData.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0))))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node);

            // Assert
            operation.Should().BeEquivalentTo(_operationWithFormData);
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
            operation.Should().BeEquivalentTo(_operationWithBody);
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
            operation.Should().BeEquivalentTo(_operationWithBody);
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
                new OpenApiOperation
                {
                    Responses = new()
                    {
                        { "200", new()
                        {
                            Description = "An array of float response",
                            Content =
                            {
                                ["application/json"] = new()
                                {
                                    Schema = new()
                                    {
                                        Type = "array",
                                        Items = new()
                                        {
                                            Type = "number",
                                            Format = "float"
                                        }
                                    },
                                    Example = new OpenApiArray
                                    {
                                        new OpenApiFloat(5),
                                        new OpenApiFloat(6),
                                        new OpenApiFloat(7),
                                    }
                                },
                                ["application/xml"] = new()
                                {
                                    Schema = new()
                                    {
                                        Type = "array",
                                        Items = new()
                                        {
                                            Type = "number",
                                            Format = "float"
                                        }
                                    }
                                }
                            }
                        }}
                    }
                }
            );
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

            // Assert
            operation.Should().BeEquivalentTo(
                new OpenApiOperation
                {
                    Responses = new()
                    {
                        { "200", new()
                        {
                            Description = "OK",
                            Content =
                            {
                                ["application/octet-stream"] = new()
                                {
                                    Schema = new()
                                    {
                                        Format = "binary",
                                        Description = "The content of the file.",
                                        Type = "string",
                                        Extensions =
                                        {
                                            ["x-ms-summary"] = new OpenApiString("File Content")
                                        }
                                    }
                                }
                            }
                        }}
                    }
                }
            );
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
            operation.Should().BeEquivalentTo(_operationWithBody);
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
