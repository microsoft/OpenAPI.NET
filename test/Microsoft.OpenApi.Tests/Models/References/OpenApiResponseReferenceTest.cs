// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiResponseReferenceTest
    {
        private const string OpenApi = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

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
";

        private const string OpenApi_2 = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /ping:
    get:
      responses:
        '200':
          $ref: '#/components/responses/OkResponse'
";

        private readonly OpenApiResponseReference _localResponseReference;
        private readonly OpenApiResponseReference _externalResponseReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiResponseReferenceTest()
        {
            var reader = new OpenApiStringReader();
            _openApiDoc = reader.Read(OpenApi, out _);
            _openApiDoc_2 = reader.Read(OpenApi_2, out _);
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/responsereference", _openApiDoc);

            _localResponseReference = new("OkResponse", _openApiDoc)
            {
                Description = "OK response"
            };

            _externalResponseReference = new("OkResponse", _openApiDoc_2, "http://localhost/responsereference")
            {
                Description = "External reference: OK response"
            };
        }

        [Fact]
        public void ResponseReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("OK response", _localResponseReference.Description);
            Assert.Equal("text/plain", _localResponseReference.Content.First().Key);
            Assert.Equal("External reference: OK response", _externalResponseReference.Description);
            Assert.Equal("OK", _openApiDoc.Components.Responses.First().Value.Description);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeResponseReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localResponseReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
