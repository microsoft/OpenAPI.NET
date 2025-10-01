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
                            Content = new Dictionary<string, OpenApiMediaType>
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
                            Content = new Dictionary<string, OpenApiMediaType>
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
                            Content = new Dictionary<string, OpenApiMediaType>
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
                }
            },
            Query = new OpenApiOperation
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
            AdditionalOperations = new Dictionary<string, OpenApiOperation>
            {
                ["notify"] = new OpenApiOperation
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
                ["custom"] = new OpenApiOperation
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
            "notify": {
              "summary": "notify operation",
              "description": "notify description",
              "operationId": "notifyOperation",
              "responses": {
                "200": {
                  "description": "notify success"
                }
              }
            },
            "custom": {
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
            Query = new OpenApiOperation
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
            AdditionalOperations = new Dictionary<string, OpenApiOperation>
            {
                ["notify"] = new OpenApiOperation
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
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then - should contain x-oas- prefixed extensions
        Assert.True(parsedActualJson!["x-oas-query"] != null);
        Assert.True(parsedActualJson!["x-oas-additionalOperations"] != null);
        Assert.Equal("query operation", parsedActualJson!["x-oas-query"]!["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", parsedActualJson!["x-oas-additionalOperations"]!["notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public async Task SerializeV32FeaturesAsExtensionsInV3Works()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary", 
            Description = "description",
            Query = new OpenApiOperation
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
            AdditionalOperations = new Dictionary<string, OpenApiOperation>
            {
                ["notify"] = new OpenApiOperation
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
        var actualJson = await pathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then - should contain x-oas- prefixed extensions
        Assert.True(parsedActualJson!["x-oas-query"] != null);
        Assert.True(parsedActualJson!["x-oas-additionalOperations"] != null);
        Assert.Equal("query operation", parsedActualJson!["x-oas-query"]!["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", parsedActualJson!["x-oas-additionalOperations"]!["notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public async Task SerializeV32FeaturesAsExtensionsInV2Works()
    {
        var pathItem = new OpenApiPathItem
        {
            Summary = "summary",
            Description = "description",
            Query = new OpenApiOperation
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
            AdditionalOperations = new Dictionary<string, OpenApiOperation>
            {
                ["notify"] = new OpenApiOperation
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
        var parsedActualJson = JsonNode.Parse(actualJson);

        // Then - should contain x-oas- prefixed extensions
        Assert.True(parsedActualJson!["x-oas-query"] != null);
        Assert.True(parsedActualJson!["x-oas-additionalOperations"] != null);
        Assert.Equal("query operation", parsedActualJson!["x-oas-query"]!["summary"]!.GetValue<string>());
        Assert.Equal("notify operation", parsedActualJson!["x-oas-additionalOperations"]!["notify"]!["summary"]!.GetValue<string>());
    }

    [Fact]
    public void CopyConstructorCopiesQueryAndAdditionalOperations()
    {
        // Arrange
        var original = new OpenApiPathItem
        {
            Summary = "summary",
            Query = new OpenApiOperation
            {
                Summary = "query operation",
                OperationId = "queryOperation"
            },
            AdditionalOperations = new Dictionary<string, OpenApiOperation>
            {
                ["notify"] = new OpenApiOperation
                {
                    Summary = "notify operation",
                    OperationId = "notifyOperation"
                }
            }
        };

        // Act
        var copy = new OpenApiPathItem(original);

        // Assert
        Assert.NotNull(copy.Query);
        Assert.Equal(original.Query.Summary, copy.Query.Summary);
        Assert.Equal(original.Query.OperationId, copy.Query.OperationId);

        Assert.NotNull(copy.AdditionalOperations);
        Assert.Equal(original.AdditionalOperations.Count, copy.AdditionalOperations.Count);
        Assert.Equal(original.AdditionalOperations["notify"].Summary, copy.AdditionalOperations["notify"].Summary);

        // Verify it's a deep copy
        copy.Query.Summary = "modified";
        Assert.NotEqual(original.Query.Summary, copy.Query.Summary);
    }
}
