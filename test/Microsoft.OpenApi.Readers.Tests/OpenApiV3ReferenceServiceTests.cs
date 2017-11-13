// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OpenApiV3ReferenceServiceTests
    {
        private readonly OpenApiV3ReferenceService _referenceService;

        public OpenApiV3ReferenceServiceTests()
        {
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var yamlDocument = new YamlDocument("{}");
            var rootNode = new RootNode(context, diagnostic, yamlDocument);
            _referenceService = new OpenApiV3ReferenceService(rootNode);
        }

        [Fact]
        public void ParseExternalHeaderReference()
        {
            // Arrange
            var input = "externalschema.json#/components/headers/headerIdentifier";

            // Act
            var reference = _referenceService.FromString(input);

            // Assert
            reference.ExternalResource.Should().Be("externalschema.json");
            reference.ReferenceType.Should().Be(ReferenceType.Unknown);
            reference.Name.Should().Be("components/headers/headerIdentifier");
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/components/parameters/parameterIdentifier");

            // Assert
            reference.ReferenceType.Should().Be(ReferenceType.Parameter);
            reference.ExternalResource.Should().BeNull();
            reference.Name.Should().Be("parameterIdentifier");
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/components/schemas/schemaIdentifier");

            // Assert
            reference.ReferenceType.Should().Be(ReferenceType.Schema);
            reference.Name.Should().Be("schemaIdentifier");
            reference.ExternalResource.Should().BeNull();
        }
    }
}