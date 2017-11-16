﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiSecurityRequirementTests
    {
        public static OpenApiSecurityRequirement BasicSecurityRequirement = new OpenApiSecurityRequirement();

        public static OpenApiSecurityRequirement SecurityRequirementWithReferencedSecurityScheme =
            new OpenApiSecurityRequirement
            {
                Schemes = new OpenApiSecuritySchemeDictionary
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme1"}
                        }
                    ] = new List<string>
                    {
                        "scope1",
                        "scope2",
                        "scope3",
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme2"}
                        }
                    ] = new List<string>
                    {
                        "scope4",
                        "scope5",
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme3"}
                        }
                    ] = new List<string>()
                }
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

        [Fact]
        public void SerializeSecurityRequirementWithReferencedSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""scheme1"": [
    ""scope1"",
    ""scope2"",
    ""scope3""
  ],
  ""scheme2"": [
    ""scope4"",
    ""scope5""
  ],
  ""scheme3"": [ ]
}";

            // Act
            var actual = SecurityRequirementWithReferencedSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

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
                Reference = new OpenApiReference
                {
                    Id = "securityScheme1",
                    Type = ReferenceType.SecurityScheme
                }
            };

            var securityScheme2 = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri("http://example.com"),
                Reference = new OpenApiReference
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
                Reference = new OpenApiReference
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
                Reference = new OpenApiReference
                {
                    Id = "securityScheme1",
                    Type = ReferenceType.SecurityScheme
                }
            };

            // Act
            securityRequirement.AddSecurityScheme(securityScheme1, new List<string>());
            securityRequirement.AddSecurityScheme(securityScheme2, new List<string> {"scope1", "scope2"});

            Action addSecurityScheme1Duplicate = () =>
                securityRequirement.AddSecurityScheme(securityScheme1Duplicate, new List<string>());
            Action addSecurityScheme1WithDifferentProperties = () =>
                securityRequirement.AddSecurityScheme(securityScheme1WithDifferentProperties, new List<string>());

            // Assert
            // Only the first two should be added successfully since the latter two are duplicates of securityScheme1.
            // Duplicate determination only considers Reference.Id.
            addSecurityScheme1Duplicate.ShouldThrow<ArgumentException>();
            addSecurityScheme1WithDifferentProperties.ShouldThrow<ArgumentException>();

            securityRequirement.Schemes.Should().NotBeNull();
            securityRequirement.Schemes.Should().HaveCount(2);

            securityRequirement.ShouldBeEquivalentTo(
                new OpenApiSecurityRequirement
                {
                    Schemes = new OpenApiSecuritySchemeDictionary
                    {
                        // This should work with any security scheme object
                        // as long as Reference.Id os securityScheme1
                        [securityScheme1WithDifferentProperties] = new List<string>(),
                        [securityScheme2] = new List<string> {"scope1", "scope2"},
                    }
                });
        }
    }
}