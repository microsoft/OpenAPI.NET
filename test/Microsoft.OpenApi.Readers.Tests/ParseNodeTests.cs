// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ParseNodeTests
    {
        public ParseNodeTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void BrokenSimpleList()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: hey
                  version: 1.0.0
                schemes: [ { "hello" }]
                paths: { }
                """;

            var result = OpenApiDocument.Parse(input, "yaml");

            result.Diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError>() {
                new OpenApiError(new OpenApiReaderException("Expected a value."))
            });
        }

        [Fact]
        public void BadSchema()
        {
            var input = """
                        openapi: 3.0.0
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
                        """;

            var res= OpenApiDocument.Parse(input, "yaml");

            res.Diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError>
            {
                new(new OpenApiReaderException("schema must be a map/object") {
                    Pointer = "#/paths/~1foo/get/responses/200/content/application~1json/schema"
                })
            });
        }
    }
}

