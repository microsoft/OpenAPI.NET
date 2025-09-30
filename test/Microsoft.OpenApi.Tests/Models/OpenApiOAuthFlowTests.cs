// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiOAuthFlowTests
    {
        public static OpenApiOAuthFlow BasicOAuthFlow = new();

        public static OpenApiOAuthFlow PartialOAuthFlow = new()
        {
            AuthorizationUrl = new("http://example.com/authorization"),
            Scopes = new Dictionary<string, string>
            {
                ["scopeName3"] = "description3",
                ["scopeName4"] = "description4"
            }
        };

        public static OpenApiOAuthFlow CompleteOAuthFlow = new()
        {
            AuthorizationUrl = new("http://example.com/authorization"),
            TokenUrl = new("http://example.com/token"),
            RefreshUrl = new("http://example.com/refresh"),
            Scopes = new Dictionary<string, string>
            {
                ["scopeName3"] = "description3",
                ["scopeName4"] = "description4"
            }
        };

        public static OpenApiOAuthFlow OAuthFlowWithDeviceAuthorizationUrl = new()
        {
            AuthorizationUrl = new("http://example.com/authorization"),
            TokenUrl = new("http://example.com/token"),
            RefreshUrl = new("http://example.com/refresh"),
            DeviceAuthorizationUrl = new("http://example.com/device"),
            Scopes = new Dictionary<string, string>
            {
                ["scopeName3"] = "description3",
                ["scopeName4"] = "description4"
            }
        };

        [Fact]
        public async Task SerializeBasicOAuthFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "scopes": { }
                }
                """;

            // Act
            var actual = await BasicOAuthFlow.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeBasicOAuthFlowAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"scopes: { }";

            // Act
            var actual = await BasicOAuthFlow.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializePartialOAuthFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "authorizationUrl": "http://example.com/authorization",
                  "scopes": {
                    "scopeName3": "description3",
                    "scopeName4": "description4"
                  }
                }
                """;

            // Act
            var actual = await PartialOAuthFlow.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeCompleteOAuthFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "authorizationUrl": "http://example.com/authorization",
                  "tokenUrl": "http://example.com/token",
                  "refreshUrl": "http://example.com/refresh",
                  "scopes": {
                    "scopeName3": "description3",
                    "scopeName4": "description4"
                  }
                }
                """;

            // Act
            var actual = await CompleteOAuthFlow.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowWithDeviceAuthorizationUrlAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "authorizationUrl": "http://example.com/authorization",
                  "tokenUrl": "http://example.com/token",
                  "refreshUrl": "http://example.com/refresh",
                  "deviceAuthorizationUrl": "http://example.com/device",
                  "scopes": {
                    "scopeName3": "description3",
                    "scopeName4": "description4"
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowWithDeviceAuthorizationUrl.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowWithDeviceAuthorizationUrlAsV31JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "authorizationUrl": "http://example.com/authorization",
                  "tokenUrl": "http://example.com/token",
                  "refreshUrl": "http://example.com/refresh",
                  "x-oai-deviceAuthorizationUrl": "http://example.com/device",
                  "scopes": {
                    "scopeName3": "description3",
                    "scopeName4": "description4"
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowWithDeviceAuthorizationUrl.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowWithDeviceAuthorizationUrlAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "authorizationUrl": "http://example.com/authorization",
                  "tokenUrl": "http://example.com/token",
                  "refreshUrl": "http://example.com/refresh",
                  "x-oai-deviceAuthorizationUrl": "http://example.com/device",
                  "scopes": {
                    "scopeName3": "description3",
                    "scopeName4": "description4"
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowWithDeviceAuthorizationUrl.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
