﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        public static OpenApiMediaType BasicMediaType = new OpenApiMediaType();

        public static OpenApiMediaType AdvanceMediaType = new OpenApiMediaType
        {
            Example = new OpenApiAny(42),
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithObjectExample = new OpenApiMediaType
        {
            Example = new OpenApiAny(new JsonObject
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
            }),
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithXmlExample = new OpenApiMediaType
        {
            Example = new OpenApiAny("<xml>123</xml>"),
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

        public static OpenApiMediaType MediaTypeWithObjectExamples = new OpenApiMediaType
        {
            Examples = {
                ["object1"] = new OpenApiExample
                {
                    Value = new OpenApiAny(new JsonObject
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
                    })
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
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicMediaTypeAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = BasicMediaType.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceMediaTypeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""example"": 42,
  ""encoding"": {
    ""testEncoding"": {
      ""contentType"": ""image/png, image/jpeg"",
      ""style"": ""simple"",
      ""explode"": true,
      ""allowReserved"": true
    }
  }
}";

            // Act
            var actual = AdvanceMediaType.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceMediaTypeAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"example: 42
encoding:
  testEncoding:
    contentType: 'image/png, image/jpeg'
    style: simple
    explode: true
    allowReserved: true";

            // Act
            var actual = AdvanceMediaType.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithObjectExampleAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"example:
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
    allowReserved: true";

            // Act
            var actual = MediaTypeWithObjectExample.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithObjectExampleAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""example"": {
    ""versions"": [
      {
        ""status"": ""Status1"",
        ""id"": ""v1"",
        ""links"": [
          {
            ""href"": ""http://example.com/1"",
            ""rel"": ""sampleRel1""
          }
        ]
      },
      {
        ""status"": ""Status2"",
        ""id"": ""v2"",
        ""links"": [
          {
            ""href"": ""http://example.com/2"",
            ""rel"": ""sampleRel2""
          }
        ]
      }
    ]
  },
  ""encoding"": {
    ""testEncoding"": {
      ""contentType"": ""image/png, image/jpeg"",
      ""style"": ""simple"",
      ""explode"": true,
      ""allowReserved"": true
    }
  }
}";

            // Act
            var actual = MediaTypeWithObjectExample.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithXmlExampleAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"example: <xml>123</xml>
encoding:
  testEncoding:
    contentType: 'image/png, image/jpeg'
    style: simple
    explode: true
    allowReserved: true";

            // Act
            var actual = MediaTypeWithXmlExample.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithXmlExampleAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""example"": ""<xml>123</xml>"",
  ""encoding"": {
    ""testEncoding"": {
      ""contentType"": ""image/png, image/jpeg"",
      ""style"": ""simple"",
      ""explode"": true,
      ""allowReserved"": true
    }
  }
}";

            // Act
            var actual = MediaTypeWithXmlExample.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithObjectExamplesAsV3YamlWorks()
        {
            // Arrange
            var expected = @"examples:
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
    allowReserved: true";

            // Act
            var actual = MediaTypeWithObjectExamples.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeMediaTypeWithObjectExamplesAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""examples"": {
    ""object1"": {
      ""value"": {
        ""versions"": [
          {
            ""status"": ""Status1"",
            ""id"": ""v1"",
            ""links"": [
              {
                ""href"": ""http://example.com/1"",
                ""rel"": ""sampleRel1""
              }
            ]
          },
          {
            ""status"": ""Status2"",
            ""id"": ""v2"",
            ""links"": [
              {
                ""href"": ""http://example.com/2"",
                ""rel"": ""sampleRel2""
              }
            ]
          }
        ]
      }
    }
  },
  ""encoding"": {
    ""testEncoding"": {
      ""contentType"": ""image/png, image/jpeg"",
      ""style"": ""simple"",
      ""explode"": true,
      ""allowReserved"": true
    }
  }
}";

            // Act
            var actual = MediaTypeWithObjectExamples.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
