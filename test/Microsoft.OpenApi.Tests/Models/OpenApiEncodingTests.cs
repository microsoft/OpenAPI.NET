// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
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
        [InlineData(OpenApiConstants.Json, "{ }")]
        [InlineData(OpenApiConstants.Yaml, "{ }")]
        public async Task SerializeBasicEncodingAsV3Works(string format, string expected)
        {
            // Arrange & Act
            var actual = await BasicEncoding.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvanceEncodingAsV3JsonWorks()
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
            var actual = await AdvanceEncoding.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvanceEncodingAsV3YamlWorks()
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
            var actual = await AdvanceEncoding.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
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
            Assert.Equal(parameter.Explode, expectedExplode);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(null, false)]
        public async Task WhenExplodeIsSetOutputShouldHaveExplode(bool? expectedExplode, bool hasExplode)
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
            var actual = await parameter.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(actual, expected);
        }

        [Fact]
        public async Task SerializeEncodingWithNestedEncodingAsV32JsonWorks()
        {
            // Arrange
            var encoding = new OpenApiEncoding
            {
                ContentType = "application/json",
                Encoding = new Dictionary<string, OpenApiEncoding>
                {
                    ["nestedField"] = new OpenApiEncoding
                    {
                        ContentType = "application/xml",
                        Style = ParameterStyle.Form,
                        Explode = true
                    },
                    ["anotherField"] = new OpenApiEncoding
                    {
                        ContentType = "text/plain"
                    }
                }
            };

            var expected =
                """
                {
                  "contentType": "application/json",
                  "encoding": {
                    "nestedField": {
                      "contentType": "application/xml",
                      "style": "form",
                      "explode": true
                    },
                    "anotherField": {
                      "contentType": "text/plain"
                    }
                  }
                }
                """;

            // Act
            var actual = await encoding.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeEncodingWithNestedEncodingAsV32YamlWorks()
        {
            // Arrange
            var encoding = new OpenApiEncoding
            {
                ContentType = "application/json",
                Encoding = new Dictionary<string, OpenApiEncoding>
                {
                    ["nestedField"] = new OpenApiEncoding
                    {
                        ContentType = "application/xml",
                        Style = ParameterStyle.Form,
                        Explode = true
                    },
                    ["anotherField"] = new OpenApiEncoding
                    {
                        ContentType = "text/plain"
                    }
                }
            };

            var expected =
                """
                contentType: application/json
                encoding:
                  nestedField:
                    contentType: application/xml
                    style: form
                    explode: true
                  anotherField:
                    contentType: text/plain
                """;

            // Act
            var actual = await encoding.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
