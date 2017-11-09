// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiReferenceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ConstructorShouldThrowArgumentNullOrWhiteSpaceWithNullInput(string reference)
        {
            // Arrange & Act
            Action action = () => new OpenApiReference(reference);

            // Assert
            action.ShouldThrow<ArgumentException>(String.Format(SRResource.ArgumentNullOrWhiteSpace, "externalResource"));
        }

        [Theory]
        [InlineData("#/components/schemas/Pet", ReferenceType.Schema, "Pet")]
        [InlineData("#/components/parameters/name", ReferenceType.Parameter, "name")]
        [InlineData("#/components/responses/200", ReferenceType.Response, "200")]
        public void ConstructorSetLocalPropertyValue(string input, ReferenceType type, string pointer)
        {
            // Arrange & Act
            var reference = new OpenApiReference(type, pointer);

            // Assert
            reference.ExternalResource.Should().BeNull();
            reference.ReferenceType.Should().Be(type);
            reference.LocalPointer.Should().Be(pointer);
            reference.ToString().Should().Be(input);
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", null)]
        [InlineData("Pet.yaml", "Pet.yaml", null)]
        [InlineData("abc", "abc", null)]
        [InlineData("Pet.json#/Pet", "Pet.json", "Pet")]
        [InlineData("Pet.yaml#/Pet", "Pet.yaml", "Pet")]
        [InlineData("abc#/Pet", "abc","Pet")]
        public void ConstructorSetExernalPropertyValues(string input, string external, string pointer)
        {
            // Arrange & Act
            var reference = new OpenApiReference(external, pointer);

            // Assert
            reference.ExternalResource.Should().Be(external);
            reference.ReferenceType.Should().Be(ReferenceType.Unknown);
            reference.LocalPointer.Should().Be(pointer);
            reference.ToString().Should().Be(input);
        }

        [Fact]
        public void SerializeSchemaReferenceAsJsonV3Works()
        {
            // Arrange
            var reference = new OpenApiReference(ReferenceType.Schema, "Pet");
            string expected = @"{
  ""$ref"": ""#/components/schemas/Pet""
}";

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference(ReferenceType.Schema, "Pet");
            string expected = @"$ref: '#/components/schemas/Pet'";

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsJsonV2Works()
        {
            // Arrange
            var reference = new OpenApiReference(ReferenceType.Schema, "Pet");
            string expected = @"{
  ""$ref"": ""#/definitions/Pet""
}";

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaReferenceAsYamlV2Works()
        {
            // Arrange
            var reference = new OpenApiReference(ReferenceType.Schema, "Pet");
            string expected = @"$ref: '#/definitions/Pet'";

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeExternalReferenceAsJsonV2Works()
        {
            // Arrange
            var reference = new OpenApiReference("main.json", "Pets");
            string expect = @"{
  ""$ref"": ""main.json#/Pets""
}";

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expect);
        }

        [Fact]
        public void SerializeExternalReferenceAsYamlV2Works()
        {
            // Arrange
            var reference = new OpenApiReference("main.json", "Pets");
            string expect = @"$ref: main.json#/Pets";

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual.Should().Be(expect);
        }

        [Fact]
        public void SerializeExternalReferenceAsJsonV3Works()
        {
            // Arrange
            var reference = new OpenApiReference("main.json", "Pets");
            string expect = @"{
  ""$ref"": ""main.json#/Pets""
}";

            // Act
            string actual = reference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expect);
        }

        [Fact]
        public void SerializeExternalReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference("main.json", "Pets");
            string expect = @"$ref: main.json#/Pets";

            // Act
            string actual = reference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expect);
        }
    }
}
