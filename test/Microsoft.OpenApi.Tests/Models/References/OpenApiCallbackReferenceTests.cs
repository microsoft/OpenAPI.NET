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
    public class OpenApiCallbackReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Callback with ref Example
  version: 1.0.0
paths:
  /register:
    post:
      summary: Subscribe to a webhook
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                callbackUrl: # Callback URL
                  type: string
                  format: uri
                  example: https://myserver.com/send/callback/here
              required:
                - callbackUrl
      responses:
        '200':
          description: subscription successfully created
          content:
            application/json:
              schema:
                type: object
                description: subscription information
                required:
                  - subscriptionId
                properties:
                  subscriptionId:
                    description: unique identifier
                    type: string
                    example: 2531329f-fb09-4ef7-887e-84e648214436
      callbacks:
        myEvent:
          $ref: '#/components/callbacks/callbackEvent'
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
              description: ok";

        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Callback with ref Example
  version: 1.0.0
paths:
  /register:
    post:
      summary: Subscribe to a webhook
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                callbackUrl: # Callback URL
                  type: string
                  format: uri
                  example: https://myserver.com/send/callback/here
              required:
                - callbackUrl
      responses:
        '200':
          description: subscription successfully created
          content:
            application/json:
              schema:
                type: object
                description: subscription information
                required:
                  - subscriptionId
                properties:
                  subscriptionId:
                    description: unique identifier
                    type: string
                    example: 2531329f-fb09-4ef7-887e-84e648214436
      callbacks:
        myEvent:
          $ref: '#/components/callbacks/callbackEvent'
";

        private readonly OpenApiCallbackReference _localCallbackReference;
        private readonly OpenApiCallbackReference _externalCallbackReference;

        public OpenApiCallbackReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            OpenApiDocument openApiDoc = OpenApiDocument.Parse(OpenApi, "yaml").OpenApiDocument;
            OpenApiDocument openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, "yaml").OpenApiDocument;
            openApiDoc_2.Workspace = new();
            openApiDoc_2.Workspace.AddDocument("http://localhost/callbackreference", openApiDoc);
            _localCallbackReference = new("callbackEvent", openApiDoc);
            _externalCallbackReference = new("callbackEvent", openApiDoc_2, "http://localhost/callbackreference");
        }

        [Fact]
        public void CallbackReferenceResolutionWorks()
        {
            // Assert
            Assert.NotEmpty(_localCallbackReference.PathItems);
            Assert.NotEmpty(_externalCallbackReference.PathItems);
            Assert.Equal("{$request.body#/callbackUrl}", _localCallbackReference.PathItems.First().Key.Expression);
            Assert.Equal("{$request.body#/callbackUrl}", _externalCallbackReference.PathItems.First().Key.Expression);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeCallbackReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });            

            // Act
            _localCallbackReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeCallbackReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localCallbackReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
