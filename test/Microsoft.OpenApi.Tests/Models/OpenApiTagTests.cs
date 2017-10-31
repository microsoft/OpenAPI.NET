﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Xunit;

namespace Microsoft.OpenApi.Models.Tests
{
    public class OpenApiTagTests
    {
        public static OpenApiTag BasicTag = new OpenApiTag();
        public static OpenApiTag AdvanceTag = new OpenApiTag()
        {
            Name = "pet",
            Description = "Pets operations",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs,
            Extensions = new Dictionary<string, IOpenApiAny>
            {
                { "x-tag-extension", new OpenApiNull() }
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml, "")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml, "")]
        public void SerializeBasicTagWorks(OpenApiSpecVersion version,
            OpenApiFormat format, string expect)
        {
            // Arrange & Act
            string actual = BasicTag.Serialize(version, format);

            // Assert
            Assert.Equal(expect, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceTagAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            string expect = @"
{
  ""name"": ""pet"",
  ""description"": ""Pets operations"",
  ""externalDocs"": {
    ""description"": ""Find more info here"",
    ""url"": ""https://example.com""
  },
  ""x-tag-extension"": null
}";

            // Act
            string actual = AdvanceTag.SerializeAsJson(version);

            // Assert
            Assert.Equal(expect, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceTagAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            string expect = @"
name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension: ";

            // Act
            string actual = AdvanceTag.SerializeAsYaml(version);

            // Assert
            Assert.Equal(expect, actual);
        }
    }
}
