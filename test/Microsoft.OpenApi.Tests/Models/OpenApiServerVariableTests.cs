// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiServerVariableTests
    {
        public static OpenApiServerVariable BasicServerVariable = new OpenApiServerVariable();

        public static OpenApiServerVariable AdvancedServerVariable = new OpenApiServerVariable
        {
            Default = "8443",
            Enum = new List<string>
            {
                "8443",
                "443"
            },
            Description = "test description"
        };

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicServerVariableAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = BasicServerVariable.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedServerVariableAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""default"": ""8443"",
  ""description"": ""test description"",
  ""enum"": [
    ""8443"",
    ""443""
  ]
}";

            // Act
            var actual = AdvancedServerVariable.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedServerVariableAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"default: '8443'
description: test description
enum:
  - '8443'
  - '443'";

            // Act
            var actual = AdvancedServerVariable.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}