// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiDocumentTests
    {
        [Fact]
        public void ShouldThrowWhenReferenceTypeIsInvalid()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
paths: 
  '/':
    get:
      responses:
        '200':
          description: ok
          schema:
            $ref: '#/defi888nition/does/notexist'
";

            var reader = new OpenApiStringReader();
            var doc = reader.Read(input, out var diagnostic);

            diagnostic.Errors.ShouldBeEquivalentTo(new List<OpenApiError> {
                new OpenApiError( new OpenApiException("Unknown reference type 'defi888nition'")) });
            doc.Should().NotBeNull();
        }


        [Fact]
        public void ShouldThrowWhenReferenceDoesNotExist()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
paths: 
  '/':
    get:
      produces: ['application/json']
      responses:
        '200':
          description: ok
          schema:
            $ref: '#/definitions/doesnotexist'
";

            var reader = new OpenApiStringReader();

            var doc = reader.Read(input, out var diagnostic);

            diagnostic.Errors.ShouldBeEquivalentTo(new List<OpenApiError> {
                new OpenApiError( new OpenApiException("Invalid Reference identifier 'doesnotexist'.")) });
            doc.Should().NotBeNull();
        }
    }
}