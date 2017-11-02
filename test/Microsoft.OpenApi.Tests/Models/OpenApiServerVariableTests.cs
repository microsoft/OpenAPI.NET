// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiServerVariableTests
    {
        public static OpenApiServerVariable BasicVariable = new OpenApiServerVariable();
        public static OpenApiServerVariable AdvanceVariable = new OpenApiServerVariable()
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
        [InlineData(OpenApiFormat.Yaml, "")]
        public void SerializeBasicServerVariableAsV3Works(OpenApiFormat format, string expect)
        {
            // Arrange & Act
            string actual = BasicVariable.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual.Should().Be(expect);
        }

        [Fact]
        public void SerializeAdvanceServerVariableAsV3JsonWorks()
        {
            // Arrange
            string expect = 
@"{
  ""default"": ""8443"",
  ""description"": ""test description"",
  ""enum"": [
    ""8443"",
    ""443""
  ]
}";

            // Act
            string actual = AdvanceVariable.SerializeAsJson();

            // Assert
            actual.Should().Be(expect);
        }

        [Fact]
        public void SerializeAdvanceServerVariableAsV3YamlWorks()
        {
            // Arrange
            string expect = 
@"default: 8443
description: test description
enum:
  - 8443
  - 443";

            // Act
            string actual = AdvanceVariable.SerializeAsYaml();

            // Assert
            actual.Should().Be(expect);
        }
    }
}