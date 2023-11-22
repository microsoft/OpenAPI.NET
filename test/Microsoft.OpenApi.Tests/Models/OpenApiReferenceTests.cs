// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiReferenceTests
    {
        [Theory]
        [InlineData("#/components/schemas/Pet", ReferenceType.Schema, "Pet")]
        [InlineData("#/components/parameters/name", ReferenceType.Parameter, "name")]
        [InlineData("#/components/responses/200", ReferenceType.Response, "200")]
        public void SettingInternalReferenceForComponentsStyleReferenceShouldSucceed(
            string input,
            ReferenceType type,
            string id)
        {
            // Arrange & Act
            var reference = new OpenApiReference
            {
                Type = type,
                Id = id
            };

            // Assert
            reference.ExternalResource.Should().BeNull();
            reference.Type.Should().Be(type);
            reference.Id.Should().Be(id);

            reference.ReferenceV3.Should().Be(input);
            reference.ReferenceV2.Should().Be(input.Replace("schemas", "definitions").Replace("/components", ""));
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", null, null)]
        [InlineData("Pet.yaml", "Pet.yaml", null, null)]
        [InlineData("abc", "abc", null, null)]
        [InlineData("Pet.json#/components/schemas/Pet", "Pet.json", "Pet", ReferenceType.Schema)]
        [InlineData("Pet.yaml#/components/schemas/Pet", "Pet.yaml", "Pet", ReferenceType.Schema)]
        [InlineData("abc#/components/schemas/Pet", "abc", "Pet", ReferenceType.Schema)]
        public void SettingExternalReferenceV3ShouldSucceed(string expected, string externalResource, string id, ReferenceType? type)
        {
            // Arrange & Act
            var reference = new OpenApiReference
            {
                ExternalResource = externalResource,
                Type = type,
                Id = id
            };

            // Assert
            reference.ExternalResource.Should().Be(externalResource);
            reference.Id.Should().Be(id);

            reference.ReferenceV3.Should().Be(expected);
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", null, null)]
        [InlineData("Pet.yaml", "Pet.yaml", null, null)]
        [InlineData("abc", "abc", null, null)]
        [InlineData("Pet.json#/definitions/Pet", "Pet.json", "Pet", ReferenceType.Schema)]
        [InlineData("Pet.yaml#/definitions/Pet", "Pet.yaml", "Pet", ReferenceType.Schema)]
        [InlineData("abc#/definitions/Pet", "abc", "Pet", ReferenceType.Schema)]
        public void SettingExternalReferenceV2ShouldSucceed(string expected, string externalResource, string id, ReferenceType? type)
        {
            // Arrange & Act
            var reference = new OpenApiReference
            {
                ExternalResource = externalResource,
                Type = type,
                Id = id
            };

            // Assert
            reference.ExternalResource.Should().Be(externalResource);
            reference.Id.Should().Be(id);

            reference.ReferenceV2.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsJsonV3Works()
        {
            // Arrange
            var reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Pet" };
            var expected =
                """
                {
                  "$ref": "#/components/schemas/Pet"
                }
                """;

            // Act
            var actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "Pet"
            };

            var expected = @"$ref: '#/components/schemas/Pet'";

            // Act
            var actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsJsonV2Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "Pet"
            };

            var expected =
                """
                {
                  "$ref": "#/definitions/Pet"
                }
                """.MakeLineBreaksEnvironmentNeutral();

            // Act
            var actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.MakeLineBreaksEnvironmentNeutral().Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsYamlV2Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "Pet"
            };
            var expected = @"$ref: '#/definitions/Pet'";

            // Act
            var actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeExternalReferenceAsJsonV2Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                ExternalResource = "main.json",
                Type = ReferenceType.Schema,
                Id = "Pets"
            };

            var expected =
                """
                {
                  "$ref": "main.json#/definitions/Pets"
                }
                """;

            // Act
            var actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeExternalReferenceAsYamlV2Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                ExternalResource = "main.json",
                Type = ReferenceType.Schema,
                Id = "Pets"
            };
            var expected = @"$ref: main.json#/definitions/Pets";

            // Act
            var actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeExternalReferenceAsJsonV3Works()
        {
            // Arrange
            var reference = new OpenApiReference { ExternalResource = "main.json", Type = ReferenceType.Schema, Id = "Pets" };

            var expected =
                """
                {
                  "$ref": "main.json#/components/schemas/Pets"
                }
                """;

            // Act
            var actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeExternalReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference { ExternalResource = "main.json", Type = ReferenceType.Schema, Id = "Pets" };
            var expected = @"$ref: main.json#/components/schemas/Pets";

            // Act
            var actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
