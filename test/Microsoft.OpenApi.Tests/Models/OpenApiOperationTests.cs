// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiOperationTests
    {
        private static readonly OpenApiOperation _basicOperation = new();

        private static readonly OpenApiOperation _operationWithBody = new()
        {
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new()
            {
                Description = "externalDocsDescription",
                Url = new("http://external.com")
            },
            OperationId = "operationId1",
            Parameters =
            [
                new OpenApiParameter()
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1",
                },
                new OpenApiParameter()
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            ],
            RequestBody = new OpenApiRequestBody()
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Number,
                            Minimum = 5,
                            Maximum = 10
                        }
                    }
                }
            },
            Responses = new()
            {
                ["200"] = new OpenApiResponseReference("response1"),
                ["400"] = new OpenApiResponse()
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Number,
                                Minimum = 5,
                                Maximum = 10
                            }
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            },
            Metadata = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } },
        };

        private static OpenApiOperation _advancedOperationWithTagsAndSecurity => new()
        {
            Tags = new HashSet<OpenApiTagReference>
            {
                new OpenApiTagReference("tagId1", new OpenApiDocument{ Tags = new HashSet<OpenApiTag>() { new OpenApiTag{Name = "tagId1"}} })
            },
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new()
            {
                Description = "externalDocsDescription",
                Url = new("http://external.com")
            },
            OperationId = "operationId1",
            Parameters =
            [
                new OpenApiParameter()
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1"
                },
                new OpenApiParameter()
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            ],
            RequestBody = new OpenApiRequestBody()
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Number,
                            Minimum = 5,
                            Maximum = 10
                        }
                    }
                }
            },
            Responses = new()
            {
                ["200"] = new OpenApiResponseReference("response1"),
                ["400"] = new OpenApiResponse()
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Number,
                                Minimum = 5,
                                Maximum = 10
                            }
                        }
                    }
                }
            },
            Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [new OpenApiSecuritySchemeReference("securitySchemeId1", __advancedOperationWithTagsAndSecurity_supportingDocument)] = new List<string>(),
                    [new OpenApiSecuritySchemeReference("securitySchemeId2", __advancedOperationWithTagsAndSecurity_supportingDocument)] = new List<string>
                    {
                        "scopeName1",
                        "scopeName2"
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            }
        };
        private static OpenApiDocument __advancedOperationWithTagsAndSecurity_supportingDocument 
        {
            get
            {
                var document = new OpenApiDocument()
                {
                    Components = new() 
                    {
                        SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                        {
                            ["securitySchemeId1"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.ApiKey,
                                Name = "apiKeyName1",
                                In = ParameterLocation.Header,
                            },
                            ["securitySchemeId2"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.OpenIdConnect,
                                OpenIdConnectUrl = new("http://example.com"),
                            }
                        }
                    }
                };
                document.RegisterComponents();
                return document;
            }
        }

        private static readonly OpenApiOperation _operationWithFormData =
            new()
            {
                Summary = "Updates a pet in the store with form data",
                Description = "",
                OperationId = "updatePetWithForm",
                Parameters =
                [
                    new OpenApiParameter()
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
                RequestBody = new OpenApiRequestBody()
                {
                    Content =
                    {
                        ["application/x-www-form-urlencoded"] = new()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Properties =
                                {
                                    ["name"] = new OpenApiSchema()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = JsonSchemaType.String
                                    },
                                    ["status"] = new OpenApiSchema()
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
                        ["multipart/form-data"] = new()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Properties =
                                {
                                    ["name"] = new OpenApiSchema()
                                    {
                                        Description = "Updated name of the pet",
                                        Type = JsonSchemaType.String
                                    },
                                    ["status"] = new OpenApiSchema()
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
                Responses = new()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "Pet updated."
                    },
                    ["405"] = new OpenApiResponse()
                    {
                        Description = "Invalid input"
                    }
                }
            };

        [Fact]
        public async Task SerializeBasicOperationAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "responses": { }
                }
                """;

            // Act
            var actual = await _basicOperation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOperationWithBodyAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "summary": "summary1",
                  "description": "operationDescription",
                  "externalDocs": {
                    "description": "externalDocsDescription",
                    "url": "http://external.com"
                  },
                  "operationId": "operationId1",
                  "parameters": [
                    {
                      "name": "parameter1",
                      "in": "path"
                    },
                    {
                      "name": "parameter2",
                      "in": "header"
                    }
                  ],
                  "requestBody": {
                    "description": "description2",
                    "content": {
                      "application/json": {
                        "schema": {
                          "maximum": 10,
                          "minimum": 5,
                          "type": "number"
                        }
                      }
                    },
                    "required": true
                  },
                  "responses": {
                    "200": {
                      "$ref": "#/components/responses/response1"
                    },
                    "400": {
                      "description": null,
                      "content": {
                        "application/json": {
                          "schema": {
                            "maximum": 10,
                            "minimum": 5,
                            "type": "number"
                          }
                        }
                      }
                    }
                  },
                  "servers": [
                    {
                      "url": "http://server.com",
                      "description": "serverDescription"
                    }
                  ]
                }
                """;

            // Act
            var actual = await _operationWithBody.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdvancedOperationWithTagAndSecurityAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "tags": [
                    "tagId1"
                  ],
                  "summary": "summary1",
                  "description": "operationDescription",
                  "externalDocs": {
                    "description": "externalDocsDescription",
                    "url": "http://external.com"
                  },
                  "operationId": "operationId1",
                  "parameters": [
                    {
                      "name": "parameter1",
                      "in": "path"
                    },
                    {
                      "name": "parameter2",
                      "in": "header"
                    }
                  ],
                  "requestBody": {
                    "description": "description2",
                    "content": {
                      "application/json": {
                        "schema": {
                          "maximum": 10,
                          "minimum": 5,
                          "type": "number"
                        }
                      }
                    },
                    "required": true
                  },
                  "responses": {
                    "200": {
                      "$ref": "#/components/responses/response1"
                    },
                    "400": {
                      "description": null,
                      "content": {
                        "application/json": {
                          "schema": {
                            "maximum": 10,
                            "minimum": 5,
                            "type": "number"
                          }
                        }
                      }
                    }
                  },
                  "security": [
                    {
                      "securitySchemeId1": [ ],
                      "securitySchemeId2": [
                        "scopeName1",
                        "scopeName2"
                      ]
                    }
                  ],
                  "servers": [
                    {
                      "url": "http://server.com",
                      "description": "serverDescription"
                    }
                  ]
                }
                """;

            // Act
            var actual = await _advancedOperationWithTagsAndSecurity.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeBasicOperationAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "responses": { }
                }
                """;

            // Act
            var actual = await _basicOperation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOperationWithFormDataAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "summary": "Updates a pet in the store with form data",
                  "description": "",
                  "operationId": "updatePetWithForm",
                  "parameters": [
                    {
                      "name": "petId",
                      "in": "path",
                      "description": "ID of pet that needs to be updated",
                      "required": true,
                      "schema": {
                        "type": "string"
                      }
                    }
                  ],
                  "requestBody": {
                    "content": {
                      "application/x-www-form-urlencoded": {
                        "schema": {
                          "required": [
                            "name"
                          ],
                          "properties": {
                            "name": {
                              "type": "string",
                              "description": "Updated name of the pet"
                            },
                            "status": {
                              "type": "string",
                              "description": "Updated status of the pet"
                            }
                          }
                        }
                      },
                      "multipart/form-data": {
                        "schema": {
                          "required": [
                            "name"
                          ],
                          "properties": {
                            "name": {
                              "type": "string",
                              "description": "Updated name of the pet"
                            },
                            "status": {
                              "type": "string",
                              "description": "Updated status of the pet"
                            }
                          }
                        }
                      }
                    }
                  },
                  "responses": {
                    "200": {
                      "description": "Pet updated."
                    },
                    "405": {
                      "description": "Invalid input"
                    }
                  }
                }
                """;

            // Act
            var actual = await _operationWithFormData.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOperationWithFormDataAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "summary": "Updates a pet in the store with form data",
                  "description": "",
                  "operationId": "updatePetWithForm",
                  "consumes": [
                    "application/x-www-form-urlencoded",
                    "multipart/form-data"
                  ],
                  "parameters": [
                    {
                      "in": "path",
                      "name": "petId",
                      "description": "ID of pet that needs to be updated",
                      "required": true,
                      "type": "string"
                    },
                    {
                      "in": "formData",
                      "name": "name",
                      "description": "Updated name of the pet",
                      "required": true,
                      "type": "string"
                    },
                    {
                      "in": "formData",
                      "name": "status",
                      "description": "Updated status of the pet",
                      "type": "string"
                    }
                  ],
                  "responses": {
                    "200": {
                      "description": "Pet updated."
                    },
                    "405": {
                      "description": "Invalid input"
                    }
                  }
                }
                """;

            // Act
            var actual = await _operationWithFormData.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOperationWithBodyAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "summary": "summary1",
                  "description": "operationDescription",
                  "externalDocs": {
                    "description": "externalDocsDescription",
                    "url": "http://external.com"
                  },
                  "operationId": "operationId1",
                  "consumes": [
                    "application/json"
                  ],
                  "produces": [
                    "application/json"
                  ],
                  "parameters": [
                    {
                      "in": "path",
                      "name": "parameter1"
                    },
                    {
                      "in": "header",
                      "name": "parameter2"
                    },
                    {
                      "in": "body",
                      "name": "body",
                      "description": "description2",
                      "required": true,
                      "schema": {
                        "type": "number",
                        "maximum": 10,
                        "minimum": 5
                      }
                    }
                  ],
                  "responses": {
                    "200": {
                      "$ref": "#/responses/response1"
                    },
                    "400": {
                      "description": null,
                      "schema": {
                        "type": "number",
                        "maximum": 10,
                        "minimum": 5
                      }
                    }
                  },
                  "schemes": [
                    "http"
                  ]
                }
                """;

            // Act
            var actual = await _operationWithBody.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdvancedOperationWithTagAndSecurityAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "tags": [
                    "tagId1"
                  ],
                  "summary": "summary1",
                  "description": "operationDescription",
                  "externalDocs": {
                    "description": "externalDocsDescription",
                    "url": "http://external.com"
                  },
                  "operationId": "operationId1",
                  "consumes": [
                    "application/json"
                  ],
                  "produces": [
                    "application/json"
                  ],
                  "parameters": [
                    {
                      "in": "path",
                      "name": "parameter1"
                    },
                    {
                      "in": "header",
                      "name": "parameter2"
                    },
                    {
                      "in": "body",
                      "name": "body",
                      "description": "description2",
                      "required": true,
                      "schema": {
                        "type": "number",
                        "maximum": 10,
                        "minimum": 5
                      }
                    }
                  ],
                  "responses": {
                    "200": {
                      "$ref": "#/responses/response1"
                    },
                    "400": {
                      "description": null,
                      "schema": {
                        "type": "number",
                        "maximum": 10,
                        "minimum": 5
                      }
                    }
                  },
                  "schemes": [
                    "http"
                  ],
                  "security": [
                    {
                      "securitySchemeId1": [ ],
                      "securitySchemeId2": [
                        "scopeName1",
                        "scopeName2"
                      ]
                    }
                  ]
                }
                """;

            // Act
            var actual = await _advancedOperationWithTagsAndSecurity.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOperationWithNullCollectionAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "responses": { }
                }
                """;
            var operation = new OpenApiOperation
            {
                Parameters = null,
                Servers = null,
            };

            // Act
            var actual = await operation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public void EnsureOpenApiOperationCopyConstructorCopiesResponsesObject()
        {
            // Arrange and act
            var operation = new OpenApiOperation(_operationWithBody);

            // Assert
            Assert.NotNull(operation.Responses);
            Assert.Equal(2, operation.Responses.Count);
        }

        [Fact]
        public void EnsureOpenApiOperationCopyConstructorCopiesNull()
        {
            // Arrange
            _basicOperation.Parameters = null;
            _basicOperation.Tags = null;
            _basicOperation.Responses = null;
            _basicOperation.Callbacks = null;
            _basicOperation.Security = null;
            _basicOperation.Servers = null;
            _basicOperation.Extensions = null;

            // Act
            var operation = new OpenApiOperation(_basicOperation);

            // Assert
            Assert.Null(operation.Tags);
            Assert.Null(operation.Summary);
            Assert.Null(operation.Description);
            Assert.Null(operation.ExternalDocs);
            Assert.Null(operation.OperationId);
            Assert.Null(operation.Parameters);
            Assert.Null(operation.RequestBody);
            Assert.Null(operation.Responses);
            Assert.Null(operation.Callbacks);
            Assert.Null(operation.Security);
            Assert.Null(operation.Servers);
            Assert.Null(operation.Extensions);
        }

        [Fact]
        public async Task EnsureOpenApiOperationCopyConstructor_SerializationResultsInSame()
        {
            var operations = new[]
            {
                _basicOperation,
                _operationWithBody,
                _operationWithFormData,
                _advancedOperationWithTagsAndSecurity
            };

            foreach (var operation in operations)
            {
                // Act
                var expected = await operation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
                var openApiOperation = new OpenApiOperation(operation);
                var actual = await openApiOperation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

                // Assert
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void OpenApiOperationCopyConstructorWithAnnotationsSucceeds()
        {
            var baseOperation = new OpenApiOperation
            {
                Metadata = new Dictionary<string, object>
                {
                    ["key1"] = "value1",
                    ["key2"] = 2
                }
            };

            var actualOperation = new OpenApiOperation(baseOperation);

            Assert.Equal(baseOperation.Metadata["key1"], actualOperation.Metadata["key1"]);

            baseOperation.Metadata["key1"] = "value2";

            Assert.NotEqual(baseOperation.Metadata["key1"], actualOperation.Metadata["key1"]);
        }
    }
}
