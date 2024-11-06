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
    public class OpenApiXmlTests
    {
        public static OpenApiXml AdvancedXml = new()
        {
            Name = "animal",
            Namespace = new("http://swagger.io/schema/sample"),
            Prefix = "sample",
            Wrapped = true,
            Attribute = true,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-xml-extension", new OpenApiAny(7)}
            }
        };

        public static OpenApiXml BasicXml = new();

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)]
        public void SerializeBasicXmlWorks(
            OpenApiSpecVersion version,
            OpenApiFormat format)
        {
            // Act
            var actual = BasicXml.Serialize(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be("{ }");
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvancedXmlAsJsonWorks(OpenApiSpecVersion version)
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
            var actual = AdvancedXml.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvancedXmlAsYamlWorks(OpenApiSpecVersion version)
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
            var actual = AdvancedXml.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
