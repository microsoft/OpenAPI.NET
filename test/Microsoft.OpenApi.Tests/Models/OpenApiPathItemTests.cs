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
}
