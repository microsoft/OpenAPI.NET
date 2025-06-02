// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
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
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                {"x-copyright", new JsonNodeExtension("Abc")}
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
        public async Task SerializeBasicLicenseAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "Apache 2.0"
                }
                """;

            // Act
            var actual = await BasicLicense.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeBasicLicenseAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected = "name: Apache 2.0";

            // Act
            var actual = await BasicLicense.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvanceLicenseAsJsonWorks(OpenApiSpecVersion version)
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
            var actual = await AdvanceLicense.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvanceLicenseAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                name: Apache 2.0
                url: http://www.apache.org/licenses/LICENSE-2.0.html
                x-copyright: Abc
                """;

            // Act
            var actual = await AdvanceLicense.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
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
        public async Task SerializeLicenseWithIdentifierAsJsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""name"": ""Apache 2.0"",
  ""identifier"": ""Apache-2.0""
}";

            // Act
            var actual = await LicenseWithIdentifier.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeLicenseWithIdentifierAsYamlWorks()
        {
            // Arrange
            var expected = @"name: Apache 2.0
identifier: Apache-2.0";

            // Act
            var actual = await LicenseWithIdentifier.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
