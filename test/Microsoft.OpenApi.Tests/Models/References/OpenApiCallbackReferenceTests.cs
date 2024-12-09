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
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackReferenceTests
    {
        // OpenApi doc with external $ref
        private const string OpenApi = @"
openapi: 3.0.0
info:
  title: Callback with ref Example
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
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
          $ref: 'https://myserver.com/beta#/components/callbacks/callbackEvent'";

        // OpenApi doc with local $ref
        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Callback with ref Example
  version: 1.0.0
servers: 
  - url: https://myserver.com/beta
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

        private readonly OpenApiCallbackReference _externalCallbackReference;
        private readonly OpenApiCallbackReference _localCallbackReference;

        public OpenApiCallbackReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            OpenApiDocument openApiDoc = OpenApiDocument.Parse(OpenApi, OpenApiConstants.Yaml).Document;
            OpenApiDocument openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, OpenApiConstants.Yaml).Document;
            openApiDoc.Workspace.AddDocumentId("https://myserver.com/beta", openApiDoc_2.BaseUri);
            openApiDoc.Workspace.RegisterComponents(openApiDoc_2);
            _externalCallbackReference = new("callbackEvent", openApiDoc, "https://myserver.com/beta");
            _localCallbackReference = new("callbackEvent", openApiDoc_2);
        }

        [Fact]
        public void CallbackReferenceResolutionWorks()
        {
            // Assert
            // External reference resolution works
            Assert.NotEmpty(_externalCallbackReference.PathItems);
            Assert.Single(_externalCallbackReference.PathItems);
            Assert.Equal("{$request.body#/callbackUrl}", _externalCallbackReference.PathItems.First().Key.Expression);
            Assert.Equal(OperationType.Post, _externalCallbackReference.PathItems.FirstOrDefault().Value.Operations.FirstOrDefault().Key);;

            // Local reference resolution works
            Assert.NotEmpty(_localCallbackReference.PathItems);
            Assert.Single(_localCallbackReference.PathItems);
            Assert.Equal("{$request.body#/callbackUrl}", _localCallbackReference.PathItems.First().Key.Expression);
            Assert.Equal(OperationType.Post, _localCallbackReference.PathItems.FirstOrDefault().Value.Operations.FirstOrDefault().Key); ;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeCallbackReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineExternalReferences = true });            

            // Act
            _externalCallbackReference.SerializeAsV3(writer);
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
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineExternalReferences = true });

            // Act
            _externalCallbackReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
