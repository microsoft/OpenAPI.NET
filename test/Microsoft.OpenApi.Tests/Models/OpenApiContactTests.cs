// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
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
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-internal-id", new OpenApiAny(42)}
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicContactWorks(
            OpenApiSpecVersion version,
            OpenApiFormat format,
            string expected)
        {
            // Arrange & Act
            var actual = BasicContact.Serialize(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceContactAsJsonWorks(OpenApiSpecVersion version)
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
            var actual = AdvanceContact.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceContactAsYamlWorks(OpenApiSpecVersion version)
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
            var actual = AdvanceContact.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
