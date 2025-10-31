﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiOAuthFlowsTests
    {
        public static OpenApiOAuthFlows BasicOAuthFlows = new();

        public static OpenApiOAuthFlows OAuthFlowsWithSingleFlow = new()
        {
            Implicit = new()
            {
                AuthorizationUrl = new("http://example.com/authorization"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName1"] = "description1",
                    ["scopeName2"] = "description2"
                }
            }
        };

        public static OpenApiOAuthFlows OAuthFlowsWithMultipleFlows = new()
        {
            Implicit = new()
            {
                AuthorizationUrl = new("http://example.com/authorization"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName1"] = "description1",
                    ["scopeName2"] = "description2"
                }
            },
            Password = new()
            {
                TokenUrl = new("http://example.com/token"),
                RefreshUrl = new("http://example.com/refresh"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName3"] = "description3",
                    ["scopeName4"] = "description4"
                }
            }
        };

        public static OpenApiOAuthFlows OAuthFlowsWithDeviceAuthorization = new()
        {
            DeviceAuthorization = new()
            {
                TokenUrl = new("http://example.com/token"),
                RefreshUrl = new("http://example.com/refresh"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName1"] = "description1",
                    ["scopeName2"] = "description2"
                }
            }
        };

        [Fact]
        public async Task SerializeBasicOAuthFlowsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{ }";

            // Act
            var actual = await BasicOAuthFlows.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeBasicOAuthFlowsAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"{ }";

            // Act
            var actual = await BasicOAuthFlows.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowsWithSingleFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "implicit": {
                    "authorizationUrl": "http://example.com/authorization",
                    "scopes": {
                      "scopeName1": "description1",
                      "scopeName2": "description2"
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowsWithSingleFlow.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowsWithMultipleFlowsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "implicit": {
                    "authorizationUrl": "http://example.com/authorization",
                    "scopes": {
                      "scopeName1": "description1",
                      "scopeName2": "description2"
                    }
                  },
                  "password": {
                    "tokenUrl": "http://example.com/token",
                    "refreshUrl": "http://example.com/refresh",
                    "scopes": {
                      "scopeName3": "description3",
                      "scopeName4": "description4"
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowsWithMultipleFlows.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthFlowsWithDeviceAuthorizationAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "deviceAuthorization": {
                    "tokenUrl": "http://example.com/token",
                    "refreshUrl": "http://example.com/refresh",
                    "scopes": {
                      "scopeName1": "description1",
                      "scopeName2": "description2"
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowsWithDeviceAuthorization.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOAuthFlowsWithDeviceAuthorizationAsV31JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "x-oai-deviceAuthorization": {
                    "tokenUrl": "http://example.com/token",
                    "refreshUrl": "http://example.com/refresh",
                    "scopes": {
                      "scopeName1": "description1",
                      "scopeName2": "description2"
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowsWithDeviceAuthorization.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOAuthFlowsWithDeviceAuthorizationAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "x-oai-deviceAuthorization": {
                    "tokenUrl": "http://example.com/token",
                    "refreshUrl": "http://example.com/refresh",
                    "scopes": {
                      "scopeName1": "description1",
                      "scopeName2": "description2"
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuthFlowsWithDeviceAuthorization.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }
    }
}
