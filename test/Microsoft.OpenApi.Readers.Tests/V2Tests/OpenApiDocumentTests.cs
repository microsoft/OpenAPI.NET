// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

        public OpenApiDocumentTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

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

            var result = OpenApiDocument.Parse(input, "yaml");

            result.OpenApiDiagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
                new( new OpenApiException("Unknown reference type 'defi888nition'")) });
            result.OpenApiDocument.Should().NotBeNull();
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

            var result = OpenApiDocument.Parse(input, "yaml");

            result.OpenApiDiagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
                new( new OpenApiException("Invalid Reference identifier 'doesnotexist'.")) });
            result.OpenApiDocument.Should().NotBeNull();
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

            var result = OpenApiDocument.Parse(
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
                "yaml");

            result.OpenApiDocument.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new()
                    {
                        Title = "Simple Document",
                        Version = "0.9.1",
                        Extensions =
                        {
                            ["x-extension"] = new OpenApiAny(2.335)
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

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi2_0 });
        }

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "twoResponses.json"));

            var okSchema = new OpenApiSchema
            {
                Reference = new()
                {
                    Type = ReferenceType.Schema,
                    Id = "Item",
                    HostDocument = result.OpenApiDocument
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
                    HostDocument = result.OpenApiDocument
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

            result.OpenApiDocument.Should().BeEquivalentTo(new OpenApiDocument
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
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleProduces.json"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Json);

            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, result.OpenApiDiagnostic.SpecificationVersion);

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
                        HostDocument = result.OpenApiDocument
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
                    HostDocument = result.OpenApiDocument
                }
            };
            var responses = result.OpenApiDocument.Paths["/items"].Operations[OperationType.Get].Responses;
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
            // Act
            var actual = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "ComponentRootReference.json")).OpenApiDocument;
            var schema1 = actual.Components.Schemas["AllPets"];
            Assert.False(schema1.UnresolvedReference);
            var schema2 = actual.ResolveReferenceTo<OpenApiSchema>(schema1.Reference);
            if (schema2.UnresolvedReference && schema1.Reference.Id == schema2.Reference.Id)
            {
                // detected a cycle - this code gets triggered
                Assert.Fail("A cycle should not be detected");
            }
        }

        [Fact]
        public void ParseDocumentWithDefaultContentTypeSettingShouldSucceed()
        {
            var settings = new OpenApiReaderSettings
            {
                DefaultContentType = ["application/json"]
            };

            var actual = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "docWithEmptyProduces.yaml"), settings);
            var mediaType = actual.OpenApiDocument.Paths["/example"].Operations[OperationType.Get].Responses["200"].Content;
            Assert.Contains("application/json", mediaType);
        }

        [Fact]
        public void testContentType()
        {
            var contentType = "application/json; charset = utf-8";
            var res = contentType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
            var expected = res.Split('/').LastOrDefault();
            Assert.Equal("application/json", res);
        }
    }
}
