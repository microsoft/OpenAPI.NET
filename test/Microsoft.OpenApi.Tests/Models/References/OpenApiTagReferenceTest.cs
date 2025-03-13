// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
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
    public class OpenApiTagReferenceTest
    {
        private const string OpenApi = @"openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /users/{userId}:
    get:
      summary: Returns a user by ID.
      parameters:
        - name: userId
          in: path
          required: true
          description: The ID of the user to return.
          schema:
            type: integer
      responses:
        '200':
          description: A user object.
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/User'
        '404':
          description: The user was not found.
      tags:
        - $ref: '#/tags/user'
        - users.user
components:
  schemas:
    User:
      type: object
      properties:
        id:
          type: integer
        name:
          type: string
tags:
  - name: user
    description: Operations about users.
";

        readonly OpenApiTagReference _openApiTagReference;
        readonly OpenApiTagReference _openApiTagReference2;
        readonly OpenApiDocument _openApiDocument;

        public OpenApiTagReferenceTest()
        {
            var result = OpenApiDocument.Parse(OpenApi, "yaml", SettingsFixture.ReaderSettings);
            _openApiDocument = result.Document;
            _openApiTagReference = new("user", result.Document);
            _openApiTagReference2 = new("users.user", result.Document);
        }

        [Fact]
        public void TagReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("user", _openApiTagReference.Name);
            Assert.Equal("Operations about users.", _openApiTagReference.Description);
            Assert.True(_openApiTagReference2.UnresolvedReference);// the target is null
            var operationTags = _openApiDocument.Paths["/users/{userId}"].Operations[HttpMethod.Get].Tags;
            Assert.Null(operationTags); // the operation tags are not loaded due to the invalid syntax at the operation level(should be a list of strings)
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _openApiTagReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _openApiTagReference.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _openApiTagReference2.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert 
            Assert.Equal("\"users.user\"", outputStringWriter.ToString());
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _openApiTagReference2.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            Assert.Equal("\"users.user\"", outputStringWriter.ToString());
        }
    }
}
