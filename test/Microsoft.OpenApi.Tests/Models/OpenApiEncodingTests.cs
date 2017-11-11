// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiEncodingTests
    {
        public static OpenApiEncoding BasicEncoding = new OpenApiEncoding();
        public static OpenApiEncoding AdvanceEncoding = new OpenApiEncoding()
        {
            ContentType = "image/png, image/jpeg",
            Style = ParameterStyle.Simple,
            Explode = true,
            AllowReserved = true,
        };

        [Theory]
        [InlineData(OpenApiFormat.Json, "{ }")]
        [InlineData(OpenApiFormat.Yaml, "{ }")]
        public void SerializeBasicEncodingAsV3Works(OpenApiFormat format, string expected)
        {
            // Arrange & Act
            string actual = BasicEncoding.Serialize(OpenApiSpecVersion.OpenApi3_0, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceEncodingAsV3JsonWorks()
        {
            // Arrange
            string expected = 
@"{
  ""contentType"": ""image/png, image/jpeg"",
  ""style"": ""simple"",
  ""explode"": true,
  ""allowReserved"": true
}";

            // Act
            string actual = AdvanceEncoding.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvanceEncodingAsV3YamlWorks()
        {
            // Arrange
            string expected = 
@"contentType: image/png, image/jpeg
style: simple
explode: true
allowReserved: true";

            // Act
            string actual = AdvanceEncoding.SerializeAsYaml();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
