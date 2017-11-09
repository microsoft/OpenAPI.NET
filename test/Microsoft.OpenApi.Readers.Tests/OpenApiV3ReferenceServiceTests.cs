// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OpenApiV3ReferenceServiceTests
    {
        private OpenApiV3ReferenceService _referenceService;

        public OpenApiV3ReferenceServiceTests()
        {
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var yamlDocument = new YamlDocument("{}");
            RootNode rootNode = new RootNode(context, diagnostic, yamlDocument);
            _referenceService = new OpenApiV3ReferenceService(rootNode);
        }

        [Fact]
        public void ParseExternalHeaderReference()
        {
            // Arrange
            string input = "externalschema.json#/components/headers/blah";

            // Act
            var reference = _referenceService.FromString(input);

            // Assert
            Assert.Equal("externalschema.json", reference.ExternalResource);
            Assert.Equal(ReferenceType.Unknown, reference.ReferenceType);
            Assert.Equal("components/headers/blah", reference.LocalPointer);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/components/parameters/foobar");

            // Assert.
            Assert.Equal(ReferenceType.Parameter, reference.ReferenceType);
            Assert.Null(reference.ExternalResource);
            Assert.Equal("foobar", reference.LocalPointer);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/components/schemas/foobar");

            // Assert.
            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal("foobar", reference.LocalPointer);
            Assert.Null(reference.ExternalResource);
        }
    }
}
