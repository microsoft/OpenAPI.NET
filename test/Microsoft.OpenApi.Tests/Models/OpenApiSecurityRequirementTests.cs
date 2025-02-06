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
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models.Interfaces;

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
                    new OpenApiSecuritySchemeReference("scheme1", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>
                {
                    "scope1",
                    "scope2",
                    "scope3",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme2", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>
                {
                    "scope4",
                    "scope5",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme3", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>()
            };
        public static OpenApiDocument SecurityRequirementWithReferencedSecurityScheme_supportingDocument
        {
            get
            {
                var document = new OpenApiDocument()
                {
                    Components = new()
                    {
                        SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                        {
                            ["scheme1"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.ApiKey,
                                Name = "apiKeyName1",
                                In = ParameterLocation.Header,
                            },
                            ["scheme2"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.OpenIdConnect,
                                OpenIdConnectUrl = new("http://example.com"),
                            },
                            ["scheme3"] = new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                            },
                        }
                    }
                };
                document.RegisterComponents();
                return document;
            }
        }

        public static OpenApiSecurityRequirement SecurityRequirementWithUnreferencedSecurityScheme =
            new()
            {
                [
                    new OpenApiSecuritySchemeReference("scheme1", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>
                {
                    "scope1",
                    "scope2",
                    "scope3",
                },
                [
                    new OpenApiSecuritySchemeReference("brokenUnreferencedScheme", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>
                {
                    "scope4",
                    "scope5",
                },
                [
                    new OpenApiSecuritySchemeReference("scheme3", SecurityRequirementWithReferencedSecurityScheme_supportingDocument)
                ] = new List<string>()
            };

        [Fact]
        public async Task SerializeBasicSecurityRequirementAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = await BasicSecurityRequirement.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
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
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeSecurityRequirementWithReferencedSecuritySchemeAsV3JsonWorks()
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
            var actual = await SecurityRequirementWithReferencedSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeSecurityRequirementWithReferencedSecuritySchemeAsV2JsonWorks()
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
            var actual = await SecurityRequirementWithReferencedSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeSecurityRequirementWithUnreferencedSecuritySchemeAsV3JsonShouldSkipUnserializableKeyValuePair()
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
            var actual = await SecurityRequirementWithUnreferencedSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeSecurityRequirementWithUnreferencedSecuritySchemeAsV2JsonShouldSkipUnserializableKeyValuePair()
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
                await SecurityRequirementWithUnreferencedSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
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
            };

            var securityScheme2 = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new("http://example.com"),
            };

            var securityScheme1Duplicate = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "apiKeyName1",
                In = ParameterLocation.Header,
            };

            var securityScheme1WithDifferentProperties = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "apiKeyName2",
                In = ParameterLocation.Query,
            };

            // Act
            securityRequirement.Add(new OpenApiSecuritySchemeReference("securityScheme1"), new List<string>());
            securityRequirement.Add(new OpenApiSecuritySchemeReference("securityScheme2"), new List<string> { "scope1", "scope2" });

            var addSecurityScheme1Duplicate = () =>
                securityRequirement.Add(new OpenApiSecuritySchemeReference("securityScheme1"), new List<string>());
            var addSecurityScheme1WithDifferentProperties = () =>
                securityRequirement.Add(new OpenApiSecuritySchemeReference("securityScheme1"), new List<string>());

            // Assert
            // Only the first two should be added successfully since the latter two are duplicates of securityScheme1.
            // Duplicate determination only considers Reference.Id.
            Assert.Throws<ArgumentException>(addSecurityScheme1Duplicate);
            Assert.Throws<ArgumentException>(addSecurityScheme1WithDifferentProperties);

            Assert.Equal(2, securityRequirement.Count);

            securityRequirement.Should().BeEquivalentTo(
                new OpenApiSecurityRequirement
                {
                    // This should work with any security scheme object
                    // as long as Reference.Id os securityScheme1
                    [new OpenApiSecuritySchemeReference("securityScheme1", null)] = new List<string>(),
                    [new OpenApiSecuritySchemeReference("securityScheme2", null)] = new List<string> { "scope1", "scope2" },
                });
        }
    }
}
