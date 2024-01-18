// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        public static OpenApiResponse BasicResponse = new();

        public static OpenApiResponse AdvancedResponse = new()
        {
            Description = "A complex object array response",
            Content =
            {
                ["text/plain"] = new()
                {
                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Reference = new() {Type = ReferenceType.Schema, Id = "customType"}
                        }
                    },
                    Example = new OpenApiString("Blabla"),
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["myextension"] = new OpenApiString("myextensionvalue"),
                    },
                }
            },
            Headers =
            {
                ["X-Rate-Limit-Limit"] = new()
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new()
                    {
                        Type = "integer"
                    }
                },
                ["X-Rate-Limit-Reset"] = new()
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new()
                    {
                        Type = "integer"
                    }
                },
            }
        };

        public static OpenApiResponse ReferencedResponse = new()
        {
            Reference = new()
            {
                Type = ReferenceType.Response,
                Id = "example1"
            },
            Description = "A complex object array response",
            Content =
            {
                ["text/plain"] = new()
                {
                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Reference = new() {Type = ReferenceType.Schema, Id = "customType"}
                        }
                    }
                }
            },
            Headers =
            {
                ["X-Rate-Limit-Limit"] = new()
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new()
                    {
                        Type = "integer"
                    }
                },
                ["X-Rate-Limit-Reset"] = new()
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new()
                    {
                        Type = "integer"
                    }
                },
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)]
        public void SerializeBasicResponseWorks(
            OpenApiSpecVersion version,
            OpenApiFormat format)
        {
            // Arrange
            var expected = format == OpenApiFormat.Json ?
                """
                {
                  "description": null
                }
                """ :
                @"description: ";

            // Act
            var actual = BasicResponse.Serialize(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedResponseAsV3JsonWorks()
        {
            // Arrange
            var expected = """
                           {
                             "description": "A complex object array response",
                             "headers": {
                               "X-Rate-Limit-Limit": {
                                 "description": "The number of allowed requests in the current period",
                                 "schema": {
                                   "type": "integer"
                                 }
                               },
                               "X-Rate-Limit-Reset": {
                                 "description": "The number of seconds left in the current period",
                                 "schema": {
                                   "type": "integer"
                                 }
                               }
                             },
                             "content": {
                               "text/plain": {
                                 "schema": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/customType"
                                   }
                                 },
                                 "example": "Blabla",
                                 "myextension": "myextensionvalue"
                               }
                             }
                           }
                           """;

            // Act
            var actual = AdvancedResponse.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedResponseAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                description: A complex object array response
                headers:
                  X-Rate-Limit-Limit:
                    description: The number of allowed requests in the current period
                    schema:
                      type: integer
                  X-Rate-Limit-Reset:
                    description: The number of seconds left in the current period
                    schema:
                      type: integer
                content:
                  text/plain:
                    schema:
                      type: array
                      items:
                        $ref: '#/components/schemas/customType'
                    example: Blabla
                    myextension: myextensionvalue
                """;

            // Act
            var actual = AdvancedResponse.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedResponseAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "description": "A complex object array response",
                  "schema": {
                    "type": "array",
                    "items": {
                      "$ref": "#/definitions/customType"
                    }
                  },
                  "examples": {
                    "text/plain": "Blabla"
                  },
                  "myextension": "myextensionvalue",
                  "headers": {
                    "X-Rate-Limit-Limit": {
                      "description": "The number of allowed requests in the current period",
                      "type": "integer"
                    },
                    "X-Rate-Limit-Reset": {
                      "description": "The number of seconds left in the current period",
                      "type": "integer"
                    }
                  }
                }
                """;

            // Act
            var actual = AdvancedResponse.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedResponseAsV2YamlWorks()
        {
            // Arrange
            var expected =
                """
                description: A complex object array response
                schema:
                  type: array
                  items:
                    $ref: '#/definitions/customType'
                examples:
                  text/plain: Blabla
                myextension: myextensionvalue
                headers:
                  X-Rate-Limit-Limit:
                    description: The number of allowed requests in the current period
                    type: integer
                  X-Rate-Limit-Reset:
                    description: The number of seconds left in the current period
                    type: integer
                """;

            // Act
            var actual = AdvancedResponse.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedResponseAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedResponse.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedResponseAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedResponse.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedResponseAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedResponse.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedResponseAsV2JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedResponse.SerializeAsV2WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
