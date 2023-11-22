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
    public class OpenApiLicenseTests
    {
        public static OpenApiLicense BasicLicense = new()
        {
            Name = "Apache 2.0"
        };

        public static OpenApiLicense AdvanceLicense = new()
        {
            Name = "Apache 2.0",
            Url = new("http://www.apache.org/licenses/LICENSE-2.0.html"),
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-copyright", new OpenApiAny("Abc")}
            }
        };

        public static OpenApiLicense LicenseWithIdentifier = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Identifier = "Apache-2.0"
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeBasicLicenseAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "Apache 2.0"
                }
                """;

            // Act
            var actual = BasicLicense.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeBasicLicenseAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected = "name: Apache 2.0";

            // Act
            var actual = BasicLicense.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceLicenseAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "Apache 2.0",
                  "url": "http://www.apache.org/licenses/LICENSE-2.0.html",
                  "x-copyright": "Abc"
                }
                """;

            // Act
            var actual = AdvanceLicense.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceLicenseAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                name: Apache 2.0
                url: http://www.apache.org/licenses/LICENSE-2.0.html
                x-copyright: Abc
                """;

            // Act
            var actual = AdvanceLicense.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldCopyFromOriginalObjectWithoutMutating()
        {
            // Arrange
            var licenseCopy = new OpenApiLicense(AdvanceLicense);

            // Act
            licenseCopy.Name = "";
            licenseCopy.Url = new("https://exampleCopy.com");

            // Assert
            Assert.NotEqual(AdvanceLicense.Name, licenseCopy.Name);
            Assert.NotEqual(AdvanceLicense.Url, licenseCopy.Url);
        }

        [Fact]
        public void SerializeLicenseWithIdentifierAsJsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""name"": ""Apache 2.0"",
  ""identifier"": ""Apache-2.0""
}";

            // Act
            var actual = LicenseWithIdentifier.SerializeAsJson(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeLicenseWithIdentifierAsYamlWorks()
        {
            // Arrange
            var expected = @"name: Apache 2.0
identifier: Apache-2.0";

            // Act
            var actual = LicenseWithIdentifier.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
