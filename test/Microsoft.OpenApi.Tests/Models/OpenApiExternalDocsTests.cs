// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiExternalDocsTests
    {
        public static OpenApiExternalDocs BasicExDocs = new();

        public static OpenApiExternalDocs AdvanceExDocs = new()
        {
            Url = new("https://example.com"),
            Description = "Find more info here"
        };

        #region OpenAPI V3

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public async Task SerializeBasicExternalDocsAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = await BasicExDocs.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvanceExDocsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "description": "Find more info here",
                  "url": "https://example.com"
                }
                """;

            // Act
            var actual = await AdvanceExDocs.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvanceExDocsAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                description: Find more info here
                url: https://example.com
                """;

            // Act
            var actual = await AdvanceExDocs.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        #endregion

        #region OpenAPI V2

        #endregion
    }
}
