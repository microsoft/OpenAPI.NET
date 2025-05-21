﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiXmlTests
    {
        public static OpenApiXml AdvancedXml = new()
        {
            Name = "animal",
            Namespace = new("http://swagger.io/schema/sample"),
            Prefix = "sample",
            Wrapped = true,
            Attribute = true,
            Extensions = new()
            {
                {"x-xml-extension", new JsonNodeExtension(7)}
            }
        };

        public static OpenApiXml BasicXml = new();

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Yaml)]
        public async Task SerializeBasicXmlWorks(
            OpenApiSpecVersion version,
            string format)
        {
            // Act
            var actual = await BasicXml.SerializeAsync(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal("{ }", actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvancedXmlAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "animal",
                  "namespace": "http://swagger.io/schema/sample",
                  "prefix": "sample",
                  "attribute": true,
                  "wrapped": true,
                  "x-xml-extension": 7
                }
                """;

            // Act
            var actual = await AdvancedXml.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvancedXmlAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                name: animal
                namespace: http://swagger.io/schema/sample
                prefix: sample
                attribute: true
                wrapped: true
                x-xml-extension: 7
                """;

            // Act
            var actual = await AdvancedXml.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
