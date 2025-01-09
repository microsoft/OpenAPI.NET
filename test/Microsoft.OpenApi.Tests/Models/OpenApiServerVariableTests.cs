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
    public class OpenApiServerVariableTests
    {
        public static OpenApiServerVariable BasicServerVariable = new();

        public static OpenApiServerVariable AdvancedServerVariable = new()
        {
            Default = "8443",
            Enum = new()
            {
                "8443",
                "443"
            },
            Description = "test description"
        };

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public async Task SerializeBasicServerVariableAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = await BasicServerVariable.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedServerVariableAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "default": "8443",
                  "description": "test description",
                  "enum": [
                    "8443",
                    "443"
                  ]
                }
                """;

            // Act
            var actual = await AdvancedServerVariable.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedServerVariableAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                default: '8443'
                description: test description
                enum:
                  - '8443'
                  - '443'
                """;

            // Act
            var actual = await AdvancedServerVariable.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
