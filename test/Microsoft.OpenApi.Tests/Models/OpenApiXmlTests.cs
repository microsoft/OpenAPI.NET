// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    /// <summary>
    /// Test cases for <see cref="OpenApiXml"/>.
    /// </summary>
    public class OpenApiXmlTests
    {
        public static OpenApiXml BasicXml = new OpenApiXml();
        public static OpenApiXml AdvancedXml = new OpenApiXml()
        {
            Name = "animal",
            Namespace = new Uri("http://swagger.io/schema/sample"),
            Prefix = "sample",
            Wrapped = true,
            Attribute = true,
            Extensions = new Dictionary<string, IOpenApiAny>
            {
                { "x-xml-extension", new OpenApiInteger(7) }
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)]
        public void SerializeBasicXmlWorks(OpenApiSpecVersion version,
            OpenApiFormat format)
        {
            // Act
            string actual = BasicXml.Serialize(version, format);

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
            string expected =
@"{
  ""name"": ""animal"",
  ""namespace"": ""http://swagger.io/schema/sample"",
  ""prefix"": ""sample"",
  ""attribute"": true,
  ""wrapped"": true,
  ""x-xml-extension"": 7
}";

            // Act
            string actual = AdvancedXml.SerializeAsJson(version);

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
            string expected =
@"name: animal
namespace: http://swagger.io/schema/sample
prefix: sample
attribute: true
wrapped: true
x-xml-extension: 7";

            // Act
            string actual = AdvancedXml.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
