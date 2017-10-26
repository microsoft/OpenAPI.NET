// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Serialization.Tests
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
        [InlineData(OpenApiFormat.Yaml, "")]
        public void SerializeBasicExternalDocsAsV3Works(OpenApiFormat format, string expect)
        {
            // Arrange & Act
            string actual = BasicExDocs.Serialize(format, (e, w) => e.SerializeV3(w));

            // Assert
            Assert.Equal(expect, actual);
        }

        [Fact]
        public void SerializeAdvanceExDocssAsV3JsonWorks()
        {
            // Arrange
            string expect = @"
{
  ""description"": ""Find more info here"",
  ""url"": ""https://example.com""
}";

            // Act
            string actual = AdvanceExDocs.SerializeAsJson((e, w) => e.SerializeV3(w));

            // Assert
            Assert.Equal(expect, actual);
        }

        [Fact]
        public void SerializeAdvanceExDocssAsV3YamlWorks()
        {
            // Arrange
            string expect = @"
description: Find more info here
url: https://example.com";

            // Act
            string actual = AdvanceExDocs.SerializeAsYaml((e, w) => e.SerializeV3(w));

            // Assert
            Assert.Equal(expect, actual);
        }

        #endregion

        #region OpenAPI V2

        #endregion
    }
}
