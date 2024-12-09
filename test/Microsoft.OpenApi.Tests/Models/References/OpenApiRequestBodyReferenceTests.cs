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
    public class OpenApiRequestBodyReferenceTests
    {
        private readonly string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
paths:
  /users:
    post:
      summary: Create a user
      requestBody:
        $ref: 'https://myserver.com/beta#/components/requestBodies/UserRequest'  # <---- externally referencing the requestBody here
      responses:
        '201':
          description: User created
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

        private readonly string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/beta
paths:
  /users:
    post:
      summary: Create a user
      requestBody:
        $ref: '#/components/requestBodies/UserRequest'  # <---- referencing the requestBody here
      responses:
        '201':
          description: User created
components:
  requestBodies:
    UserRequest:
      description: User creation request body
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/UserSchema'
  schemas:
    UserSchema:
      type: object
      properties:
        name:
          type: string
        email:
          type: string
";

        private readonly OpenApiRequestBodyReference _localRequestBodyReference;
        private readonly OpenApiRequestBodyReference _externalRequestBodyReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiRequestBodyReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);

            _localRequestBodyReference = new("UserRequest", _openApiDoc_2)
            {
                Description = "User request body"
            };

            _externalRequestBodyReference = new("UserRequest", _openApiDoc, "https://myserver.com/beta")
            {
                Description = "External Reference: User request body"
            };
        }

        [Fact]
        public void RequestBodyReferenceResolutionWorks()
        {
            // Assert
            var localContent = _localRequestBodyReference.Content.Values.FirstOrDefault();
            Assert.NotNull(localContent);
            Assert.Equal("UserSchema", localContent.Schema.Reference.Id);
            Assert.Equal("User request body", _localRequestBodyReference.Description);
            Assert.Equal("application/json", _localRequestBodyReference.Content.First().Key);

            var externalContent = _externalRequestBodyReference.Content.Values.FirstOrDefault();
            Assert.NotNull(externalContent);
            Assert.Equal("UserSchema", externalContent.Schema.Reference.Id);

            Assert.Equal("External Reference: User request body", _externalRequestBodyReference.Description);
            Assert.Equal("User creation request body", _openApiDoc_2.Components.RequestBodies.First().Value.Description);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeRequestBodyReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput});

            // Act
            _localRequestBodyReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeRequestBodyReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localRequestBodyReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
