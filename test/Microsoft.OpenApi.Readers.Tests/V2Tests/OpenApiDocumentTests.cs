// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

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
  x-extension: 2.335
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
                        Version = "0.9.1",
                        Extensions =
                        {
                            ["x-extension"] = new OpenApiDouble(2.335)
                        }
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

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "twoResponses.json")))
            {
                var reader = new OpenApiStreamReader();
                var doc = reader.Read(stream, out var diagnostic);

                Assert.NotNull(doc.Paths["/items"]);
                Assert.Equal(3, doc.Paths["/items"].Operations.Count);

                foreach (var operation in doc.Paths["/items"].Operations)
                {
                    Assert.Equal(2, operation.Value.Responses.Count);

                    var okResponse = operation.Value.Responses["200"];
                    okResponse.ShouldBeEquivalentTo(
                        new OpenApiResponse()
                        {
                            Description = "An OK response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema()
                                        {
                                            Properties = new Dictionary<string, OpenApiSchema>()
                                            {
                                                { "id", new OpenApiSchema()
                                                    {
                                                        Type = "string",
                                                        Description = "Item identifier."
                                                    }
                                                }
                                            },
                                            Reference = new OpenApiReference()
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "Item"
                                            }
                                        }
                                    },
                                }
                            }
                        });

                    var errorResponse = operation.Value.Responses["default"];
                    errorResponse.ShouldBeEquivalentTo(
                        new OpenApiResponse()
                        {
                            Description = "An error response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Properties = new Dictionary<string, OpenApiSchema>()
                                        {
                                            { "code", new OpenApiSchema()
                                                {
                                                    Type = "integer",
                                                    Format = "int32"
                                                }
                                            },
                                            { "message", new OpenApiSchema()
                                                {
                                                    Type = "string"
                                                }
                                            },
                                            { "fields", new OpenApiSchema()
                                                {
                                                    Type = "string"
                                                }
                                            }
                                        },
                                        Reference = new OpenApiReference()
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "Error"
                                        }
                                    },
                                }
                            }
                        });
                }
            }
        }

        [Fact]
        public void ShouldAssignSchemaToAllResponses()
        {
            OpenApiDocument document;
            OpenApiDiagnostic diagnostic;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleProduces.json")))
            {
                document = new OpenApiStreamReader().Read(stream, out diagnostic);
            }

            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, diagnostic.SpecificationVersion);

            var responses = document.Paths["/items"].Operations[OperationType.Get].Responses;
            foreach (var content in responses.Values.Select(r => r.Content))
            {
                var json = content["application/json"];
                Assert.NotNull(json);
                Assert.NotNull(json.Schema);

                var xml = content["application/xml"];
                Assert.NotNull(xml);
                Assert.NotNull(xml.Schema);
            }
        }
    }
}