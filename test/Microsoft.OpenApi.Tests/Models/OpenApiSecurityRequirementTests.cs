// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Microsoft.OpenApi.Models.References;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSecurityRequirementTests
    {
        public static OpenApiSecurityRequirement BasicSecurityRequirement = new();

        public static OpenApiSecurityRequirement SecurityRequirementWithReferencedSecurityScheme =
            new()
            {
                [
                    new OpenApiSecuritySchemeReference("scheme1", hostDocument: null)
                ] = new List<string>
                {
                    "scope1",
                    "scope2",
                    "scope3",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme2", hostDocument: null)
                ] = new List<string>
                {
                    "scope4",
                    "scope5",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme3", hostDocument: null)
                ] = new List<string>()
            };

        public static OpenApiSecurityRequirement SecurityRequirementWithUnreferencedSecurityScheme =
            new()
            {
                [
                    new OpenApiSecuritySchemeReference("scheme1", hostDocument: null)
                ] = new List<string>
                {
                    "scope1",
                    "scope2",
                    "scope3",
                },
                [
                    new()
                    {
                        // This security scheme is unreferenced, so this key value pair cannot be serialized.
                        Name = "brokenUnreferencedScheme"
                    }
                ] = new List<string>
                {
                    "scope4",
                    "scope5",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme3", hostDocument: null)
                ] = new List<string>()
            };

        [Fact]
        public void SerializeBasicSecurityRequirementAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSecurityRequirement.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSecurityRequirementAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            SecurityRequirementWithReferencedSecurityScheme.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeSecurityRequirementWithReferencedSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "scheme1": [
                    "scope1",
                    "scope2",
                    "scope3"
                  ],
                  "scheme2": [
                    "scope4",
                    "scope5"
                  ],
                  "scheme3": [ ]
                }
                """;

            // Act
            var actual = SecurityRequirementWithReferencedSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSecurityRequirementWithReferencedSecuritySchemeAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "scheme1": [
                    "scope1",
                    "scope2",
                    "scope3"
                  ],
                  "scheme2": [
                    "scope4",
                    "scope5"
                  ],
                  "scheme3": [ ]
                }
                """;

            // Act
            var actual = SecurityRequirementWithReferencedSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSecurityRequirementWithUnreferencedSecuritySchemeAsV3JsonShouldSkipUnserializableKeyValuePair()
        {
            // Arrange
            var expected =
                """
                {
                  "scheme1": [
                    "scope1",
                    "scope2",
                    "scope3"
                  ],
                  "scheme3": [ ]
                }
                """;

            // Act
            var actual = SecurityRequirementWithUnreferencedSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSecurityRequirementWithUnreferencedSecuritySchemeAsV2JsonShouldSkipUnserializableKeyValuePair()
        {
            // Arrange
            var expected =
                """
                {
                  "scheme1": [
                    "scope1",
                    "scope2",
                    "scope3"
                  ],
                  "scheme3": [ ]
                }
                """;

            // Act
            var actual =
                SecurityRequirementWithUnreferencedSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SchemesShouldConsiderOnlyReferenceIdForEquality()
        {
            // Arrange
            var securityRequirement = new OpenApiSecurityRequirement();

            var securityScheme1 = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "apiKeyName1",
                In = ParameterLocation.Header,
                Reference = new()
                {
                    Id = "securityScheme1",
                    Type = ReferenceType.SecurityScheme
                }
            };

            var securityScheme2 = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new("http://example.com"),
                Reference = new()
                {
                    Id = "securityScheme2",
                    Type = ReferenceType.SecurityScheme
                }
            };

            var securityScheme1Duplicate = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "apiKeyName1",
                In = ParameterLocation.Header,
                Reference = new()
                {
                    Id = "securityScheme1",
                    Type = ReferenceType.SecurityScheme
                }
            };

            var securityScheme1WithDifferentProperties = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "apiKeyName2",
                In = ParameterLocation.Query,
                Reference = new()
                {
                    Id = "securityScheme1",
                    Type = ReferenceType.SecurityScheme
                }
            };

            // Act
            securityRequirement.Add(securityScheme1, new List<string>());
            securityRequirement.Add(securityScheme2, new List<string> { "scope1", "scope2" });

            var addSecurityScheme1Duplicate = () =>
                securityRequirement.Add(securityScheme1Duplicate, new List<string>());
            var addSecurityScheme1WithDifferentProperties = () =>
                securityRequirement.Add(securityScheme1WithDifferentProperties, new List<string>());

            // Assert
            // Only the first two should be added successfully since the latter two are duplicates of securityScheme1.
            // Duplicate determination only considers Reference.Id.
            addSecurityScheme1Duplicate.Should().Throw<ArgumentException>();
            addSecurityScheme1WithDifferentProperties.Should().Throw<ArgumentException>();

            securityRequirement.Should().HaveCount(2);

            securityRequirement.Should().BeEquivalentTo(
                new OpenApiSecurityRequirement
                {
                    // This should work with any security scheme object
                    // as long as Reference.Id os securityScheme1
                    [securityScheme1WithDifferentProperties] = new List<string>(),
                    [securityScheme2] = new List<string> { "scope1", "scope2" },
                });
        }
    }
}
