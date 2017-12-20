// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        public static OpenApiMediaType BasicMediaType = new OpenApiMediaType();

        public static OpenApiMediaType AdvanceMediaType = new OpenApiMediaType
        {
            Example = new OpenApiInteger(42),
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                {"testEncoding", OpenApiEncodingTests.AdvanceEncoding}
            }
        };

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
    }
}