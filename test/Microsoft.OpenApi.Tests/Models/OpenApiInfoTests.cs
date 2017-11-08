// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiInfoTests
    {
        public static OpenApiInfo BasicInfo = new OpenApiInfo();
        public static OpenApiInfo AdvanceInfo = new OpenApiInfo()
        {
            Title = "Sample Pet Store App",
            Description = "This is a sample server for a pet store.",
            TermsOfService = new Uri("http://example.com/terms/"),
            Contact = OpenApiContactTests.AdvanceContact,
            License = OpenApiLicenseTests.AdvanceLicense,
            Version = new Version(1, 1, 1),
            Extensions = new Dictionary<string, IOpenApiAny>
            {
                { "x-updated", new OpenApiString("metadata") }
            }
        };

        public static IEnumerable<object[]> BasicInfoJsonExpect()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion, 
@"{
  ""title"": ""Default Title"",
  ""version"": ""1.0""
}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(BasicInfoJsonExpect))]
        public void SerializeBasicInfoAsJsonWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            string actual = BasicInfo.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> BasicInfoYamlExpect()
        {
            var specVersions = new[] { OpenApiSpecVersion.OpenApi3_0, OpenApiSpecVersion.OpenApi2_0 };
            foreach (var specVersion in specVersions)
            {
                yield return new object[]
                {
                    specVersion, 
@"title: Default Title
version: '1.0'"
                };
            }
        }

        [Theory]
        [MemberData(nameof(BasicInfoYamlExpect))]
        public void SerializeBasicInfoAsYamlWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            string actual = BasicInfo.SerializeAsYaml(version);

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
@"{
  ""title"": ""Sample Pet Store App"",
  ""description"": ""This is a sample server for a pet store."",
  ""termsOfService"": ""http://example.com/terms/"",
  ""contact"": {
    ""name"": ""API Support"",
    ""url"": ""http://www.example.com/support"",
    ""email"": ""support@example.com"",
    ""x-internal-id"": 42
  },
  ""license"": {
    ""name"": ""Apache 2.0"",
    ""url"": ""http://www.apache.org/licenses/LICENSE-2.0.html"",
    ""x-copyright"": ""Abc""
  },
  ""version"": ""1.1.1"",
  ""x-updated"": ""metadata""
}"
                };
            }
        }

        [Theory]
        [MemberData(nameof(AdvanceInfoJsonExpect))]
        public void SerializeAdvanceInfoAsJsonWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            string actual = AdvanceInfo.SerializeAsJson(version);

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
@"title: Sample Pet Store App
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
version: 1.1.1
x-updated: metadata"
                };
            }
        }

        [Theory]
        [MemberData(nameof(AdvanceInfoYamlExpect))]
        public void SerializeAdvanceInfoAsYamlWorks(OpenApiSpecVersion version, string expected)
        {
            // Arrange & Act
            string actual = AdvanceInfo.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
