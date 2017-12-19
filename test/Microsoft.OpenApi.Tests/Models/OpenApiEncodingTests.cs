// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiEncodingTests
    {
        public static OpenApiEncoding BasicEncoding = new OpenApiEncoding();

        public static OpenApiEncoding AdvanceEncoding = new OpenApiEncoding
        {
            ContentType = "image/png, image/jpeg",
            Style = ParameterStyle.Simple,
            Explode = true,
            AllowReserved = true,
        };

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicEncodingAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = BasicEncoding.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceEncodingAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""contentType"": ""image/png, image/jpeg"",
  ""style"": ""simple"",
  ""explode"": true,
  ""allowReserved"": true
}";

            // Act
            var actual = AdvanceEncoding.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceEncodingAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"contentType: 'image/png, image/jpeg'
style: simple
explode: true
allowReserved: true";

            // Act
            var actual = AdvanceEncoding.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}