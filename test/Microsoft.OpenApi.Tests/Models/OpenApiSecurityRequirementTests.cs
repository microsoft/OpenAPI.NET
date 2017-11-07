// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiSecurityRequirementTests
    {
        public static OpenApiSecurityRequirement BasicSecurityRequirement = new OpenApiSecurityRequirement();

        public static OpenApiSecurityRequirement AdvancedSecurityRequirement = new OpenApiSecurityRequirement
        {
            Schemes = new Dictionary<OpenApiSecurityScheme, List<string>>
            {
                [new OpenApiSecurityScheme
                {
                    Pointer = new OpenApiReference
                    {
                        TypeName = "scheme1",
                        ReferenceType = ReferenceType.SecurityScheme
                    }
                }] = new List<string>
                {
                    "scope1",
                    "scope2",
                    "scope3",
                },
                [new OpenApiSecurityScheme
                {
                    Pointer = new OpenApiReference
                    {
                        TypeName = "scheme2",
                        ReferenceType = ReferenceType.SecurityScheme
                    }
                }] = new List<string>
                {
                    "scope4",
                    "scope5",
                },
                [new OpenApiSecurityScheme
                {
                    Pointer = new OpenApiReference
                    {
                        TypeName = "scheme3",
                        ReferenceType = ReferenceType.SecurityScheme
                    }
                }] = new List<string>()
            }
        };

        [Fact]
        public void SerializeBasicSecurityRequirementAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSecurityRequirement.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSecurityRequirementAsV3JsonWorks()
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
            var actual = AdvancedSecurityRequirement.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}