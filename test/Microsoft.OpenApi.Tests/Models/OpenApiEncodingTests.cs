// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
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
        public async Task SerializeBasicEncodingAsV3Works(OpenApiFormat format, string expected)
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
    }
}
