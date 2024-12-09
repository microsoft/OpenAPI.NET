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
    public class OpenApiExampleReferenceTests
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
    get:
      summary: Get users
      responses:
        '200':
          description: Successful operation
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: 'https://myserver.com/beta#/components/schemas/User'
              examples:
                - $ref: 'https://myserver.com/beta#/components/examples/UserExample'
components:
  callbacks:
    callbackEvent:
      '{$request.body#/callbackUrl}':
        post:
          requestBody: # Contents of the callback message
            required: true
            content:
              application/json:
                schema:
                  type: object
                  properties:
                    message:
                      type: string
                      example: Some event happened
                  required:
                    - message
          responses:
            '200':
              description: ok"";
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
    get:
      summary: Get users
      responses:
        '200':
          description: Successful operation
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/User'
              examples:
                - $ref: '#/components/examples/UserExample'
components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
  examples:
    UserExample:
       summary: Example of a user
       description: This is is an example of a user
       value:
        - id: 1
          name: John Doe
";

        private readonly OpenApiExampleReference _localExampleReference;
        private readonly OpenApiExampleReference _externalExampleReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiExampleReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);

            _localExampleReference = new OpenApiExampleReference("UserExample", _openApiDoc_2)
            {
                Summary = "Example of a local user",
                Description = "This is an example of a local user"
            };

            _externalExampleReference = new OpenApiExampleReference("UserExample", _openApiDoc, "https://myserver.com/beta")
            {
                Summary = "Example of an external user",
                Description = "This is an example of an external user"
            };
        }

        [Fact]
        public void ExampleReferenceResolutionWorks()
         {
            // Assert
            Assert.NotNull(_localExampleReference.Value);
            Assert.Equal("[{\"id\":1,\"name\":\"John Doe\"}]", _localExampleReference.Value.ToJsonString());
            Assert.Equal("Example of a local user", _localExampleReference.Summary);
            Assert.Equal("This is an example of a local user", _localExampleReference.Description);

            Assert.NotNull(_externalExampleReference.Value);
            Assert.Equal("Example of an external user", _externalExampleReference.Summary);
            Assert.Equal("This is an example of an external user", _externalExampleReference.Description);            

            // The main description and summary values shouldn't change
            Assert.Equal("Example of a user", _openApiDoc_2.Components.Examples.First().Value.Summary);
            Assert.Equal("This is is an example of a user",
                _openApiDoc_2.Components.Examples.FirstOrDefault().Value.Description);         
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeExampleReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localExampleReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeExampleReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localExampleReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }    
}
