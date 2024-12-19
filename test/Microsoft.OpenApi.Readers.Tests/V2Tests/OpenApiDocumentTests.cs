// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
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

            result.Document.Should().BeEquivalentTo(
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
                                Type = JsonSchemaType.Object,
                                Properties =
                                {
                                    ["sampleProperty"] = new()
                                    {
                                        Type = JsonSchemaType.Number,
                                        Minimum = (decimal)100.54,
                                        Maximum = (decimal)60000000.35,
                                        ExclusiveMaximum = true,
                                        ExclusiveMinimum = false
                                    }
                                }
                            }
                        }
                    },
                    Paths = new()
                }, options => options
                .Excluding(x=> x.BaseUri)
                .Excluding((IMemberInfo memberInfo) =>
                                        memberInfo.Path.EndsWith("Parent"))
                .Excluding((IMemberInfo memberInfo) =>
                                        memberInfo.Path.EndsWith("Root")));;
        }

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "twoResponses.json"));

            var okSchema = new OpenApiSchema
            {
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "id", new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Description = "Item identifier."
                        }
                    }
                }
            };

            var errorSchema = new OpenApiSchema
            {
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "code", new OpenApiSchema
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int32"
                        }
                    },
                    { "message", new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    { "fields", new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                }
            };

            var okMediaType = new OpenApiMediaType
            {
                Schema = new()
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchemaReference("Item", result.Document)
                }
            };

            var errorMediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchemaReference("Error", result.Document)
            };

            result.Document.Should().BeEquivalentTo(new OpenApiDocument
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
            }, options => options.Excluding(x => x.BaseUri));
        }

        [Fact]
        public void ShouldAssignSchemaToAllResponses()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleProduces.json"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Json);

            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, result.Diagnostic.SpecificationVersion);

            var successSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchemaReference("Item", result.Document)
            };
            var errorSchema = new OpenApiSchemaReference("Error", result.Document);

            var responses = result.Document.Paths["/items"].Operations[OperationType.Get].Responses;
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
            var actual = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "ComponentRootReference.json")).Document;
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
            var mediaType = actual.Document.Paths["/example"].Operations[OperationType.Get].Responses["200"].Content;
            Assert.Contains("application/json", mediaType);
        }

        [Fact]
        public void testContentType()
        {
            var contentType = "application/json; charset = utf-8";
            var res = contentType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
            Assert.Equal("application/json", res);
        }
    }
}
