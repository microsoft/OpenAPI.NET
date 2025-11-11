// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiComponentsMediaTypesTests
    {
        public static OpenApiComponents ComponentsWithMediaTypes = new()
        {
            MediaTypes = new Dictionary<string, IOpenApiMediaType>()
            {
                ["application/json"] = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["age"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer
                            }
                        }
                    }
                },
                ["text/plain"] = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        [Fact]
        public async Task SerializeMediaTypesAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "mediaTypes": {
                    "application/json": {
                      "schema": {
                        "type": "object",
                        "properties": {
                          "name": {
                            "type": "string"
                          },
                          "age": {
                            "type": "integer"
                          }
                        }
                      }
                    },
                    "text/plain": {
                      "schema": {
                        "type": "string"
                      }
                    }
                  }
                }
                """;

            // Act
            var actual = await ComponentsWithMediaTypes.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeMediaTypesAsV31JsonWorks()
        {
            // Arrange - When serializing to v3.1, mediaTypes should be prefixed with x-oai-
            var expected =
                """
                {}
                """;

            // Act
            var actual = await ComponentsWithMediaTypes.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeMediaTypesAsV30JsonWorks()
        {
            // Arrange - When serializing to v3.0, mediaTypes should be prefixed with x-oai-
            var expected =
                """
                {}
                """;

            // Act
            var actual = await ComponentsWithMediaTypes.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public void CopyConstructorCopiesMediaTypes()
        {
            // Arrange
            var original = ComponentsWithMediaTypes;

            // Act
            var copy = new OpenApiComponents(original);

            // Assert
            Assert.NotNull(copy.MediaTypes);
            Assert.Equal(2, copy.MediaTypes.Count);
            Assert.True(copy.MediaTypes.ContainsKey("application/json"));
            Assert.True(copy.MediaTypes.ContainsKey("text/plain"));
        }

        [Fact]
        public void WorkspaceCanRegisterMediaTypeComponents()
        {
            // Arrange
            var mediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object
                }
            };

            var doc = new OpenApiDocument()
            {
                Components = new OpenApiComponents()
                {
                    MediaTypes = new Dictionary<string, IOpenApiMediaType>()
                    {
                        ["application/json"] = mediaType
                    }
                }
            };

            // Act
            doc.RegisterComponents();

            // Assert
            Assert.Equal(1, doc.Workspace.ComponentsCount());
        }

        [Fact]
        public void WorkspaceCanRegisterMediaTypeForDocument()
        {
            // Arrange
            var mediaType = new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.String
                }
            };

            var doc = new OpenApiDocument();

            // Act
            var result = doc.Workspace.RegisterComponentForDocument(doc, mediaType, "text/plain");

            // Assert
            Assert.True(result);
            Assert.Equal(1, doc.Workspace.ComponentsCount());
        }
    }
}
