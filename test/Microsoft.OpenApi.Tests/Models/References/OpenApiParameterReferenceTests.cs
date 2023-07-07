﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class OpenApiParameterReferenceTests
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
      parameters:
        - $ref: '#/components/parameters/limitParam'
      responses:
        200:
          description: Successful operation
components:
  parameters:
    limitParam:
      name: limit
      in: query
      description: Number of results to return
      schema:
        type: integer
        minimum: 1
        maximum: 100
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
      parameters:
        - $ref: '#/components/parameters/limitParam'
      responses:
        200:
          description: Successful operation
";
        private readonly OpenApiParameterReference _localParameterReference;
        private readonly OpenApiParameterReference _externalParameterReference;
        private readonly OpenApiDocument _openApiDoc;
        private readonly OpenApiDocument _openApiDoc_2;

        public OpenApiParameterReferenceTests()
        {
            var reader = new OpenApiStringReader();
            _openApiDoc = reader.Read(OpenApi, out _);
            _openApiDoc_2 = reader.Read(OpenApi_2, out _);
            _openApiDoc_2.Workspace = new();
            _openApiDoc_2.Workspace.AddDocument("http://localhost/parameterreference", _openApiDoc);

            _localParameterReference = new("limitParam", _openApiDoc)
            {
                Description = "Results to return"
            };

            _externalParameterReference = new OpenApiParameterReference("limitParam", _openApiDoc_2, "http://localhost/parameterreference")
            {
                Description = "Externally referenced: Results to return"
            };
        }

        [Fact]
        public void ParameterReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("limit", _localParameterReference.Name);
            Assert.Equal("Results to return", _localParameterReference.Description);
            Assert.Equal("Externally referenced: Results to return", _externalParameterReference.Description);
            Assert.Equal("Number of results to return",
                _openApiDoc.Components.Parameters.First().Value.Description); // The main description value shouldn't change
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localParameterReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localParameterReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterReferenceAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            _localParameterReference.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
