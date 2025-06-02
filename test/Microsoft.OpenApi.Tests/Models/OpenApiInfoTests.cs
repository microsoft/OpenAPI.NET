// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiInfoTests
    {
        public static OpenApiInfo AdvanceInfo = new()
        {
            Title = "Sample Pet Store App",
            Description = "This is a sample server for a pet store.",
            TermsOfService = new("http://example.com/terms/"),
            Contact = OpenApiContactTests.AdvanceContact,
            License = OpenApiLicenseTests.AdvanceLicense,
            Version = "1.1.1",
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                {"x-updated", new JsonNodeExtension("metadata")}
            }
        };

        public static OpenApiInfo InfoWithSummary = new()
        {
            Title = "Sample Pet Store App",
            Summary = "This is a sample server for a pet store.",
            Description = "This is a sample server for a pet store.",
            Version = "1.1.1",
        };

        public static OpenApiInfo BasicInfo = new OpenApiInfo
        {
            Title = "Sample Pet Store App",
            Version = "1.0"
        };

        public static IEnumerable<object[]> BasicInfoJsonExpected()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion,
                    """
                    {
                      "title": "Sample Pet Store App",
                      "version": "1.0"
                    }
                    """
                };
            }
        }

        [Theory]
        [MemberData(nameof(BasicInfoJsonExpected))]
        public async Task SerializeBasicInfoAsJsonWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            var actual = await BasicInfo.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> BasicInfoYamlExpected()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion,
                    """
                    title: Sample Pet Store App
                    version: '1.0'
                    """
                };
            }
        }

        [Theory]
        [MemberData(nameof(BasicInfoYamlExpected))]
        public async Task SerializeBasicInfoAsYamlWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            var actual = await BasicInfo.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> AdvanceInfoJsonExpect()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion,
                    """
                    {
                      "title": "Sample Pet Store App",
                      "description": "This is a sample server for a pet store.",
                      "termsOfService": "http://example.com/terms/",
                      "contact": {
                        "name": "API Support",
                        "url": "http://www.example.com/support",
                        "email": "support@example.com",
                        "x-internal-id": 42
                      },
                      "license": {
                        "name": "Apache 2.0",
                        "url": "http://www.apache.org/licenses/LICENSE-2.0.html",
                        "x-copyright": "Abc"
                      },
                      "version": "1.1.1",
                      "x-updated": "metadata"
                    }
                    """
                };
            }
        }

        [Theory]
        [MemberData(nameof(AdvanceInfoJsonExpect))]
        public async Task SerializeAdvanceInfoAsJsonWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            var actual = await AdvanceInfo.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> AdvanceInfoYamlExpect()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion,
                    """
                    title: Sample Pet Store App
                    description: This is a sample server for a pet store.
                    termsOfService: http://example.com/terms/
                    contact:
                      name: API Support
                      url: http://www.example.com/support
                      email: support@example.com
                      x-internal-id: 42
                    license:
                      name: Apache 2.0
                      url: http://www.apache.org/licenses/LICENSE-2.0.html
                      x-copyright: Abc
                    version: '1.1.1'
                    x-updated: metadata
                    """
                };
            }
        }

        [Theory]
        [MemberData(nameof(AdvanceInfoYamlExpect))]
        public async Task SerializeAdvanceInfoAsYamlWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            var actual = await AdvanceInfo.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task InfoVersionShouldAcceptDateStyledAsVersions()
        {
            // Arrange
            var info = new OpenApiInfo
            {
                Title = "Sample Pet Store App",
                Version = "2017-03-01"
            };

            var expected =
                """
                title: Sample Pet Store App
                version: '2017-03-01'
                """;

            // Act
            var actual = await info.SerializeAsync(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeInfoObjectWithSummaryAsV31YamlWorks()
        {
            // Arrange
            var expected = @"title: Sample Pet Store App
description: This is a sample server for a pet store.
version: '1.1.1'
summary: This is a sample server for a pet store.";

            // Act
            var actual = await InfoWithSummary.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeInfoObjectWithSummaryAsV31JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""Sample Pet Store App"",
  ""description"": ""This is a sample server for a pet store."",
  ""version"": ""1.1.1"",
  ""summary"": ""This is a sample server for a pet store.""
}";

            // Act
            var actual = await InfoWithSummary.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
