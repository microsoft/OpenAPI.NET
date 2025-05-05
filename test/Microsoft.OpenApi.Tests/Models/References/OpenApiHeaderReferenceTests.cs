﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;
using System.Collections.Generic;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiHeaderReferenceTests
    {
        // OpenApi doc with external $ref
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
paths:
  /users:
    post:
      summary: Create a post
      responses:
        '201':
          description: Post created successfully
          headers:
            Location:
              $ref: 'https://myserver.com/beta##/components/headers/LocationHeader'
components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
";

        // OpenApi doc with local $ref
        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/beta
paths:
  /users:
    post:
      summary: Create a post
      responses:
        '201':
          description: Post created successfully
          headers:
            Location:
              $ref: '#/components/headers/LocationHeader'
components:
  headers:
    LocationHeader:
      description: The URL of the newly created post
      schema:
        type: string
";

        private readonly OpenApiHeaderReference _localHeaderReference;
        private readonly OpenApiHeaderReference _externalHeaderReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiHeaderReferenceTests()
        {
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);

            _localHeaderReference = new OpenApiHeaderReference("LocationHeader", _openApiDoc_2)
            {
                Description = "Location of the locally referenced post"
            };

            _externalHeaderReference = new OpenApiHeaderReference("LocationHeader", _openApiDoc, "https://myserver.com/beta")
            {
                Description = "Location of the externally referenced post"
            };
        }

        [Fact]
        public void HeaderReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal(JsonSchemaType.String, _externalHeaderReference.Schema.Type);
            Assert.Equal("Location of the locally referenced post", _localHeaderReference.Description);
            Assert.Equal("Location of the externally referenced post", _externalHeaderReference.Description);
            Assert.Equal("The URL of the newly created post",
                _openApiDoc_2.Components.Headers.First().Value.Description); // The main description value shouldn't change
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task SerializeHeaderReferenceAsV3JsonWorks(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _localHeaderReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task SerializeHeaderReferenceAsV31JsonWorks(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _localHeaderReference.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task SerializeHeaderReferenceAsV2JsonWorksAsync(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _localHeaderReference.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }

        [Fact]
        public void OpenApiHeaderTargetShouldResolveReference()
        {
            var doc = new OpenApiDocument
            {
                Components = new OpenApiComponents
                {
                    Headers = new OrderedDictionary<string, IOpenApiHeader>
                    {
                        { "header1", new OpenApiHeader
                            {
                                Description = "test header",
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String
                                }
                            }
                        }
                    }
                }
            };

            doc.Workspace.RegisterComponents(doc);

            var headerReference = new OpenApiHeaderReference("header1", doc);
            Assert.Equal("test header", headerReference.Description);
            Assert.Equal(JsonSchemaType.String, headerReference.Schema.Type);
        }
    }
}
