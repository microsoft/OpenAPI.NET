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
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiLinkReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  version: 0.0.0
  title: Links example
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
      description: The id value returned in the response can be used as the userId parameter in GET /users/{userId}";

        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  version: 0.0.0
  title: Links example
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
";

        private readonly OpenApiLinkReference _localLinkReference;
        private readonly OpenApiLinkReference _externalLinkReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiLinkReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, "yaml").OpenApiDocument;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, "yaml").OpenApiDocument;
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/linkreferencesample", _openApiDoc);

            _localLinkReference = new("GetUserByUserId", _openApiDoc)
            {
                Description = "Use the id returned as the userId in `GET /users/{userId}`"
            };

            _externalLinkReference = new("GetUserByUserId", _openApiDoc_2, "http://localhost/linkreferencesample")
            {
                Description = "Externally referenced: Use the id returned as the userId in `GET /users/{userId}`"
            };
        }

        [Fact]
        public void LinkReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Use the id returned as the userId in `GET /users/{userId}`", _localLinkReference.Description);
            Assert.Equal("getUser", _localLinkReference.OperationId);
            Assert.Equal("userId", _localLinkReference.Parameters.First().Key);
            Assert.Equal("Externally referenced: Use the id returned as the userId in `GET /users/{userId}`", _externalLinkReference.Description);
            Assert.Equal("The id value returned in the response can be used as the userId parameter in GET /users/{userId}",
                _openApiDoc.Components.Links.First().Value.Description); // The main description value shouldn't change
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
