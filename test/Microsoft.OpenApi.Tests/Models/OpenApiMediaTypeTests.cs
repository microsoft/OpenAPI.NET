// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        public static OpenApiMediaType BasicMediaType = new();

        public static OpenApiMediaType AdvanceMediaType = new()
            {
                Example = 42,
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithObjectExample = new()
        {
            Example = new JsonObject
            {
                ["versions"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["status"] = "Status1",
                        ["id"] = "v1",
                        ["links"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["href"] = "http://example.com/1",
                                ["rel"] = "sampleRel1"
                            }
                        }
                    },

                    new JsonObject
                    {
                        ["status"] = "Status2",
                        ["id"] = "v2",
                        ["links"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["href"] = "http://example.com/2",
                                ["rel"] = "sampleRel2"
                            }
                        }
                    }
                }
            },
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithXmlExample = new()
        {
            Example = "<xml>123</xml>",
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithObjectExamples = new()
        {
            Examples = new Dictionary<string, IOpenApiExample>
            {
                ["object1"] = new OpenApiExample()
                {
                    Value = new JsonObject
                    {
                        ["versions"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["status"] = "Status1",
                                ["id"] = "v1",
                                ["links"] = new JsonArray
                                {
                                    new JsonObject
                                    {
                                        ["href"] = "http://example.com/1",
                                        ["rel"] = "sampleRel1"
                                    }
                                }
                            },

                            new JsonObject
                            {
                                ["status"] = "Status2",
                                ["id"] = "v2",
                                ["links"] = new JsonArray
                                {
                                    new JsonObject
                                    {
                                        ["href"] = "http://example.com/2",
                                        ["rel"] = "sampleRel2"
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiMediaTypeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(OpenApiConstants.Json, "{ }")]
        [InlineData(OpenApiConstants.Yaml, "{ }")]
        public async Task SerializeBasicMediaTypeAsV3Works(string format, string expected)
        {
            // Arrange & Act
            var actual = await BasicMediaType.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvanceMediaTypeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "example": 42,
                  "encoding": {
                    "testEncoding": {
                      "contentType": "image/png, image/jpeg",
                      "style": "simple",
                      "explode": true,
                      "allowReserved": true
                    }
                  }
                }
                """;

            // Act
            var actual = await AdvanceMediaType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvanceMediaTypeAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                example: 42
                encoding:
                  testEncoding:
                    contentType: 'image/png, image/jpeg'
                    style: simple
                    explode: true
                    allowReserved: true
                """;

            // Act
            var actual = await AdvanceMediaType.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithObjectExampleAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                example:
                  versions:
                    - status: Status1
                      id: v1
                      links:
                        - href: http://example.com/1
                          rel: sampleRel1
                    - status: Status2
                      id: v2
                      links:
                        - href: http://example.com/2
                          rel: sampleRel2
                encoding:
                  testEncoding:
                    contentType: 'image/png, image/jpeg'
                    style: simple
                    explode: true
                    allowReserved: true
                """;

            // Act
            var actual = await MediaTypeWithObjectExample.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithObjectExampleAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "example": {
                    "versions": [
                      {
                        "status": "Status1",
                        "id": "v1",
                        "links": [
                          {
                            "href": "http://example.com/1",
                            "rel": "sampleRel1"
                          }
                        ]
                      },
                      {
                        "status": "Status2",
                        "id": "v2",
                        "links": [
                          {
                            "href": "http://example.com/2",
                            "rel": "sampleRel2"
                          }
                        ]
                      }
                    ]
                  },
                  "encoding": {
                    "testEncoding": {
                      "contentType": "image/png, image/jpeg",
                      "style": "simple",
                      "explode": true,
                      "allowReserved": true
                    }
                  }
                }
                """;

            // Act
            var actual = await MediaTypeWithObjectExample.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithXmlExampleAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                example: <xml>123</xml>
                encoding:
                  testEncoding:
                    contentType: 'image/png, image/jpeg'
                    style: simple
                    explode: true
                    allowReserved: true
                """;

            // Act
            var actual = await MediaTypeWithXmlExample.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithXmlExampleAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "example": "<xml>123</xml>",
                  "encoding": {
                    "testEncoding": {
                      "contentType": "image/png, image/jpeg",
                      "style": "simple",
                      "explode": true,
                      "allowReserved": true
                    }
                  }
                }
                """;

            // Act
            var actual = await MediaTypeWithXmlExample.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithObjectExamplesAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                examples:
                  object1:
                    value:
                      versions:
                        - status: Status1
                          id: v1
                          links:
                            - href: http://example.com/1
                              rel: sampleRel1
                        - status: Status2
                          id: v2
                          links:
                            - href: http://example.com/2
                              rel: sampleRel2
                encoding:
                  testEncoding:
                    contentType: 'image/png, image/jpeg'
                    style: simple
                    explode: true
                    allowReserved: true
                """;

            // Act
            var actual = await MediaTypeWithObjectExamples.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);
            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeMediaTypeWithObjectExamplesAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "examples": {
                    "object1": {
                      "value": {
                        "versions": [
                          {
                            "status": "Status1",
                            "id": "v1",
                            "links": [
                              {
                                "href": "http://example.com/1",
                                "rel": "sampleRel1"
                              }
                            ]
                          },
                          {
                            "status": "Status2",
                            "id": "v2",
                            "links": [
                              {
                                "href": "http://example.com/2",
                                "rel": "sampleRel2"
                              }
                            ]
                          }
                        ]
                      }
                    }
                  },
                  "encoding": {
                    "testEncoding": {
                      "contentType": "image/png, image/jpeg",
                      "style": "simple",
                      "explode": true,
                      "allowReserved": true
                    }
                  }
                }
                """;

            // Act
            var actual = await MediaTypeWithObjectExamples.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MediaTypeCopyConstructorWorks()
        {
            var clone = new OpenApiMediaType(MediaTypeWithObjectExamples)
            {
                Example = 42,
                Examples = new Dictionary<string, IOpenApiExample>(),
                Encoding = new Dictionary<string, OpenApiEncoding>(),
                Extensions = new Dictionary<string, IOpenApiExtension>()
            };

            // Assert
            Assert.Equal(42, clone.Example.GetValue<int>());
            Assert.Empty(clone.Examples);
            Assert.Empty(clone.Encoding);
            Assert.Empty(clone.Extensions);
            Assert.Null(MediaTypeWithObjectExamples.Example);
        }
    }
}
