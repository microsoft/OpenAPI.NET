// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models;

[Collection("DefaultSettings")]
public class OpenApiPathItemTests
{
    [Fact]
    public async Task SerializeAsV2JsonWorks()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "summary",
                    Description = "description",
                    OperationId = "operationId",
                    Tags = new HashSet<OpenApiTagReference> { new("tag1") },
                    Parameters = new List<IOpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "param1",
                            In = ParameterLocation.Query,
                            Description = "description",
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "description",
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "https://api.example.com/v1"
                }
            },
            Parameters = new List<IOpenApiParameter>
            {
                new OpenApiParameter()
                {
                    Name = "param1",
                    In = ParameterLocation.Query,
                    Description = "description",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        var expectedJson =
        """
        {
          "get": {
            "tags": [
              "tag1"
            ],
            "summary": "summary",
            "description": "description",
            "operationId": "operationId",
            "produces": [
              "application/json"
            ],
            "parameters": [
              {
                "in": "query",
                "name": "param1",
                "description": "description",
                "required": true,
                "type": "string"
              }
            ],
            "responses": {
              "200": {
                "description": "description",
                "schema": {
                "type": "string"
                }
              }
            }
          },
          "parameters": [
            {
              "in": "query",
              "name": "param1",
              "description": "description",
              "required": true,
              "type": "string"
            }
          ],
          "x-summary": "summary",
          "x-description": "description"
        }
        """;

        var parsedExpectedJson = JsonNode.Parse(expectedJson);
        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then
        Assert.True(JsonNode.DeepEquals(parsedExpectedJson, parsedActualJson));
    }
    [Fact]
    public async Task SerializeAsV3JsonWorks()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "summary",
                    Description = "description",
                    OperationId = "operationId",
                    Tags = new HashSet<OpenApiTagReference> { new("tag1") },
                    Parameters = new List<IOpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "param1",
                            In = ParameterLocation.Query,
                            Description = "description",
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "description",
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "https://api.example.com/v1"
                }
            },
            Parameters = new List<IOpenApiParameter>
            {
                new OpenApiParameter()
                {
                    Name = "param1",
                    In = ParameterLocation.Query,
                    Description = "description",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        var expectedJson =
        """
        {
          "summary": "summary",
          "description": "description",
          "get": {
            "summary": "summary",
            "description": "description",
            "operationId": "operationId",
            "tags": ["tag1"],
            "parameters": [
              {
                "name": "param1",
                "in": "query",
                "description": "description",
                "required": true,
                "schema": {
                  "type": "string"
                }
              }
            ],
            "responses": {
              "200": {
                "description": "description",
                "content": {
                  "application/json": {
                    "schema": {
                      "type": "string"
                    }
                  }
                }
              }
            }
          },
          "servers": [
            {
              "url": "https://api.example.com/v1"
            }
          ],
          "parameters": [
            {
              "name": "param1",
              "in": "query",
              "description": "description",
              "required": true,
              "schema": {
                "type": "string"
              }
            }
          ]
        }
        """;

        var parsedExpectedJson = JsonNode.Parse(expectedJson);
        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then
        Assert.True(JsonNode.DeepEquals(parsedExpectedJson, parsedActualJson));
    }
    [Fact]
    public async Task SerializeAsV31JsonWorks()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "summary",
                    Description = "description",
                    OperationId = "operationId",
                    Tags = new HashSet<OpenApiTagReference> { new("tag1") },
                    Parameters = new List<IOpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "param1",
                            In = ParameterLocation.Query,
                            Description = "description",
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "description",
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = "https://api.example.com/v1"
                }
            },
            Parameters = new List<IOpenApiParameter>
            {
                new OpenApiParameter()
                {
                    Name = "param1",
                    In = ParameterLocation.Query,
                    Description = "description",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        var expectedJson =
        """
        {
          "summary": "summary",
          "description": "description",
          "get": {
            "summary": "summary",
            "description": "description",
            "operationId": "operationId",
            "tags": ["tag1"],
            "parameters": [
              {
                "name": "param1",
                "in": "query",
                "description": "description",
                "required": true,
                "schema": {
                  "type": "string"
                }
              }
            ],
            "responses": {
              "200": {
                "description": "description",
                "content": {
                  "application/json": {
                    "schema": {
                      "type": "string"
                    }
                  }
                }
              }
            }
          },
          "servers": [
            {
              "url": "https://api.example.com/v1"
            }
          ],
          "parameters": [
            {
              "name": "param1",
              "in": "query",
              "description": "description",
              "required": true,
              "schema": {
                "type": "string"
              }
            }
          ]
        }
        """;

        var parsedExpectedJson = JsonNode.Parse(expectedJson);
        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then
        Assert.True(JsonNode.DeepEquals(parsedExpectedJson, parsedActualJson));
    }

    [Fact]
    public async Task SerializeAsV32JsonWithQueryAndAdditionalOperationsWorks()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "get operation",
                    Description = "get description",
                    OperationId = "getOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "success"
                        }
                    }
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "query operation",
                    Description = "query description", 
                    OperationId = "queryOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "query success"
                        }
                    }
                },
                [new HttpMethod("Notify")] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    Description = "notify description",
                    OperationId = "notifyOperation", 
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "notify success"
                        }
                    }
                },
                [new HttpMethod("Custom")] = new OpenApiOperation
                {
                    Summary = "custom operation",
                    Description = "custom description",
                    OperationId = "customOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "custom success"
                        }
                    },
                }
            }
        };

        var expectedJson =
        """
        {
          "summary": "summary",
          "description": "description",
          "get": {
            "summary": "get operation",
            "description": "get description",
            "operationId": "getOperation",
            "responses": {
              "200": {
                "description": "success"
              }
            }
          },
          "query": {
            "summary": "query operation",
            "description": "query description",
            "operationId": "queryOperation",
            "responses": {
              "200": {
                "description": "query success"
              }
            }
          },
          "additionalOperations": {
            "Notify": {
              "summary": "notify operation",
              "description": "notify description",
              "operationId": "notifyOperation",
              "responses": {
                "200": {
                  "description": "notify success"
                }
              }
            },
            "Custom": {
              "summary": "custom operation",
              "description": "custom description",
              "operationId": "customOperation",
              "responses": {
                "200": {
                  "description": "custom success"
                }
              }
            }
          }
        }
        """;

        var parsedExpectedJson = JsonNode.Parse(expectedJson);
        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then
        Assert.True(JsonNode.DeepEquals(parsedExpectedJson, parsedActualJson));
    }

    [Fact]
    public async Task SerializeV32FeaturesAsExtensionsInV31Works()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description", 
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "get operation",
                    OperationId = "getOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "success"
                        }
                    }
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "query operation",
                    OperationId = "queryOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "query success"
                        }
                    }
                },
                [new HttpMethod("Notify")] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    OperationId = "notifyOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "notify success"
                        }
                    }
                },
                    
            }
        };

        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
        var parsedActualJson = Assert.IsType<JsonObject>(JsonNode.Parse(actualJson));

        // Then - should contain x-oai- prefixed extensions in the operation
        var additionalOperationsElement = Assert.IsType<JsonObject>(Assert.Contains("x-oai-additionalOperations", parsedActualJson));
        var queryElement = Assert.IsType<JsonObject>(Assert.Contains("Query", additionalOperationsElement));
        Assert.Equal("query operation", queryElement["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", additionalOperationsElement["Notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public async Task SerializeV32FeaturesAsExtensionsInV3Works()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary", 
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "get operation",
                    OperationId = "getOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "success"
                        }
                    }
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "query operation",
                    OperationId = "queryOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "query success"
                        }
                    }
                },
                [new HttpMethod("Notify")] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    OperationId = "notifyOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "notify success"
                        }
                    }
                }
            },
        };

        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
        var parsedActualJson = Assert.IsType<JsonObject>(JsonNode.Parse(actualJson));

        // Then - should contain x-oai- prefixed extensions in the operation
        var additionalOperationsElement = Assert.IsType<JsonObject>(Assert.Contains("x-oai-additionalOperations", parsedActualJson));
        var queryElement = Assert.IsType<JsonObject>(Assert.Contains("Query", additionalOperationsElement));
        Assert.Equal("query operation", queryElement["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", additionalOperationsElement["Notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public async Task SerializeV32FeaturesAsExtensionsInV2Works()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "get operation",
                    OperationId = "getOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "success"
                        }
                    },
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "query operation",
                    OperationId = "queryOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "query success"
                        }
                    }
                },
                [new HttpMethod("Notify")] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    OperationId = "notifyOperation",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "notify success"
                        }
                    }
                }
            }
        };

        // When
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);
        var parsedActualJson = Assert.IsType<JsonObject>(JsonNode.Parse(actualJson));

        // Then - should contain x-oai- prefixed extensions in the operation
        var additionalOperationsElement = Assert.IsType<JsonObject>(Assert.Contains("x-oai-additionalOperations", parsedActualJson));
        var queryElement = Assert.IsType<JsonObject>(Assert.Contains("Query", additionalOperationsElement));
        Assert.Equal("query operation", queryElement["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", additionalOperationsElement["Notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public void CopyConstructorCopiesQueryAndAdditionalOperations()
    {
        // Arrange
        var original = new OpenApiPathItem
        {
            Summary = "summary",
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "get operation",
                    OperationId = "getOperation",
                    Responses = new OpenApiResponses()
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "query operation",
                    OperationId = "queryOperation"
                },
                [new HttpMethod("Notify")] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    OperationId = "notifyOperation"
                }
            }
        };

        // Act
        var copy = new OpenApiPathItem(original);

        // Assert
        Assert.NotNull(original.Operations);
        Assert.NotNull(copy.Operations);
        Assert.Contains(HttpMethod.Get, original.Operations);
        Assert.Contains(HttpMethod.Get, copy.Operations);

        var copyQueryOp = Assert.Contains(new HttpMethod("Query"), copy.Operations);
        var originalQueryOp = Assert.Contains(new HttpMethod("Query"), original.Operations);
        Assert.Equal(originalQueryOp.Summary, copyQueryOp.Summary);
        Assert.Equal(originalQueryOp.OperationId, copyQueryOp.OperationId);

        var originalNotifyOp = Assert.Contains(new HttpMethod("Notify"), original.Operations);
        var copyNotifyOp = Assert.Contains(new HttpMethod("Notify"), copy.Operations);

        Assert.Equal(originalNotifyOp.Summary, copyNotifyOp.Summary);
    }
}
