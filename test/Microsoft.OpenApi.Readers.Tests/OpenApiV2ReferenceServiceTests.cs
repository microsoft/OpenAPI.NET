// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using SharpYaml.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class OpenApiV2ReferenceServiceTests
    {
        private OpenApiV2ReferenceService _referenceService;
        public OpenApiV2ReferenceServiceTests()
        {
            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var yamlDocument = new YamlDocument("{}");
            RootNode rootNode = new RootNode(context, diagnostic, yamlDocument);
            _referenceService = new OpenApiV2ReferenceService(rootNode);
        }

        [Fact]
        public void ParseExternalHeaderReference()
        {
            // Arrange
            string input = "swagger.json#/definitions/parameters/blahblah";

            // Act
            var reference = _referenceService.FromString(input);

            // Assert
            Assert.Equal("swagger.json", reference.ExternalResource);
            Assert.Equal(ReferenceType.Unknown, reference.ReferenceType);
            Assert.Equal("definitions/parameters/blahblah", reference.LocalPointer);
        }

        [Fact]
        public void ParseLocalParameterReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/parameters/foobar");

            // Assert
            Assert.Equal(ReferenceType.Parameter, reference.ReferenceType);
            Assert.Null(reference.ExternalResource);
            Assert.Equal("foobar", reference.LocalPointer);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            // Arrange & Act
            var reference = _referenceService.FromString("#/definitions/foobar");

            // Assert
            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal("foobar", reference.LocalPointer);
            Assert.Null(reference.ExternalResource);
        }
    }
}
