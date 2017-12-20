// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ReferenceService
{
    public class ConvertToOpenApiReferenceV2Tests
    {
       

        [Fact]
        public void ParseExternalReference()
        {
            // Arrange
            var referenceService = new OpenApiV2VersionService();
            var externalResource = "externalSchema.json";
            var id = "externalPathSegment1/externalPathSegment2/externalPathSegment3";
            var input = $"{externalResource}#/{id}";

            // Act
            var reference = referenceService.ConvertToOpenApiReference(input, null);

            // Assert
            reference.ExternalResource.Should().Be(externalResource);
            reference.Type.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange
            var referenceService = new OpenApiV2VersionService();
            var referenceType = ReferenceType.Parameter;
            var id = "parameterId";
            var input = $"#/parameters/{id}";

            // Act
            var reference = referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange
            var referenceService = new OpenApiV2VersionService();
            var referenceType = ReferenceType.Schema;
            var id = "parameterId";
            var input = $"#/definitions/{id}";

            // Act
            var reference = referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseTagReference()
        {
            // Arrange
            var referenceService = new OpenApiV2VersionService();
            var referenceType = ReferenceType.Tag;
            var id = "tagId";
            var input = $"{id}";

            // Act
            var reference = referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseSecuritySchemeReference()
        {
            // Arrange
            var referenceService = new OpenApiV2VersionService();
            var referenceType = ReferenceType.SecurityScheme;
            var id = "securitySchemeId";
            var input = $"{id}";

            // Act
            var reference = referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }
    }
}