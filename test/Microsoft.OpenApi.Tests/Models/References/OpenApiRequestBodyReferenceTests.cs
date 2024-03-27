// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Json.Schema;
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
    public class OpenApiRequestBodyReferenceTests
    {
        private readonly string OpenApi = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0

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

        private readonly string OpenApi_2 = @"
openapi: 3.0.0
info:
  title: Sample API
  version: 1.0.0

paths:
  /users:
    post:
      summary: Create a user
      requestBody:
        $ref: '#/components/requestBodies/UserRequest'  # <---- referencing the requestBody here
      responses:
        '201':
          description: User created
";

        private readonly OpenApiRequestBodyReference _localRequestBodyReference;
        private readonly OpenApiRequestBodyReference _externalRequestBodyReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiRequestBodyReferenceTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            _openApiDoc = OpenApiDocument.Parse(OpenApi, "yaml").OpenApiDocument;
            _openApiDoc_2 = OpenApiDocument.Parse(OpenApi_2, "yaml").OpenApiDocument;
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/requestbodyreference", _openApiDoc);

            _localRequestBodyReference = new("UserRequest", _openApiDoc)
            {
                Description = "User request body"
            };

            _externalRequestBodyReference = new("UserRequest", _openApiDoc_2, "http://localhost/requestbodyreference")
            {
                Description = "External Reference: User request body"
            };
        }

        [Fact]
        public void RequestBodyReferenceResolutionWorks()
        {
            // Arrange
            var expectedMediaType = @"{
  ""schema"": {
    ""type"": ""object"",
    ""properties"": {
      ""name"": {
        ""type"": ""string""
      },
      ""email"": {
        ""type"": ""string""
      }
    }
  }
}";
            var mediaType = _localRequestBodyReference.Content["application/json"];
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);

            // Act
            mediaType.SerializeAsV3(new OpenApiJsonWriter(outputStringWriter, 
                new OpenApiJsonWriterSettings { InlineLocalReferences = true }));
            var serialized = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            serialized.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expectedMediaType.MakeLineBreaksEnvironmentNeutral());
            Assert.Equal("User request body", _localRequestBodyReference.Description);
            Assert.Equal("application/json", _localRequestBodyReference.Content.First().Key);
            Assert.Equal("External Reference: User request body", _externalRequestBodyReference.Description);
            Assert.Equal("User creation request body", _openApiDoc.Components.RequestBodies.First().Value.Description);
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
