using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
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
    public class OpenApiCallbackReferenceTests
    {        
        const string openapi = @"
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
        
        readonly OpenApiCallbackReference _callbackReference;
        
        public OpenApiCallbackReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(openapi, out _);
            _callbackReference = new("callbackEvent", openApiDoc);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedCallbackAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });
            

            // Act
            _callbackReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            Assert.NotEmpty(_callbackReference.PathItems);
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
