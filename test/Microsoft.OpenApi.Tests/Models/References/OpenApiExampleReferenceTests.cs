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
    [UsesVerify]
    public class OpenApiExampleReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
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

        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
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
";

        private readonly OpenApiExampleReference _localExampleReference;
        private readonly OpenApiExampleReference _externalExampleReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiExampleReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, "yaml").OpenApiDocument;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, "yaml").OpenApiDocument;
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/examplereference", _openApiDoc);

            _localExampleReference = new OpenApiExampleReference("UserExample", _openApiDoc)
            {
                Summary = "Example of a local user",
                Description = "This is an example of a local user"
            };

            _externalExampleReference = new OpenApiExampleReference("UserExample", _openApiDoc_2, "http://localhost/examplereference")
            {
                Summary = "Example of an external user",
                Description = "This is an example of an external user"
            };
        }

        [Fact]
        public void ExampleReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Example of a local user", _localExampleReference.Summary);
            Assert.Equal("This is an example of a local user", _localExampleReference.Description);
            Assert.NotNull(_localExampleReference.Value);

            Assert.Equal("Example of an external user", _externalExampleReference.Summary);
            Assert.Equal("This is an example of an external user", _externalExampleReference.Description);
            Assert.NotNull(_externalExampleReference.Value);

            // The main description and summary values shouldn't change
            Assert.Equal("Example of a user", _openApiDoc.Components.Examples.First().Value.Summary);
            Assert.Equal("This is is an example of a user",
                _openApiDoc.Components.Examples.First().Value.Description);         
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeExampleReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localExampleReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }    
}
