﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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

        [Fact]
        public void SerializeBasicOAuthFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "scopes": { }
                }
                """;

            // Act
            var actual = BasicOAuthFlow.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicOAuthFlowAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"scopes: { }";

            // Act
            var actual = BasicOAuthFlow.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializePartialOAuthFlowAsV3JsonWorks()
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
            var actual = PartialOAuthFlow.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeCompleteOAuthFlowAsV3JsonWorks()
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
            var actual = CompleteOAuthFlow.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
