// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Globalization;
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

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
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

            diagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
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
        maximum: 60000000.35
        exclusiveMaximum: true
        exclusiveMinimum: false
paths: {}",
                out var context);

            openApiDoc.Should().BeEquivalentTo(
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

            context.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi2_0 });
        }

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "twoResponses.json")))
            {
                var reader = new OpenApiStreamReader();
                var doc = reader.Read(stream, out var diagnostic);

                var successSchema = new OpenApiSchema()
                {
                    Type = "array",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "Item"
                    },
                    Items = new OpenApiSchema()
                    {
                        //Properties = new Dictionary<string, OpenApiSchema>()
                        //                    {
                        //                        { "id", new OpenApiSchema()
                        //                            {
                        //                                Type = "string",
                        //                                Description = "Item identifier."
                        //                            }
                        //                        }
                        //                    },
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.Schema,
                            Id = "Item"
                        }
                    }
                };

                var okSchema = new OpenApiSchema()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "Item"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>()
                                                    {
                                                        { "id", new OpenApiSchema()
                                                            {
                                                                Type = "string",
                                                                Description = "Item identifier."
                                                            }
                                                        }
                                                    }
                };

                var errorSchema = new OpenApiSchema()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "Error"
                    },
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
                                                    }
                };

                var okMediaType = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
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
                    Info = new OpenApiInfo
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
                    Paths = new OpenApiPaths
                    {
                        ["/items"] = new OpenApiPathItem
                        {
                            Operations =
                            {
                                [OperationType.Get] = new OpenApiOperation
                                {
                                    Responses =
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "An OK response",
                                            Content =
                                            {
                                                ["application/json"] = okMediaType,
                                                ["application/xml"] = okMediaType,
                                            }
                                        },
                                        ["default"] = new OpenApiResponse
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
                                [OperationType.Post] = new OpenApiOperation
                                {
                                    Responses =
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "An OK response",
                                            Content =
                                            {
                                                ["html/text"] = okMediaType
                                            }
                                        },
                                        ["default"] = new OpenApiResponse
                                        {
                                            Description = "An error response",
                                            Content =
                                            {
                                                ["html/text"] = errorMediaType
                                            }
                                        }
                                    }
                                },
                                [OperationType.Patch] = new OpenApiOperation
                                {
                                    Responses =
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "An OK response",
                                            Content =
                                            {
                                                ["application/json"] = okMediaType,
                                                ["application/xml"] = okMediaType,
                                            }
                                        },
                                        ["default"] = new OpenApiResponse
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
                    Components = new OpenApiComponents
                    {
                        Schemas =
                        {
                            ["Item"] = okSchema,
                            ["Error"] = errorSchema
                        }
                    }
                });
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

            var successSchema = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Properties = {
                        { "id", new OpenApiSchema
                            {
                                Type = "string",
                                Description = "Item identifier."
                            }
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Id = "Item",
                        Type = ReferenceType.Schema
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
                Reference = new OpenApiReference
                {
                    Id = "Error",
                    Type = ReferenceType.Schema
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
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "ComponentRootReference.json")))
            {
                OpenApiStreamReader reader = new OpenApiStreamReader();
                OpenApiDocument doc = reader.Read(stream, out OpenApiDiagnostic diags);
                OpenApiSchema schema1 = doc.Components.Schemas["AllPets"];
                Assert.False(schema1.UnresolvedReference);
                OpenApiSchema schema2 = (OpenApiSchema)doc.ResolveReference(schema1.Reference);
                if (schema2.UnresolvedReference && schema1.Reference.Id == schema2.Reference.Id)
                {
                    // detected a cycle - this code gets triggered
                    Assert.True(false, "A cycle should not be detected");
                }
            }
        }
    }
}
