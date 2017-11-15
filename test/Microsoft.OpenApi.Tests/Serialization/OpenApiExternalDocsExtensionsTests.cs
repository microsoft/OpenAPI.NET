// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Serialization
{
    public class OpenApiExternalDocsExtensionsTests
    {
        public static OpenApiExternalDocs BasicExDocs = new OpenApiExternalDocs();
        public static OpenApiExternalDocs AdvanceExDocs = new OpenApiExternalDocs()
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
            string actual = BasicExDocs.Serialize(OpenApiSpecVersion.OpenApi3_0, format);
            
            // Assert
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceExDocssAsV3JsonWorks()
        {
            // Arrange
            string expected = 
@"{
  ""description"": ""Find more info here"",
  ""url"": ""https://example.com""
}";

            // Act
            string actual = AdvanceExDocs.SerializeAsJson();

            // Assert
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceExDocssAsV3YamlWorks()
        {
            // Arrange
            string expected = 
@"description: Find more info here
url: https://example.com";

            // Act
            string actual = AdvanceExDocs.SerializeAsYaml();

            // Assert
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        #endregion

        #region OpenAPI V2

        #endregion
    }
}
