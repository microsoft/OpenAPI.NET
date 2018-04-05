// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        public static OpenApiResponse BasicResponse = new OpenApiResponse();

        public static OpenApiResponse AdvancedResponse = new OpenApiResponse
        {
            Description = "A complex object array response",
            Content =
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = "customType"}
                        }
                    },
                    Example = new OpenApiString("Blabla")
                }
            },
            Headers =
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                },
            }
        };

        public static OpenApiResponse ReferencedResponse = new OpenApiResponse
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Response,
                Id = "example1"
            },
            Description = "A complex object array response",
            Content =
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = "customType"}
                        }
                    }
                }
            },
            Headers =
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                },
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiResponseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)]
        public void SerializeBasicResponseWorks(
            OpenApiSpecVersion version,
            OpenApiFormat format)
        {
            // Arrange & Act
            var actual = BasicResponse.Serialize(version, format);

            // Assert
            actual.Should().Be("{ }");
        }

        [Fact]
        public void SerializeAdvancedResponseAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""description"": ""A complex object array response"",
  ""headers"": {
    ""X-Rate-Limit-Limit"": {
      ""description"": ""The number of allowed requests in the current period"",
      ""schema"": {
        ""type"": ""integer""
      }
    },
    ""X-Rate-Limit-Reset"": {
      ""description"": ""The number of seconds left in the current period"",
      ""schema"": {
        ""type"": ""integer""
      }
    }
  },
  ""content"": {
    ""text/plain"": {
      ""schema"": {
        ""type"": ""array"",
        ""items"": {
          ""$ref"": ""#/components/schemas/customType""
        }
      },
      ""example"": ""Blabla""
    }
  }
}";

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
                @"description: A complex object array response
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
    example: Blabla";

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
            var expected = @"{
  ""description"": ""A complex object array response"",
  ""schema"": {
    ""type"": ""array"",
    ""items"": {
      ""$ref"": ""#/definitions/customType""
    }
  },
  ""examples"": {
    ""text/plain"": ""Blabla""
  },
  ""headers"": {
    ""X-Rate-Limit-Limit"": {
      ""description"": ""The number of allowed requests in the current period"",
      ""type"": ""integer""
    },
    ""X-Rate-Limit-Reset"": {
      ""description"": ""The number of seconds left in the current period"",
      ""type"": ""integer""
    }
  }
}";

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
                @"description: A complex object array response
schema:
  type: array
  items:
    $ref: '#/definitions/customType'
examples:
  text/plain: Blabla
headers:
  X-Rate-Limit-Limit:
    description: The number of allowed requests in the current period
    type: integer
  X-Rate-Limit-Reset:
    description: The number of seconds left in the current period
    type: integer";

            // Act
            var actual = AdvancedResponse.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedResponseAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/responses/example1""
}";

            // Act
            ReferencedResponse.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedResponseAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""A complex object array response"",
  ""headers"": {
    ""X-Rate-Limit-Limit"": {
      ""description"": ""The number of allowed requests in the current period"",
      ""schema"": {
        ""type"": ""integer""
      }
    },
    ""X-Rate-Limit-Reset"": {
      ""description"": ""The number of seconds left in the current period"",
      ""schema"": {
        ""type"": ""integer""
      }
    }
  },
  ""content"": {
    ""text/plain"": {
      ""schema"": {
        ""type"": ""array"",
        ""items"": {
          ""$ref"": ""#/components/schemas/customType""
        }
      }
    }
  }
}";

            // Act
            ReferencedResponse.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedResponseAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/responses/example1""
}";

            // Act
            ReferencedResponse.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedResponseAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""A complex object array response"",
  ""schema"": {
    ""type"": ""array"",
    ""items"": {
      ""$ref"": ""#/definitions/customType""
    }
  },
  ""headers"": {
    ""X-Rate-Limit-Limit"": {
      ""description"": ""The number of allowed requests in the current period"",
      ""type"": ""integer""
    },
    ""X-Rate-Limit-Reset"": {
      ""description"": ""The number of seconds left in the current period"",
      ""type"": ""integer""
    }
  }
}";

            // Act
            ReferencedResponse.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}