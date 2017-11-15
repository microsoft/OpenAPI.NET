// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.ReferenceServices;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ReferenceService
{
    public class ConvertToOpenApiReferenceV3Tests
    {
        private readonly OpenApiV3ReferenceService _referenceService;

        public ConvertToOpenApiReferenceV3Tests()
        {
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var yamlDocument = new YamlDocument("{}");
            var rootNode = new RootNode(context, diagnostic, yamlDocument);
            _referenceService = new OpenApiV3ReferenceService(rootNode);
        }

        [Fact]
        public void ParseExternalReference()
        {
            // Arrange
            var externalResource = "externalSchema.json";
            var id = "externalPathSegment1/externalPathSegment2/externalPathSegment3";
            var input = $"{externalResource}#/{id}";

            // Act
            var reference = _referenceService.ConvertToOpenApiReference(input, null);

            // Assert
            reference.Type.Should().BeNull();
            reference.ExternalResource.Should().Be(externalResource);
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange
            var referenceType = ReferenceType.Parameter;
            var id = "parameterId";
            var input = $"#/components/parameters/{id}";

            // Act
            var reference = _referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange
            var referenceType = ReferenceType.Schema;
            var id = "schemaId";
            var input = $"#/components/schemas/{id}";

            // Act
            var reference = _referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseTagReference()
        {
            // Arrange
            var referenceType = ReferenceType.Tag;
            var id = "tagId";
            var input = $"{id}";

            // Act
            var reference = _referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }

        [Fact]
        public void ParseSecuritySchemeReference()
        {
            // Arrange
            var referenceType = ReferenceType.SecurityScheme;
            var id = "securitySchemeId";
            var input = $"{id}";

            // Act
            var reference = _referenceService.ConvertToOpenApiReference(input, referenceType);

            // Assert
            reference.Type.Should().Be(referenceType);
            reference.ExternalResource.Should().BeNull();
            reference.Id.Should().Be(id);
        }
    }
}