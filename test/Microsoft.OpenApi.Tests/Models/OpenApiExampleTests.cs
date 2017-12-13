// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        public static OpenApiExample AdvancedExample = new OpenApiExample
        {
            Value = new OpenApiObject
            {
                ["versions"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    },

                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status2"),
                        ["id"] = new OpenApiString("v2"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/2"),
                                ["rel"] = new OpenApiString("sampleRel2")
                            }
                        }
                    }
                }
            }
        };

        public static OpenApiExample ReferencedExample = new OpenApiExample
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Example,
                Id = "example1",
            },
            Value = new OpenApiObject
            {
                ["versions"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    },

                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status2"),
                        ["id"] = new OpenApiString("v2"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/2"),
                                ["rel"] = new OpenApiString("sampleRel2")
                            }
                        }
                    }
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiExampleTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeAdvancedExampleAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
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
}";

            // Act
            AdvancedExample.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedExampleAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/examples/example1""
}";

            // Act
            ReferencedExample.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedExampleAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
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
}";

            // Act
            ReferencedExample.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}