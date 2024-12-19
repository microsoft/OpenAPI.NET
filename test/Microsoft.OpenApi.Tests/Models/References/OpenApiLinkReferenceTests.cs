// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Services;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiLinkReferenceTests
    {
        // OpenApi doc with external $ref
        private const string OpenApi = @"
openapi: 3.0.0
info:
  version: 0.0.0
  title: Links example
servers: 
  - url: https://myserver.com/v1.0
paths:
  /users:
    post:
      summary: Creates a user and returns the user ID
      operationId: createUser
      requestBody:
        required: true
        description: A JSON object that contains the user name and age.
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/User'
      responses:
        '201':
          description: Created
          content:
            application/json:
              schema:
                type: object
                properties:
                  id:
                    type: integer
                    format: int64
                    description: ID of the created user.
          links:
            GetUserByUserId:
              $ref: 'https://myserver.com/beta#/components/links/GetUserByUserId'   # <---- referencing the link here (externally)
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
  version: 0.0.0
  title: Links example
servers: 
  - url: https://myserver.com/beta
paths:
  /users:
    post:
      summary: Creates a user and returns the user ID
      operationId: createUser
      requestBody:
        required: true
        description: A JSON object that contains the user name and age.
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/User'
      responses:
        '201':
          description: Created
          content:
            application/json:
              schema:
                type: object
                properties:
                  id:
                    type: integer
                    format: int64
                    description: ID of the created user.
          links:
            GetUserByUserId:
              $ref: '#/components/links/GetUserByUserId'   # <---- referencing the link here
components:
  links:
    GetUserByUserId:
      operationId: getUser
      parameters:
        userId: '$response.body#/id'
      description: The id value returned in the response can be used as the userId parameter in GET /users/{userId}
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
";

        private readonly OpenApiLinkReference _localLinkReference;
        private readonly OpenApiLinkReference _externalLinkReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiLinkReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);

            _localLinkReference = new("GetUserByUserId", _openApiDoc_2)
            {
                Description = "Use the id returned as the userId in `GET /users/{userId}`"
            };

            _externalLinkReference = new("GetUserByUserId", _openApiDoc, "https://myserver.com/beta")
            {
                Description = "Externally referenced: Use the id returned as the userId in `GET /users/{userId}`"
            };
        }

        [Fact]
        public void LinkReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("getUser", _localLinkReference.OperationId);
            Assert.Equal("userId", _localLinkReference.Parameters.First().Key);
            Assert.Equal("Use the id returned as the userId in `GET /users/{userId}`", _localLinkReference.Description);
            
            Assert.Equal("getUser", _externalLinkReference.OperationId);
            Assert.Equal("userId", _localLinkReference.Parameters.First().Key);
            Assert.Equal("Externally referenced: Use the id returned as the userId in `GET /users/{userId}`", _externalLinkReference.Description);

            // The main description and summary values shouldn't change
            Assert.Equal("The id value returned in the response can be used as the userId parameter in GET /users/{userId}",
                _openApiDoc_2.Components.Links.FirstOrDefault().Value.Description); // The main description value shouldn't change
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeLinkReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localLinkReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeLinkReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localLinkReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
