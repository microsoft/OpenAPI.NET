// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        public static OpenApiCallback AdvancedCallback = new OpenApiCallback
        {
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        public static OpenApiCallback ReferencedCallback = new OpenApiCallback
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Callback,
                Id = "simpleHook",
            },
            PathItems =
            {
                [RuntimeExpression.Build("$request.body#/url")]
                = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Post] =
                        new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content =
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "object"
                                        }
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success"
                                }
                            }
                        }
                    }
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiCallbackTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeAdvancedCallbackAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$request.body#/url"": {
    ""post"": {
      ""requestBody"": {
        ""content"": {
          ""application/json"": {
            ""schema"": {
              ""type"": ""object""
            }
          }
        }
      },
      ""responses"": {
        ""200"": {
          ""description"": ""Success""
        }
      }
    }
  }
}";

            // Act
            AdvancedCallback.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedCallbackAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/callbacks/simpleHook""
}";

            // Act
            ReferencedCallback.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedCallbackAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$request.body#/url"": {
    ""post"": {
      ""requestBody"": {
        ""content"": {
          ""application/json"": {
            ""schema"": {
              ""type"": ""object""
            }
          }
        }
      },
      ""responses"": {
        ""200"": {
          ""description"": ""Success""
        }
      }
    }
  }
}";

            // Act
            ReferencedCallback.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}