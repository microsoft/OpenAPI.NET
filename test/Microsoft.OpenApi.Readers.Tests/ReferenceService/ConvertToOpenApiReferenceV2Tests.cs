// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class ConvertToOpenApiReferenceV2Tests
    {
        public OpenApiDiagnostic Diagnostic { get; }

        public ConvertToOpenApiReferenceV2Tests()
        {
            Diagnostic = new();
        }

        [Fact]
        public void ParseExternalReferenceToV2OpenApi()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var externalResource = "externalSchema.json";
            var id = "mySchema";
            var input = $"{externalResource}#/definitions/{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, null);

            // Assert
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.NotNull(reference.Type);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseExternalReference()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var externalResource = "externalSchema.json";
            var id = "/externalPathSegment1/externalPathSegment2/externalPathSegment3";
            var input = $"{externalResource}#{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, null);

            // Assert
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.Null(reference.Type);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var referenceType = ReferenceType.Parameter;
            var id = "parameterId";
            var input = $"#/parameters/{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            Assert.Equal(referenceType, reference.Type);
            Assert.Null(reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var referenceType = ReferenceType.Schema;
            var id = "parameterId";
            var input = $"#/definitions/{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            Assert.Equal(referenceType, reference.Type);
            Assert.Null(reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseTagReference()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var referenceType = ReferenceType.Tag;
            var id = "tagId";
            var input = $"{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            Assert.Equal(referenceType, reference.Type);
            Assert.Null(reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseSecuritySchemeReference()
        {
            // Arrange
            var versionService = new OpenApiV2VersionService(Diagnostic);
            var referenceType = ReferenceType.SecurityScheme;
            var id = "securitySchemeId";
            var input = $"{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            Assert.Equal(referenceType, reference.Type);
            Assert.Null(reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }
    }
}
