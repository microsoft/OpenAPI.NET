// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

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
                "yaml", SettingsFixture.ReaderSettings);

            Assert.Equal("0.9.1", result.Document.Info.Version, StringComparer.OrdinalIgnoreCase);
            var extension = Assert.IsType<JsonNodeExtension>(result.Document.Info.Extensions["x-extension"]);
            Assert.Equal(2.335M, extension.Node.GetValue<decimal>());
            var sampleSchema = Assert.IsType<OpenApiSchema>(result.Document.Components.Schemas["sampleSchema"]);
            var samplePropertySchema = Assert.IsType<OpenApiSchema>(sampleSchema.Properties["sampleProperty"]);
            var expectedPropertySchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Number,
                Minimum = "100.54",
                ExclusiveMaximum = "60000000.35",
            };

            Assert.Equivalent(expectedPropertySchema, samplePropertySchema);
        }

        [Fact]
        public async Task ShouldParseProducesInAnyOrder()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "twoResponses.json"));

            var okSchema = new OpenApiSchema
            {
                Properties = new()
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
                Properties = new()
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
                Schema = new OpenApiSchema()
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
                [
                    new OpenApiServer
                    {
                        Url = "https://"
                    }
                ],
                Paths = new()
                {
                    ["/items"] = new OpenApiPathItem()
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "An OK response",
                                        Content = new()
                                        {
                                            ["application/json"] = okMediaType,
                                            ["application/xml"] = okMediaType,
                                        }
                                    },
                                    ["default"] = new OpenApiResponse()
                                    {
                                        Description = "An error response",
                                        Content = new()
                                        {
                                            ["application/json"] = errorMediaType,
                                            ["application/xml"] = errorMediaType
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Post] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "An OK response",
                                        Content = new()
                                        {
                                            ["html/text"] = okMediaType
                                        }
                                    },
                                    ["default"] = new OpenApiResponse()
                                    {
                                        Description = "An error response",
                                        Content = new()
                                        {
                                            ["html/text"] = errorMediaType
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Patch] = new()
                            {
                                Responses =
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "An OK response",
                                        Content = new()
                                        {
                                            ["application/json"] = okMediaType,
                                            ["application/xml"] = okMediaType,
                                        }
                                    },
                                    ["default"] = new OpenApiResponse()
                                    {
                                        Description = "An error response",
                                        Content = new()
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
                    Schemas = new()
                    {
                        ["Item"] = okSchema,
                        ["Error"] = errorSchema
                    }
                }
            }, options => options.Excluding(x => x.BaseUri));
        }

        [Fact]
        public async Task ShouldAssignSchemaToAllResponses()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleProduces.json"));
            var result = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Json);

            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, result.Diagnostic.SpecificationVersion);

            var successSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchemaReference("Item", result.Document)
            };
            var errorSchema = new OpenApiSchemaReference("Error", result.Document);

            var responses = result.Document.Paths["/items"].Operations[HttpMethod.Get].Responses;
            foreach (var response in responses)
            {
                var targetSchema = response.Key == "200" ? (IOpenApiSchema)successSchema : errorSchema;

                var json = response.Value.Content["application/json"];
                Assert.NotNull(json);
                Assert.Equivalent(targetSchema, json.Schema);

                var xml = response.Value.Content["application/xml"];
                Assert.NotNull(xml);
                Assert.Equivalent(targetSchema, xml.Schema);
            }
        }

        [Fact]
        public async Task ShouldAllowComponentsThatJustContainAReference()
        {
            // Act
            var actual = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "ComponentRootReference.json"))).Document;
            var schema1 = actual.Components.Schemas["AllPets"];
            var schema1Reference = Assert.IsType<OpenApiSchemaReference>(schema1);
            Assert.False(schema1Reference.UnresolvedReference);
            var schema2 = actual.ResolveReferenceTo<OpenApiSchema>(schema1Reference.Reference);
            Assert.IsType<OpenApiSchema>(schema2);
            if (string.IsNullOrEmpty(schema1Reference.Reference.Id) || schema1Reference.UnresolvedReference)
            {
                // detected a cycle - this code gets triggered
                Assert.Fail("A cycle should not be detected");
            }
        }

        [Fact]
        public async Task ParseDocumentWithDefaultContentTypeSettingShouldSucceed()
        {
            var settings = new OpenApiReaderSettings
            {
                DefaultContentType = ["application/json"]
            };
            settings.AddYamlReader();

            var actual = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithEmptyProduces.yaml"), settings);
            var mediaType = actual.Document.Paths["/example"].Operations[HttpMethod.Get].Responses["200"].Content;
            Assert.Contains("application/json", mediaType);
        }

        [Fact]
        public void testContentType()
        {
            var contentType = "application/json; charset = utf-8";
            var res = contentType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.Equal("application/json", res);
        }
    }
}
