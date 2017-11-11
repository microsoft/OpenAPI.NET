// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiMediaTypeTests
    {
        public static OpenApiMediaType BasicMediaType = new OpenApiMediaType();
        public static OpenApiMediaType AdvanceMediaType = new OpenApiMediaType()
        {
            Example = new OpenApiInteger(42),
            Encoding = new Dictionary<string, OpenApiEncoding>
            {
                { "testEncoding", OpenApiEncodingTests.AdvanceEncoding }
            }
        };

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicMediaTypeAsV3Works(OpenApiFormat format, string expect)
        {
            // Arrange & Act
            string actual = BasicMediaType.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            Assert.Equal(expect, actual);
        }

        [Fact]
        public void SerializeAdvanceMediaTypeAsV3JsonWorks()
        {
            // Arrange
            string expected = 
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
            string actual = AdvanceMediaType.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceMediaTypeAsV3YamlWorks()
        {
            // Arrange
            string expected = 
@"example: 42
encoding:
  testEncoding:
    contentType: image/png, image/jpeg
    style: simple
    explode: true
    allowReserved: true";

            // Act
            string actual = AdvanceMediaType.SerializeAsYaml();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
