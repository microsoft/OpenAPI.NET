// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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

        [Theory]
        [InlineData("en-US")]
        [InlineData("hi-IN")]
        // The equivalent of English 1,000.36 in French and Danish is 1.000,36
        [InlineData("fr-FR")]
        [InlineData("da-DK")]
        public void ParseDocumentWithDifferentCultureShouldSucceed(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            var openApiDoc = new OpenApiStringReader().Read(
                @"
swagger: 2.0
info: 
  title: Simple Document
  version: 0.9.1
definitions:
  sampleSchema:
    type: object
    properties:
      sampleProperty:
        type: double
        minimum: 100.54
        maximum: 60,000,000.35
        exclusiveMaximum: true
        exclusiveMinimum: false
paths: {}",
                out var context);

            openApiDoc.ShouldBeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Components = new OpenApiComponents()
                    {
                        Schemas =
                        {
                            ["sampleSchema"] = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["sampleProperty"] = new OpenApiSchema()
                                    {
                                        Type = "double",
                                        Minimum = (decimal)100.54,
                                        Maximum = (decimal)60000000.35,
                                        ExclusiveMaximum = true,
                                        ExclusiveMinimum = false
                                    }
                                },
                                Reference = new OpenApiReference()
                                {
                                    Id = "sampleSchema",
                                    Type = ReferenceType.Schema
                                }
                            }
                        }
                    },
                    Paths = new OpenApiPaths()
                });

            context.ShouldBeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi2_0 });
        }
    }
}