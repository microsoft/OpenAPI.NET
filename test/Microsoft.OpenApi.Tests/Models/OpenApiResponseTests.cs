// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        private static OpenApiResponse BasicResponse => new OpenApiResponse();

        private static OpenApiResponse AdvancedV2Response => new OpenApiResponse
        {
            Description = "A complex object array response",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchemaReference("customType", null)
                    },
                    Example = "Blabla",
                    Extensions = new Dictionary<string, IOpenApiExtension>()
                    {
                        ["myextension"] = new JsonNodeExtension("myextensionvalue"),
                    }, 
                }
            },
            Headers = new Dictionary<string, IOpenApiHeader>
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
            }
        };
        private static OpenApiResponse AdvancedV3Response => new OpenApiResponse
        {
            Description = "A complex object array response",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchemaReference("customType", null)
                    },
                    Example = "Blabla",
                    Extensions = new Dictionary<string, IOpenApiExtension>()
                    {
                        ["myextension"] = new JsonNodeExtension("myextensionvalue"),
                    },
                }
            },
            Headers = new Dictionary<string, IOpenApiHeader>
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
            }
        };

        private static OpenApiResponseReference V2OpenApiResponseReference => new OpenApiResponseReference("example1");
        private static OpenApiResponse ReferencedV2Response => new OpenApiResponse
        {
            Description = "A complex object array response",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchemaReference("customType", null)
                    }
                }
            },
            Headers = new Dictionary<string, IOpenApiHeader>
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
            }
        };
        private static OpenApiResponseReference V3OpenApiResponseReference => new OpenApiResponseReference("example1");

        private static OpenApiResponse ReferencedV3Response => new OpenApiResponse
        {
            Description = "A complex object array response",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["text/plain"] = new OpenApiMediaType
                {
                     Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchemaReference("customType", null)
                    }
                }
            },
            Headers = new Dictionary<string, IOpenApiHeader>
            {
                ["X-Rate-Limit-Limit"] = new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
                ["X-Rate-Limit-Reset"] = new OpenApiHeader
                {
                    Description = "The number of seconds left in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
            }
        };

        private static OpenApiResponse ResponseWithSummary => new OpenApiResponse
        {
            Summary = "Successful response",
            Description = "A detailed description of a successful response",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object
                    }
                }
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Yaml)]
        public async Task SerializeBasicResponseWorks(
            OpenApiSpecVersion version,
            string format)
        {
            // Arrange
            var expected = format == OpenApiConstants.Json ? @"{
  ""description"": null
}" : @"description: ";

            // Act
            var actual = await BasicResponse.SerializeAsync(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedResponseAsV3JsonWorks()
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
      ""example"": ""Blabla"",
      ""myextension"": ""myextensionvalue""
    }
  }
}";

            // Act
            var actual = await AdvancedV3Response.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedResponseAsV3YamlWorks()
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
    example: Blabla
    myextension: myextensionvalue";

            // Act
            var actual = await AdvancedV3Response.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedResponseAsV2JsonWorks()
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
  ""myextension"": ""myextensionvalue"",
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
            var actual = await AdvancedV2Response.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedResponseAsV2YamlWorks()
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
myextension: myextensionvalue
headers:
  X-Rate-Limit-Limit:
    description: The number of allowed requests in the current period
    type: integer
  X-Rate-Limit-Reset:
    description: The number of seconds left in the current period
    type: integer";

            // Act
            var actual = await AdvancedV2Response.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedResponseAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            V3OpenApiResponseReference.SerializeAsV3(writer);
            await writer.FlushAsync();

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedV3Response.SerializeAsV3(writer);
            await writer.FlushAsync();

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            V2OpenApiResponseReference.SerializeAsV2(writer);
            await writer.FlushAsync();

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedV2Response.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeResponseWithSummaryAsV32Works()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""Successful response"",
  ""description"": ""A detailed description of a successful response"",
  ""content"": {
    ""application/json"": {
      ""schema"": {
        ""type"": ""object""
      }
    }
  }
}";

            // Act
            var actual = await ResponseWithSummary.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeResponseWithSummaryAsV31Works()
        {
            // Arrange
            var expected = @"{
  ""description"": ""A detailed description of a successful response"",
  ""content"": {
    ""application/json"": {
      ""schema"": {
        ""type"": ""object""
      }
    }
  },
  ""x-oai-summary"": ""Successful response""
}";

            // Act
            var actual = await ResponseWithSummary.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeResponseWithSummaryAsV3Works()
        {
            // Arrange
            var expected = @"{
  ""description"": ""A detailed description of a successful response"",
  ""content"": {
    ""application/json"": {
      ""schema"": {
        ""type"": ""object""
      }
    }
  },
  ""x-oai-summary"": ""Successful response""
}";

            // Act
            var actual = await ResponseWithSummary.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public void ResponseWithSummaryShouldImplementIOpenApiSummarizedElement()
        {
            // Arrange
            var response = new OpenApiResponse { Summary = "Test summary" };

            // Act & Assert
            Assert.IsType<IOpenApiSummarizedElement>(response, exactMatch: false);
            Assert.Equal("Test summary", response.Summary);
        }
    }
}
