// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiExternalDocsTests
    {
        public static OpenApiExternalDocs BasicExDocs = new OpenApiExternalDocs();

        public static OpenApiExternalDocs AdvanceExDocs = new OpenApiExternalDocs
        {
            Url = new Uri("https://example.com"),
            Description = "Find more info here"
        };

        #region OpenAPI V3

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicExternalDocsAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            var actual = BasicExDocs.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceExDocsAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""description"": ""Find more info here"",
  ""url"": ""https://example.com""
}";

            // Act
            var actual = AdvanceExDocs.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceExDocsAsV3YamlWorks()
        {
            // Arrange
            var expected =
                @"description: Find more info here
url: https://example.com";

            // Act
            var actual = AdvanceExDocs.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        #endregion

        #region OpenAPI V2

        #endregion
    }
}