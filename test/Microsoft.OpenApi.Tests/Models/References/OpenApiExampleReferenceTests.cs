// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
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
       value:
        - id: 1
          name: John Doe
";

        readonly OpenApiExampleReference _openApiExampleReference;

        public OpenApiExampleReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiExampleReference = new OpenApiExampleReference("UserExample", openApiDoc)
            {
                Summary = "Example of an admin user"
            };
        }

        [Fact]
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Example of an admin user", _openApiExampleReference.Summary);
            Assert.NotNull(_openApiExampleReference.Value);            
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
            _openApiExampleReference.SerializeAsV3(writer);
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
            _openApiExampleReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }    
}
