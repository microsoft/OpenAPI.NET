// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Exceptions;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ParseNodeTests
    {
        [Fact]
        public void BrokenSimpleList()
        {
            var input = @"swagger: 2.0
info:
  title: hey
  version: 1.0.0
schemes: [ { ""hello"" }]
paths: { }";

            var reader = new OpenApiStringReader();
            reader.Read(input, out var diagnostic);

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError>() {
                new OpenApiError(new OpenApiReaderException("Expected a value.") {
                    Pointer = "#line=4"
                })
            });
        }

        [Fact]
        public void BadSchema()
        {
            var input = @"openapi: 3.0.0
info:
  title: foo
  version: bar
paths:
  '/foo':
    get:
      responses:
        200: 
          description: ok
          content:
            application/json:  
              schema: asdasd
";

            var reader = new OpenApiStringReader();
            reader.Read(input, out var diagnostic);

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError>() {
                new OpenApiError(new OpenApiReaderException("schema must be a map/object") {
                    Pointer = "#/paths/~1foo/get/responses/200/content/application~1json/schema"
                })
            });
        }
    }
}

