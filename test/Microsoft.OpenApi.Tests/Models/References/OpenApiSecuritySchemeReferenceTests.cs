// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers;
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
        public void ReferenceResolutionWorks()
        {
            // Assert
            Assert.Equal("X-API-Key", _openApiSecuritySchemeReference.Name);
            Assert.Equal(SecuritySchemeType.ApiKey, _openApiSecuritySchemeReference.Type);
        }
    }
}
