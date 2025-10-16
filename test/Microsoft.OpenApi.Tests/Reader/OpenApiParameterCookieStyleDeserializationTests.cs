// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterCookieStyleDeserializationTests
    {
        [Fact]
        public void DeserializeCookieParameterStyleFromJsonWorks()
        {
            // Arrange
            var json = """
                {
                  "openapi": "3.2.0",
                  "info": {
                    "title": "Test API",
                    "version": "1.0.0"
                  },
                  "paths": {
                    "/test": {
                      "get": {
                        "parameters": [
                          {
                            "name": "sessionId",
                            "in": "cookie",
                            "style": "cookie",
                            "description": "Session identifier stored in cookie",
                            "schema": {
                              "type": "string"
                            }
                          }
                        ],
                        "responses": {
                          "200": {
                            "description": "Success"
                          }
                        }
                      }
                    }
                  }
                }
                """;

            // Act
            var result = OpenApiDocument.Parse(json, "json", SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            
            var parameter = result.Document.Paths["/test"].Operations[System.Net.Http.HttpMethod.Get].Parameters[0];
            Assert.Equal("sessionId", parameter.Name);
            Assert.Equal(ParameterLocation.Cookie, parameter.In);
            Assert.Equal(ParameterStyle.Cookie, parameter.Style);
        }

        [Fact]
        public void DeserializeCookieParameterWithDefaultStyleFromJsonWorks()
        {
            // Arrange
            var json = """
                {
                  "openapi": "3.2.0",
                  "info": {
                    "title": "Test API",
                    "version": "1.0.0"
                  },
                  "paths": {
                    "/test": {
                      "get": {
                        "parameters": [
                          {
                            "name": "preferences",
                            "in": "cookie",
                            "description": "User preferences stored in cookie",
                            "schema": {
                              "type": "string"
                            }
                          }
                        ],
                        "responses": {
                          "200": {
                            "description": "Success"
                          }
                        }
                      }
                    }
                  }
                }
                """;

            // Act
            var result = OpenApiDocument.Parse(json, "json", SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            
            var parameter = result.Document.Paths["/test"].Operations[System.Net.Http.HttpMethod.Get].Parameters[0];
            Assert.Equal("preferences", parameter.Name);
            Assert.Equal(ParameterLocation.Cookie, parameter.In);
            Assert.Equal(ParameterStyle.Form, parameter.Style); // Should default to Form for cookie location
        }

        [Fact]
        public void DeserializeCookieParameterStyleFromYamlWorks()
        {
            // Arrange
            var yaml = """
                openapi: 3.2.0
                info:
                  title: Test API
                  version: 1.0.0
                paths:
                  /test:
                    get:
                      parameters:
                        - name: sessionId
                          in: cookie
                          style: cookie
                          description: Session identifier stored in cookie
                          schema:
                            type: string
                      responses:
                        '200':
                          description: Success
                """;

            // Act
            var result = OpenApiDocument.Parse(yaml, "yaml", SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            
            var parameter = result.Document.Paths["/test"].Operations[System.Net.Http.HttpMethod.Get].Parameters[0];
            Assert.Equal("sessionId", parameter.Name);
            Assert.Equal(ParameterLocation.Cookie, parameter.In);
            Assert.Equal(ParameterStyle.Cookie, parameter.Style);
        }

        [Fact]
        public async Task SerializeAndDeserializeCookieParameterRoundTrip()
        {
            // Arrange
            var original = new OpenApiParameter
            {
                Name = "trackingId",
                In = ParameterLocation.Cookie,
                Style = ParameterStyle.Cookie,
                Description = "Tracking identifier",
                Schema = new OpenApiSchema { Type = JsonSchemaType.String }
            };

            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test", Version = "1.0.0" },
                Paths = new OpenApiPaths
                {
                    ["/test"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [System.Net.Http.HttpMethod.Get] = new OpenApiOperation
                            {
                                Parameters = new[] { original },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse { Description = "Success" }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var json = await document.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);
            var result = OpenApiDocument.Parse(json, "json", SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(result.Document);
            Assert.Empty(result.Diagnostic.Errors);
            
            var parameter = result.Document.Paths["/test"].Operations[System.Net.Http.HttpMethod.Get].Parameters[0];
            Assert.Equal(original.Name, parameter.Name);
            Assert.Equal(original.In, parameter.In);
            Assert.Equal(original.Style, parameter.Style);
            Assert.Equal(original.Description, parameter.Description);
        }
    }
}
