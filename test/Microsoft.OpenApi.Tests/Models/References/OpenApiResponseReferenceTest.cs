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
    public class OpenApiResponseReferenceTest
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
paths:
  /ping:
    get:
      responses:
        '200':
          $ref: 'https://myserver.com/beta#/components/responses/OkResponse'
";

        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/beta
paths:
  /ping:
    get:
      responses:
        '200':
          $ref: '#/components/responses/OkResponse'
components:
  responses:
    OkResponse:
      description: OK
      content:
        text/plain:
          schema:
            $ref: '#/components/schemas/Pong'
  schemas:
    Pong:
      type: object
      properties:
        sound:
          type: string
";

        private readonly OpenApiResponseReference _localResponseReference;
        private readonly OpenApiResponseReference _externalResponseReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiResponseReferenceTest()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml).Document;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml).Document;
            _openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", _openApiDoc_2.BaseUri);
            _openApiDoc.Workspace.RegisterComponents(_openApiDoc_2);

            _localResponseReference = new("OkResponse", _openApiDoc_2)
            {
                Description = "OK response"
            };

            _externalResponseReference = new("OkResponse", _openApiDoc, "https://myserver.com/beta")
            {
                Description = "External reference: OK response"
            };
        }

        [Fact]
        public void ResponseReferenceResolutionWorks()
        {
            // Assert
            var localContent = _localResponseReference.Content.FirstOrDefault();
            Assert.Equal("text/plain", localContent.Key);                     
            Assert.Equal("Pong", localContent.Value.Schema.Reference.Id);
            Assert.Equal("OK response", _localResponseReference.Description);

            var externalContent = _externalResponseReference.Content.FirstOrDefault();
            Assert.Equal("text/plain", externalContent.Key);
            Assert.Equal("Pong", externalContent.Value.Schema.Reference.Id);
            Assert.Equal("External reference: OK response", _externalResponseReference.Description);

            Assert.Equal("OK", _openApiDoc_2.Components.Responses.First().Value.Description);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeResponseReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput});

            // Act
            _localResponseReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeResponseReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput});

            // Act
            _localResponseReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
