﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeReferenceTests
    {
        private const string OpenApi = @"
openapi: 3.0.3
info:
  title: Sample API
  version: 1.0.0
servers: 
  - url: https://myserver.com/v1.0
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
            var result = OpenApiDocument.Parse(OpenApi, "yaml", SettingsFixture.ReaderSettings);
            _openApiSecuritySchemeReference = new("mySecurityScheme", result.Document);
        }

        [Fact]
        public void SecuritySchemeResolutionWorks()
        {
            // Assert
            Assert.Equal("X-API-Key", _openApiSecuritySchemeReference.Name);
            Assert.Equal(SecuritySchemeType.ApiKey, _openApiSecuritySchemeReference.Type);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task SerializeSecuritySchemeReferenceAsV3JsonWorks(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _openApiSecuritySchemeReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert            
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task SerializeSecuritySchemeReferenceAsV31JsonWorks(bool produceTerseOutput, bool inlineLocalReferences)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput, InlineLocalReferences = inlineLocalReferences });

            // Act
            _openApiSecuritySchemeReference.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput, inlineLocalReferences);
        }
    }
}
