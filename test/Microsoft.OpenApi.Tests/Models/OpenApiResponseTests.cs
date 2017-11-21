// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    /// <summary>
    /// Test cases for <see cref="OpenApiResponse"/>.
    /// </summary>
    public class OpenApiResponseTests
    {
        public static OpenApiResponse AdvancedResponse;
        public static OpenApiResponse BasicResponse;

        static OpenApiResponseTests()
        {
            BasicResponse = new OpenApiResponse();
            AdvancedResponse = new OpenApiResponse
            {
                Description = "A complex object array response"
            };

            AdvancedResponse.AddMediaType(
                "text/plain",
                m =>
                {
                    m.Schema = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.Schema, Id = "customType"}
                        }
                    };
                });

            AdvancedResponse.AddHeader(
                "X-Rate-Limit-Limit",
                h =>
                {
                    h.Description = "The number of allowed requests in the current period";
                    h.Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    };
                });

            AdvancedResponse.AddHeader(
                "X-Rate-Limit-Reset",
                h =>
                {
                    h.Description = "The number of seconds left in the current period";
                    h.Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    };
                });
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0_0, OpenApiFormat.Yaml)]
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
      }
    }
  }
}";

            // Act
            var actual = AdvancedResponse.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

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
        $ref: '#/components/schemas/customType'";

            // Act
            var actual = AdvancedResponse.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0_0);

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
headers:
  X-Rate-Limit-Limit:
    description: The number of allowed requests in the current period
    schema:
      type: integer
  X-Rate-Limit-Reset:
    description: The number of seconds left in the current period
    schema:
      type: integer";

            // Act
            var actual = AdvancedResponse.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}