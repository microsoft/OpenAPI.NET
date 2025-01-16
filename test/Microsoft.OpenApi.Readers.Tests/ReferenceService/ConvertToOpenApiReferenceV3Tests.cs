// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class ConvertToOpenApiReferenceV3Tests
    {
        public OpenApiDiagnostic Diagnostic { get; }

        public ConvertToOpenApiReferenceV3Tests()
        {
            Diagnostic = new();
        }

        [Fact]
        public void ParseExternalReference()
        {
            // Arrange
            var versionService = new OpenApiV3VersionService(Diagnostic);
            var externalResource = "externalSchema.json";
            var id = "/externalPathSegment1/externalPathSegment2/externalPathSegment3";
            var input = $"{externalResource}#{id}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, null);

            // Assert
            Assert.Null(reference.Type);
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange
            var versionService = new OpenApiV3VersionService(Diagnostic);
            var referenceType = ReferenceType.Parameter;
            var id = "parameterId";
            var input = $"#/components/parameters/{id}";

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
            var versionService = new OpenApiV3VersionService(Diagnostic);
            var referenceType = ReferenceType.Schema;
            var id = "schemaId";
            var input = $"#/components/schemas/{id}";

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
            var versionService = new OpenApiV3VersionService(Diagnostic);
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
            var versionService = new OpenApiV3VersionService(Diagnostic);
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

        [Fact]
        public void ParseLocalFileReference()
        {
            // Arrange
            var versionService = new OpenApiV3VersionService(Diagnostic);
            var referenceType = ReferenceType.Schema;
            var input = $"../schemas/collection.json";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            Assert.Equal(referenceType, reference.Type);
            Assert.Equal(input, reference.ExternalResource);
        }

        [Fact]
        public void ParseExternalPathReference()
        {
            // Arrange
            var versionService = new OpenApiV3VersionService(Diagnostic);
            var externalResource = "externalSchema.json";
            var referenceJsonEscaped = "/paths/~1applications~1{AppUUID}~1services~1{ServiceName}";
            var input = $"{externalResource}#{referenceJsonEscaped}";
            var id = "/applications/{AppUUID}/services/{ServiceName}";

            // Act
            var reference = versionService.ConvertToOpenApiReference(input, null);

            // Assert
            Assert.Null(reference.Type);
            Assert.Equal(externalResource, reference.ExternalResource);
            Assert.Equal(id, reference.Id);
        }
    }
}
