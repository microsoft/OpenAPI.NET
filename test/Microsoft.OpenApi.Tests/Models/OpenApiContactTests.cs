// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiContactTests
    {
        public static OpenApiContact BasicContact = new();

        public static OpenApiContact AdvanceContact = new()
        {
            Name = "API Support",
            Url = new("http://www.example.com/support"),
            Email = "support@example.com",
            Extensions = new()
            {
                {"x-internal-id", new JsonNodeExtension(42)}
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Yaml, "{ }")]
        public async Task SerializeBasicContactWorks(
            OpenApiSpecVersion version,
            string format,
            string expected)
        {
            // Arrange & Act
            var actual = await BasicContact.SerializeAsync(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvanceContactAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "API Support",
                  "url": "http://www.example.com/support",
                  "email": "support@example.com",
                  "x-internal-id": 42
                }
                """;

            // Act
            var actual = await AdvanceContact.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvanceContactAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                name: API Support
                url: http://www.example.com/support
                email: support@example.com
                x-internal-id: 42
                """;

            // Act
            var actual = await AdvanceContact.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
