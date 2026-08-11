// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiRequestBodyTests
    {
        private static OpenApiRequestBody AdvancedRequestBody => new()
        {
            Description = "description",
            Required = true,
            Content = new Dictionary<string, IOpenApiMediaType>()
            {
                ["application/json"] = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        private static OpenApiRequestBodyReference OpenApiRequestBodyReference => new("example1");
        private static OpenApiRequestBody ReferencedRequestBody => new()
        {
            Description = "description",
            Required = true,
            Content = new Dictionary<string, IOpenApiMediaType>()
            {
                ["application/json"] = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }
        };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedRequestBodyAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedRequestBody.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedRequestBodyAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiRequestBodyReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedRequestBodyAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedRequestBody.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        // A multipart body covering every way of describing a form field, including the
        // pre-3.1 and 3.1+ spellings of binary content.
        // https://spec.openapis.org/oas/v3.2.0.html#migrating-binary-descriptions-from-oas-3-0
        private static OpenApiRequestBody MultipartRequestBody => new()
        {
            Required = true,
            Content = new Dictionary<string, IOpenApiMediaType>()
            {
                ["multipart/form-data"] = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string> { "file" },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            // 3.1+ spelling of raw binary content.
                            ["file"] = new OpenApiSchema() { ContentMediaType = "application/octet-stream" },
                            // Pre-3.1 spelling of raw binary content.
                            ["legacyFile"] = new OpenApiSchema() { Type = JsonSchemaType.String, Format = "binary" },
                            // 3.1+ spelling of base64 encoded content.
                            ["token"] = new OpenApiSchema() { Type = JsonSchemaType.String, ContentEncoding = "base64" },
                            ["comment"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String,
                                Description = "description1"
                            },
                        }
                    }
                }
            }
        };

        [Fact]
        public void ConvertToFormDataParametersProjectsSchemaPropertiesOntoParameters()
        {
            // Arrange
            var requestBody = MultipartRequestBody;
            var bodySchema = (OpenApiSchema)requestBody.Content["multipart/form-data"].Schema!;
            var writer = new OpenApiJsonWriter(new StringWriter(CultureInfo.InvariantCulture));

            // Act
            var parameters = requestBody.ConvertToFormDataParameters(writer).ToList();

            // Assert
            Assert.Collection(parameters,
                file =>
                {
                    Assert.Equal("file", file.Name);
                    Assert.True(file.Required);
                    Assert.Null(file.Description);
                },
                legacyFile =>
                {
                    Assert.Equal("legacyFile", legacyFile.Name);
                    Assert.False(legacyFile.Required);
                },
                token =>
                {
                    Assert.Equal("token", token.Name);
                    Assert.False(token.Required);
                },
                comment =>
                {
                    Assert.Equal("comment", comment.Name);
                    Assert.False(comment.Required);
                    Assert.Equal("description1", comment.Description);
                });

            // The conversion must not mutate the schema it was derived from.
            var fileSchema = (OpenApiSchema)bodySchema.Properties["file"];
            Assert.Null(fileSchema.Type);
            Assert.Null(fileSchema.Format);
            Assert.Equal("application/octet-stream", fileSchema.ContentMediaType);
        }

        [Fact]
        public async Task SerializeOperationWithBinaryFormDataAsV2JsonWorks()
        {
            // Arrange
            var operation = new OpenApiOperation
            {
                RequestBody = MultipartRequestBody,
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse { Description = "Uploaded." }
                }
            };

            var expected =
                """
                {
                  "consumes": [
                    "multipart/form-data"
                  ],
                  "parameters": [
                    {
                      "in": "formData",
                      "name": "file",
                      "required": true,
                      "type": "string",
                      "format": "binary"
                    },
                    {
                      "in": "formData",
                      "name": "legacyFile",
                      "type": "string",
                      "format": "binary"
                    },
                    {
                      "in": "formData",
                      "name": "token",
                      "type": "string",
                      "format": "byte"
                    },
                    {
                      "in": "formData",
                      "name": "comment",
                      "description": "description1",
                      "type": "string"
                    }
                  ],
                  "responses": {
                    "200": {
                      "description": "Uploaded."
                    }
                  }
                }
                """;

            // Act
            var actual = await operation.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }
    }
}
