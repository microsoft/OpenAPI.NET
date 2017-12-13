// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiLinkTests
    {
        public static OpenApiLink AdvancedLink = new OpenApiLink
        {
            OperationId = "operationId1",
            Parameters =
            {
                ["parameter1"] = new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new RuntimeExpressionAnyWrapper
            {
                Any = new OpenApiObject
                {
                    ["property1"] = new OpenApiBoolean(true)
                }
            },
            Description = "description1",
            Server = new OpenApiServer
            {
                Description = "serverDescription1"
            }
        };

        public static OpenApiLink ReferencedLink = new OpenApiLink
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Link,
                Id = "example1",
            },
            OperationId = "operationId1",
            Parameters =
            {
                ["parameter1"] = new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build("$request.path.id")
                }
            },
            RequestBody = new RuntimeExpressionAnyWrapper
            {
                Any = new OpenApiObject
                {
                    ["property1"] = new OpenApiBoolean(true)
                }
            },
            Description = "description1",
            Server = new OpenApiServer
            {
                Description = "serverDescription1"
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiLinkTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeAdvancedLinkAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""operationId"": ""operationId1"",
  ""parameters"": {
    ""parameter1"": ""$request.path.id""
  },
  ""requestBody"": {
    ""property1"": true
  },
  ""description"": ""description1"",
  ""server"": {
    ""description"": ""serverDescription1""
  }
}";

            // Act
            AdvancedLink.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedLinkAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/links/example1""
}";

            // Act
            ReferencedLink.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedLinkAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""operationId"": ""operationId1"",
  ""parameters"": {
    ""parameter1"": ""$request.path.id""
  },
  ""requestBody"": {
    ""property1"": true
  },
  ""description"": ""description1"",
  ""server"": {
    ""description"": ""serverDescription1""
  }
}";

            // Act
            ReferencedLink.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}