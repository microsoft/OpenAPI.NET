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
    public class OpenApiSecuritySchemeReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0

paths:
  /users:
    get:
      summary: Retrieve users
      security:
        - mySecurityScheme: []

components:
  securitySchemes:
    mySecurityScheme:
      type: apiKey
      name: X-API-Key
      in: header
";

        readonly OpenApiSecuritySchemeReference _openApiSecuritySchemeReference;

        public OpenApiSecuritySchemeReferenceTests()
        {
            var reader = new OpenApiStringReader();
            OpenApiDocument openApiDoc = reader.Read(OpenApi, out _);
            _openApiSecuritySchemeReference = new("mySecurityScheme", openApiDoc);
        }

        [Fact]
        public void SecuritySchemeResolutionWorks()
        {
            // Assert
            Assert.Equal("X-API-Key", _openApiSecuritySchemeReference.Name);
            Assert.Equal(SecuritySchemeType.ApiKey, _openApiSecuritySchemeReference.Type);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSecuritySchemeReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _openApiSecuritySchemeReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSecuritySchemeReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = true });

            // Act
            _openApiSecuritySchemeReference.SerializeAsV31(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
