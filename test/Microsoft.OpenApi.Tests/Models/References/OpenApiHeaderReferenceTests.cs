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
    public class OpenApiHeaderReferenceTests
    {
        private const string OpenApi= @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    post:
      summary: Create a post
      responses:
        '201':
          description: Post created successfully
          headers:
            Location:
              $ref: '#/components/headers/LocationHeader'
components:
  headers:
    LocationHeader:
      description: The URL of the newly created post
      schema:
        type: string
";

        private const string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0
paths:
  /users:
    post:
      summary: Create a post
      responses:
        '201':
          description: Post created successfully
          headers:
            Location:
              $ref: '#/components/headers/LocationHeader'
";

        private readonly OpenApiHeaderReference _localHeaderReference;
        private readonly OpenApiHeaderReference _externalHeaderReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiHeaderReferenceTests()
        {
            var reader = new OpenApiStringReader();
            _openApiDoc = reader.Read(OpenApi, out _);
            _openApiDoc_2 = reader.Read(OpenApi_2, out _);
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/headerreference", _openApiDoc);

            _localHeaderReference = new OpenApiHeaderReference("LocationHeader", _openApiDoc)
            {
                Description = "Location of the locally created post"
            };

            _externalHeaderReference = new OpenApiHeaderReference("LocationHeader", _openApiDoc_2, "http://localhost/headerreference")
            {
                Description = "Location of the external created post"
            };
        }

        [Fact]
        public void HeaderReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("Location of the locally created post", _localHeaderReference.Description);
            Assert.Equal("Location of the external created post", _externalHeaderReference.Description);
            Assert.Equal("The URL of the newly created post",
                _openApiDoc.Components.Headers.First().Value.Description); // The main description value shouldn't change
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeHeaderReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localHeaderReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeHeaderReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _localHeaderReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeHeaderReferenceAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localHeaderReference.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
