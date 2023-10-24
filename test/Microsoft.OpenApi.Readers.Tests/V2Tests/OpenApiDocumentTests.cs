// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
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
            var input =
                """
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
                """;

            var reader = new OpenApiStringReader();
            var doc = reader.Read(input, out var diagnostic);

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
                new( new OpenApiException("Unknown reference type 'defi888nition'")) });
            doc.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowWhenReferenceDoesNotExist()
        {
            var input =
                """
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
                """;

            var reader = new OpenApiStringReader();

            var doc = reader.Read(input, out var diagnostic);

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
                new( new OpenApiException("Invalid Reference identifier 'doesnotexist'.")) });
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
            Thread.CurrentThread.CurrentCulture = new(culture);
            Thread.CurrentThread.CurrentUICulture = new(culture);

            var openApiDoc = new OpenApiStringReader().Read(
                """
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
                        maximum: 60000000.35
                        exclusiveMaximum: true
                        exclusiveMinimum: false
                paths: {}
                """,
                out var context);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new()
                    {
                        Title = "Simple Document",
                        Version = "0.9.1",
                        Extensions =
                        {
                            ["x-extension"] = new OpenApiDouble(2.335)
                        }
                    },
                    Components = new()
                    {
                        Schemas =
                        {
                            ["sampleSchema"] = new()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["sampleProperty"] = new()
                                    {
                                        Type = "double",
                                        Minimum = (decimal)100.54,
                                        Maximum = (decimal)60000000.35,
                                        ExclusiveMaximum = true,
                                        ExclusiveMinimum = false
                                    }
                                },
                                Reference = new()
                                {
                                    Id = "sampleSchema",
                                    Type = ReferenceType.Schema
                                }
                            }
                        }
                    },
                    Paths = new()
                });

            context.Should().BeEquivalentTo(
                new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi2_0 });
        }

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "twoResponses.json"));
            var reader = new OpenApiStreamReader();
            var doc = reader.Read(stream, out var diagnostic);

            var okSchema = new OpenApiSchema
            {
                Reference = new()
                {
                    Type = ReferenceType.Schema,
                    Id = "Item",
                    HostDocument = doc
                },
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "id", new OpenApiSchema
                        {
                            Type = "string",
                            Description = "Item identifier."
                        }
                    }
                }
            };

            var errorSchema = new OpenApiSchema
            {
                Reference = new()
                {
                    Type = ReferenceType.Schema,
                    Id = "Error",
                    HostDocument = doc
                },
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "code", new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int32"
                        }
                    },
                    { "message", new OpenApiSchema
                        {
                            Type = "string"
                        }
                    },
                    { "fields", new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                }
            };

            var okMediaType = new OpenApiMediaType
            {
                Schema = new()
                {
                    Type = "array",
                    Items = okSchema
                }
            };

            var errorMediaType = new OpenApiMediaType
            {
                Schema = errorSchema
            };

            doc.Should().BeEquivalentTo(new OpenApiDocument
            {
                Info = new()
                {
                    Title = "Two responses",
                    Version = "1.0.0"
                },
                Servers =
                {
                    new OpenApiServer
                    {
                        Url = "https://"
                    }
                },
                Paths = new()
                {
                    ["/items"] = new()
                    {
                        Operations =
                        {
                            [OperationType.Get] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new()
                                    {
                                        Description = "An OK response",
                                        Content =
                                        {
                                            ["application/json"] = okMediaType,
                                            ["application/xml"] = okMediaType,
                                        }
                                    },
                                    ["default"] = new()
                                    {
                                        Description = "An error response",
                                        Content =
                                        {
                                            ["application/json"] = errorMediaType,
                                            ["application/xml"] = errorMediaType
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new()
                                    {
                                        Description = "An OK response",
                                        Content =
                                        {
                                            ["html/text"] = okMediaType
                                        }
                                    },
                                    ["default"] = new()
                                    {
                                        Description = "An error response",
                                        Content =
                                        {
                                            ["html/text"] = errorMediaType
                                        }
                                    }
                                }
                            },
                            [OperationType.Patch] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new()
                                    {
                                        Description = "An OK response",
                                        Content =
                                        {
                                            ["application/json"] = okMediaType,
                                            ["application/xml"] = okMediaType,
                                        }
                                    },
                                    ["default"] = new()
                                    {
                                        Description = "An error response",
                                        Content =
                                        {
                                            ["application/json"] = errorMediaType,
                                            ["application/xml"] = errorMediaType
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Components = new()
                {
                    Schemas =
                    {
                        ["Item"] = okSchema,
                        ["Error"] = errorSchema
                    }
                }
            });
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

            var successSchema = new OpenApiSchema
            {
                Type = "array",
                Items = new()
                {
                    Properties = {
                        { "id", new OpenApiSchema
                            {
                                Type = "string",
                                Description = "Item identifier."
                            }
                        }
                    },
                    Reference = new()
                    {
                        Id = "Item",
                        Type = ReferenceType.Schema,
                        HostDocument = document
                    }
                }
            };
            var errorSchema = new OpenApiSchema
            {
                Properties = {
                    { "code", new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int32"
                        }
                    },
                    { "message", new OpenApiSchema
                        {
                            Type = "string"
                        }
                    },
                    { "fields", new OpenApiSchema
                        {
                            Type = "string"
                        }
                    }
                },
                Reference = new()
                {
                    Id = "Error",
                    Type = ReferenceType.Schema,
                    HostDocument = document
                }
            };
            var responses = document.Paths["/items"].Operations[OperationType.Get].Responses;
            foreach (var response in responses)
            {
                var targetSchema = response.Key == "200" ? successSchema : errorSchema;

                var json = response.Value.Content["application/json"];
                Assert.NotNull(json);
                json.Schema.Should().BeEquivalentTo(targetSchema);

                var xml = response.Value.Content["application/xml"];
                Assert.NotNull(xml);
                xml.Schema.Should().BeEquivalentTo(targetSchema);
            }
        }

        [Fact]
        public void ShouldAllowComponentsThatJustContainAReference()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "ComponentRootReference.json"));
            var reader = new OpenApiStreamReader();
            var doc = reader.Read(stream, out var diags);
            var schema1 = doc.Components.Schemas["AllPets"];
            Assert.False(schema1.UnresolvedReference);
            var schema2 = doc.ResolveReferenceTo<OpenApiSchema>(schema1.Reference);
            if (schema2.UnresolvedReference && schema1.Reference.Id == schema2.Reference.Id)
            {
                // detected a cycle - this code gets triggered
                Assert.Fail("A cycle should not be detected");
            }
        }

        [Fact]
        public void ParseDocumentWithDefaultContentTypeSettingShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "docWithEmptyProduces.yaml"));
            var doc = new OpenApiStreamReader(new() { DefaultContentType =  new() { "application/json" } })
                .Read(stream, out var diags);
            var mediaType = doc.Paths["/example"].Operations[OperationType.Get].Responses["200"].Content;
            Assert.Contains("application/json", mediaType);
        }
    }
}
