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
    public class OpenApiServerTests
    {
        public static OpenApiServer BasicServer = new OpenApiServer
        {
            Description = "description1",
            Url = "https://example.com/server1"
        };

        public static OpenApiServer AdvancedServer = new OpenApiServer
        {
            Description = "description1",
            Url = "https://{username}.example.com:{port}/{basePath}",
            Variables = new Dictionary<string, OpenApiServerVariable>
            {
                ["username"] = new OpenApiServerVariable
                {
                    Default = "unknown",
                    Description = "variableDescription1",
                },
                ["port"] = new OpenApiServerVariable
                {
                    Default = "8443",
                    Description = "variableDescription2",
                    Enum = new List<string>
                    {
                        "443",
                        "8443"
                    }
                },
                ["basePath"] = new OpenApiServerVariable
                {
                    Default = "v1"
                },
            }
        };

        [Fact]
        public void SerializeBasicServerAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""url"": ""https://example.com/server1"",
  ""description"": ""description1""
}";

            // Act
            var actual = BasicServer.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedServerAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""url"": ""https://{username}.example.com:{port}/{basePath}"",
  ""description"": ""description1"",
  ""variables"": {
    ""username"": {
      ""default"": ""unknown"",
      ""description"": ""variableDescription1""
    },
    ""port"": {
      ""default"": ""8443"",
      ""description"": ""variableDescription2"",
      ""enum"": [
        ""443"",
        ""8443""
      ]
    },
    ""basePath"": {
      ""default"": ""v1""
    }
  }
}";

            // Act
            var actual = AdvancedServer.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}