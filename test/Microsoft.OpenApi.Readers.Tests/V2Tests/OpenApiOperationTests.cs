// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Reader.V3;
using Microsoft.OpenApi.Tests;
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
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            ],
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>()
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
            Parameters =
            [
                new OpenApiParameter
                {
                    Name = "petId",
                    In = ParameterLocation.Path,
                    Description = "ID of pet that needs to be updated",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
            ],
            RequestBody = new OpenApiRequestBody
            {
                Description = "Pet to update with",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Object
                        }
                    }
                },
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    [OpenApiConstants.BodyName] = new JsonNodeExtension("petObject")
                }
            },
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Pet updated.",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["application/xml"] = new OpenApiMediaType()
                    }
                },
                ["405"] = new OpenApiResponse
                {
                    Description = "Invalid input",
                    Content = new Dictionary<string, OpenApiMediaType>()
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
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

            // Assert
            Assert.Equivalent(_basicOperation, operation);
        }

        [Fact]
        public async Task ParseBasicOperationTwiceShouldYieldSameObject()
        {
            // Arrange
            MapNode node;
            using (var stream = new MemoryStream(
                Encoding.Default.GetBytes(await _basicOperation.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0))))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

            // Assert
            Assert.Equivalent(_basicOperation, operation);
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
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

            // Assert
            operation.Should().BeEquivalentTo(_operationWithBody, options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public async Task ParseOperationWithBodyTwiceShouldYieldSameObject()
        {
            // Arrange
            MapNode node;
            using (var stream = new MemoryStream(
                Encoding.Default.GetBytes(await _operationWithBody.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0))))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

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
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

            // Assert
            operation.Should().BeEquivalentTo(
                new OpenApiOperation()
                {
                    Responses = new OpenApiResponses()
                    {
                        { "200", new OpenApiResponse()
                        {
                            Description = "An array of float response",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchema()
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
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchema()
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
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());
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
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());            
        }

        [Fact]
        public void ParseOperationWithBodyAndEmptyConsumesSetsRequestBodySchemaIfExists()
        {
            // Arrange
            MapNode node;
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithBodyAndEmptyConsumes.yaml"));
            node = TestHelper.CreateYamlMapNode(stream);

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());

            // Assert
            operation.Should().BeEquivalentTo(_operationWithBody, options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public async Task ParseV2ResponseWithExamplesExtensionWorks()
        {            
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "opWithResponseExamplesExtension.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());
            var actual = await operation.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

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
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task LoadV3ExamplesInResponseAsExtensionsWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "v3OperationWithResponseExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV3Deserializer.LoadOperation(node, new());
            var actual = await operation.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

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
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task LoadV2OperationWithBodyParameterExamplesWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "opWithBodyParameterExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV2Deserializer.LoadOperation(node, new());
            var actual = await operation.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

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
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task LoadV3ExamplesInRequestBodyParameterAsExtensionsWorks()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "v3OperationWithBodyParameterExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV3Deserializer.LoadOperation(node, new());
            var actual = await operation.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

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
            Assert.Equal(expected, actual);
        }
        [Fact]
        public async Task SerializesBodyReferencesWorks()
        {
            var openApiDocument = new OpenApiDocument();

            var operation = new OpenApiOperation
            {
                RequestBody = new OpenApiRequestBodyReference("UserRequest", openApiDocument)
                {
                    Description = "User request body"
                }
            };
            openApiDocument.Paths.Add("/users", new OpenApiPathItem
            {
                Operations = new()
                {
                    [HttpMethod.Post] = operation
                }
            });
            openApiDocument.AddComponent("UserRequest", new OpenApiRequestBody
            {
                Description = "User creation request body",
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchemaReference("UserSchema", openApiDocument)
                    }
                }
            });
            openApiDocument.AddComponent("UserSchema", new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["email"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            });

            var actual = await openApiDocument.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);
            var expected =
"""
{
  "swagger": "2.0",
  "info": { },
  "paths": {
    "/users": {
      "post": {
        "consumes": [
          "application/json"
        ],
        "parameters": [
          {
            "$ref": "#/parameters/UserRequest"
          }
        ],
        "responses": { }
      }
    }
  },
  "definitions": {
    "UserSchema": {
      "type": "object",
      "properties": {
        "name": {
          "type": "string"
        },
        "email": {
          "type": "string"
        }
      }
    }
  },
  "parameters": {
    "UserRequest": {
      "in": "body",
      "name": "body",
      "description": "User creation request body",
      "schema": {
        "$ref": "#/definitions/UserSchema"
      }
    }
  }
}
""";
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }
        [Fact]
        public void DeduplicatesTagReferences()
        {

            var openApiDocument = new OpenApiDocument
            {
                Tags = new HashSet<OpenApiTag> { new() { Name = "user" } }
            };
            // Act
            var expectedOp = new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new OpenApiTagReference("user", openApiDocument),
                    new OpenApiTagReference("user", openApiDocument),
                },
                Summary = "Logs user into the system",
                Description = "",
                OperationId = "loginUser",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "password",
                        Description = "The password for login in clear text",
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                ]
            };
            using var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);
            expectedOp.SerializeAsV2(writer);
            var result = textWriter.ToString();
            var parsedJson = JsonNode.Parse(result);
            var operationObject = Assert.IsType<JsonObject>(parsedJson);
            var tags = Assert.IsType<JsonArray>(operationObject["tags"]);
            Assert.Single(tags);
        }
    }
}
