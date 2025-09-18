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
        public static OpenApiEncoding BasicEncoding = new();

        public static OpenApiEncoding AdvanceEncoding = new()
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
                """
                {
                  "contentType": "image/png, image/jpeg",
                  "style": "simple",
                  "explode": true,
                  "allowReserved": true
                }
                """;

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
                """
                contentType: 'image/png, image/jpeg'
                style: simple
                explode: true
                allowReserved: true
                """;

            // Act
            var actual = AdvanceEncoding.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(ParameterStyle.Form, true)]
        [InlineData(ParameterStyle.SpaceDelimited, false)]
        [InlineData(null, false)]
        public void WhenStyleIsFormTheDefaultValueOfExplodeShouldBeTrueOtherwiseFalse(ParameterStyle? style, bool expectedExplode)
        {
            // Arrange
            var parameter = new OpenApiEncoding
            {
                Style = style
            };

            // Act & Assert
            parameter.Explode.Should().Be(expectedExplode);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(null, false)]
        public void WhenExplodeIsSetOutputShouldHaveExplode(bool? expectedExplode, bool hasExplode)
        {
            // Arrange
            OpenApiEncoding parameter = new()
            {
                ContentType = "multipart/form-data",
                Style = ParameterStyle.Form,
                Explode = expectedExplode,
            };

            var expected =
                $"""
                contentType: multipart/form-data
                style: form
                """;

            if (hasExplode)
            {
                expected = expected + $"\nexplode: {expectedExplode.ToString().ToLower()}";
            }

            // Act
            var actual = parameter.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
