﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiPathItemReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.1.1
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
paths:
  /users:
    $ref: 'https://myserver.com/beta#/components/pathItems/userPathItem'
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

        private const string OpenApi_2 = @"
openapi: 3.1.1
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/beta
paths:
  /users:
    $ref: '#/components/pathItems/userPathItem'
components:
  pathItems:
    userPathItem:
      description: User path item description
      summary: User path item summary
      get:
        summary: Get users
        responses:
          200:
            description: Successful operation
      post:
        summary: Create a user
        responses:
          201:
            description: User created successfully
      delete:
        summary: Delete a user
        responses:
          204:
            description: User deleted successfully
";

        private readonly OpenApiPathItemReference _localPathItemReference;
        private readonly OpenApiPathItemReference _externalPathItemReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiPathItemReferenceTests()
        {
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);
            _openApiDoc_2.Workspace.RegisterComponents(_openApiDoc_2);

            _localPathItemReference = new OpenApiPathItemReference("userPathItem", _openApiDoc_2)
            {
                Description = "Local reference: User path item description",
                Summary = "Local reference: User path item summary"
            };

            _externalPathItemReference = new OpenApiPathItemReference("userPathItem", _openApiDoc, "https://myserver.com/beta")
            {
                Description = "External reference: User path item description",
                Summary = "External reference: User path item summary"
            };
        }

        [Fact]
        public void PathItemReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal([HttpMethod.Get, HttpMethod.Post, HttpMethod.Delete],
                _localPathItemReference.Operations.Select(o => o.Key));
            Assert.Equal(3, _localPathItemReference.Operations.Count);
            Assert.Equal("Local reference: User path item description", _localPathItemReference.Description);
            Assert.Equal("Local reference: User path item summary", _localPathItemReference.Summary);

            Assert.Equal([HttpMethod.Get, HttpMethod.Post, HttpMethod.Delete],
                _externalPathItemReference.Operations.Select(o => o.Key));
            Assert.Equal("External reference: User path item description", _externalPathItemReference.Description);
            Assert.Equal("External reference: User path item summary", _externalPathItemReference.Summary);

            // The main description and summary values shouldn't change
            Assert.Equal("User path item description", _openApiDoc_2.Components.PathItems.First().Value.Description);
            Assert.Equal("User path item summary", _openApiDoc_2.Components.PathItems.First().Value.Summary);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task SerializePathItemReferenceAsV31JsonWorks(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _localPathItemReference.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }
    }
}
