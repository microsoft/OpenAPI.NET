// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
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
        [InlineData("#/components/schemas/HttpValidationsProblem", ReferenceType.Schema, "HttpValidationsProblem")]
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
            Assert.Null(reference.ExternalResource);
            Assert.Equal(type, reference.Type);
            Assert.Equal(id, reference.Id);

            Assert.Equal(input, reference.ReferenceV3);
            Assert.Equal(input.Replace("schemas", "definitions").Replace("/components", ""), reference.ReferenceV2);
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", null, ReferenceType.Schema)]
        [InlineData("Pet.yaml", "Pet.yaml", null, ReferenceType.Schema)]
        [InlineData("abc", "abc", null, ReferenceType.Schema)]
        [InlineData("Pet.json#/components/schemas/Pet", "Pet.json", "Pet", ReferenceType.Schema)]
        [InlineData("Pet.yaml#/components/schemas/Pet", "Pet.yaml", "Pet", ReferenceType.Schema)]
        [InlineData("abc#/components/schemas/Pet", "abc", "Pet", ReferenceType.Schema)]
        [InlineData("abc#/components/schemas/HttpsValidationProblem", "abc", "HttpsValidationProblem", ReferenceType.Schema)]
        public void SettingExternalReferenceV3ShouldSucceed(string expected, string externalResource, string id, ReferenceType type)
        {
            // Arrange & Act
            var reference = new OpenApiReference
            {
                ExternalResource = externalResource,
                Type = type,
                Id = id
            };

            // Assert
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.Equal(id, reference.Id);

            Assert.Equal(expected, reference.ReferenceV3);
        }

        [Theory]
        [InlineData("Pet.json", "Pet.json", null, ReferenceType.Schema)]
        [InlineData("Pet.yaml", "Pet.yaml", null, ReferenceType.Schema)]
        [InlineData("abc", "abc", null, ReferenceType.Schema)]
        [InlineData("Pet.json#/definitions/Pet", "Pet.json", "Pet", ReferenceType.Schema)]
        [InlineData("Pet.yaml#/definitions/Pet", "Pet.yaml", "Pet", ReferenceType.Schema)]
        [InlineData("abc#/definitions/Pet", "abc", "Pet", ReferenceType.Schema)]
        public void SettingExternalReferenceV2ShouldSucceed(string expected, string externalResource, string id, ReferenceType type)
        {
            // Arrange & Act
            var reference = new OpenApiReference
            {
                ExternalResource = externalResource,
                Type = type,
                Id = id
            };

            // Assert
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.Equal(id, reference.Id);

            Assert.Equal(expected, reference.ReferenceV2);
        }

        [Fact]
        public async Task SerializeSchemaReferenceAsJsonV3Works()
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
            var actual = await reference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("HttpValidationProblemDetails", "#/components/schemas/HttpValidationProblemDetails")]
        [InlineData("http://example.com", "http://example.com")]
        [InlineData("https://example.com", "https://example.com")]
        public async Task SerializeHttpSchemaReferenceAsJsonV31Works(string id, string referenceV3)
        {
            // Arrange
            var reference = new OpenApiReference { Type = ReferenceType.Schema, Id = id };
            var expected =
                $$"""
                {
                  "$ref": "{{referenceV3}}"
                }
                """;

            // Act
            var actual = await reference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeSchemaReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "Pet"
            };

            var expected = @"$ref: '#/components/schemas/Pet'";

            // Act
            var actual = await reference.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeSchemaReferenceAsJsonV2Works()
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
            var actual = await reference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.Equal(expected, actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeSchemaReferenceAsYamlV2Works()
        {
            // Arrange
            var reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "Pet"
            };
            var expected = @"$ref: '#/definitions/Pet'";

            // Act
            var actual = await reference.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeExternalReferenceAsJsonV2Works()
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
            var actual = await reference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeExternalReferenceAsYamlV2Works()
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
            var actual = await reference.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeExternalReferenceAsJsonV3Works()
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
            var actual = await reference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual = actual.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeExternalReferenceAsYamlV3Works()
        {
            // Arrange
            var reference = new OpenApiReference { ExternalResource = "main.json", Type = ReferenceType.Schema, Id = "Pets" };
            var expected = @"$ref: main.json#/components/schemas/Pets";

            // Act
            var actual = await reference.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
