﻿// ------------------------------------------------------------
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
    public class OpenApiContactTests
    {
        public static OpenApiContact BasicContact = new OpenApiContact();
        public static OpenApiContact AdvanceContact = new OpenApiContact()
        {
            Name = "API Support",
            Url = new Uri("http://www.example.com/support"),
            Email = "support@example.com",
            Extensions = new Dictionary<string, IOpenApiAny>
            {
                { "x-internal-id", new OpenApiInteger(42) }
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml, "")]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml, "")]
        public void SerializeBasicContactWorks(OpenApiSpecVersion version,
            OpenApiFormat format, string expected)
        {
            // Arrange & Act
            string actual = BasicContact.Serialize(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceContactAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            string expected = 
@"{
  ""name"": ""API Support"",
  ""url"": ""http://www.example.com/support"",
  ""email"": ""support@example.com"",
  ""x-internal-id"": 42
}";

            // Act
            string actual = AdvanceContact.SerializeAsJson(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public void SerializeAdvanceContactAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            string expected = 
@"name: API Support
url: http://www.example.com/support
email: support@example.com
x-internal-id: 42";

            // Act
            string actual = AdvanceContact.SerializeAsYaml(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
