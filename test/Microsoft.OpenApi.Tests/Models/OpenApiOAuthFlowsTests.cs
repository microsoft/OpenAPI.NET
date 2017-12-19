// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiOAuthFlowsTests
    {
        public static OpenApiOAuthFlows BasicOAuthFlows = new OpenApiOAuthFlows();

        public static OpenApiOAuthFlows OAuthFlowsWithSingleFlow = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://example.com/authorization"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName1"] = "description1",
                    ["scopeName2"] = "description2"
                }
            }
        };

        public static OpenApiOAuthFlows OAuthFlowsWithMultipleFlows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://example.com/authorization"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName1"] = "description1",
                    ["scopeName2"] = "description2"
                }
            },
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("http://example.com/token"),
                RefreshUrl = new Uri("http://example.com/refresh"),
                Scopes = new Dictionary<string, string>
                {
                    ["scopeName3"] = "description3",
                    ["scopeName4"] = "description4"
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiOAuthFlowsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicOAuthFlowsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{ }";

            // Act
            var actual = BasicOAuthFlows.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicOAuthFlowsAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"{ }";

            // Act
            var actual = BasicOAuthFlows.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOAuthFlowsWithSingleFlowAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""implicit"": {
    ""authorizationUrl"": ""http://example.com/authorization"",
    ""scopes"": {
      ""scopeName1"": ""description1"",
      ""scopeName2"": ""description2""
    }
  }
}";

            // Act
            var actual = OAuthFlowsWithSingleFlow.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOAuthFlowsWithMultipleFlowsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""implicit"": {
    ""authorizationUrl"": ""http://example.com/authorization"",
    ""scopes"": {
      ""scopeName1"": ""description1"",
      ""scopeName2"": ""description2""
    }
  },
  ""password"": {
    ""tokenUrl"": ""http://example.com/token"",
    ""refreshUrl"": ""http://example.com/refresh"",
    ""scopes"": {
      ""scopeName3"": ""description3"",
      ""scopeName4"": ""description4""
    }
  }
}";

            // Act
            var actual = OAuthFlowsWithMultipleFlows.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}