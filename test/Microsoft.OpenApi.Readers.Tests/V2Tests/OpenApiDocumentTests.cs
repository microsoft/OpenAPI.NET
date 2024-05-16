// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
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
        public void ShouldParseProducesInAnyOrder()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "twoResponses.json"));

            var okSchema = new JsonSchemaBuilder()
                    .Ref("#/definitions/Item");

            var errorSchema = new JsonSchemaBuilder()
                    .Ref("#/definitions/Error");

            var okMediaType = new OpenApiMediaType
            {
                Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(okSchema)
            };

            var errorMediaType = new OpenApiMediaType
            {
                Schema = errorSchema
            };

            var expected = new OpenApiDocument
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
                            ["Item"] = new JsonSchemaBuilder()
                                            .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Item identifier."))),
                            ["Error"] = new JsonSchemaBuilder()
                                            .Properties(
                                            ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                                            ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                            ("fields", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                        }
                }
            };
            expected.Paths["/items"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Get].Responses["200"].Content["application/xml"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Get].Responses["default"].Content["application/json"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Get].Responses["default"].Content["application/xml"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Post].Responses["200"].Content["html/text"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Post].Responses["default"].Content["html/text"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Patch].Responses["200"].Content["application/json"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Patch].Responses["200"].Content["application/xml"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Patch].Responses["default"].Content["application/json"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Paths["/items"].Operations[OperationType.Patch].Responses["default"].Content["application/xml"].Schema.BaseUri = result.OpenApiDocument.BaseUri;
            expected.Components.Schemas["Item"].BaseUri = result.OpenApiDocument.BaseUri;
            expected.Components.Schemas["Error"].BaseUri = result.OpenApiDocument.BaseUri;

            result.OpenApiDocument.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));
        }

        [Fact]
        public void ShouldAssignSchemaToAllResponses()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleProduces.json"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Json);

            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, result.OpenApiDiagnostic.SpecificationVersion);

            var successSchema = new JsonSchemaBuilder()
                .Type(SchemaValueType.Array)
                .Items(new JsonSchemaBuilder()
                    .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Item identifier."))));

            var errorSchema = new JsonSchemaBuilder()
                    .Ref("#/definitions/Error");

            var responses = result.OpenApiDocument.Paths["/items"].Operations[OperationType.Get].Responses;
            foreach (var response in responses)
            {
                var targetSchema = response.Key == "200" ? successSchema.Build() : errorSchema.Build();

                var json = response.Value.Content["application/json"];
                Assert.NotNull(json);
                Assert.Equal(json.Schema.Keywords.Count, targetSchema.Keywords.Count);

                var xml = response.Value.Content["application/xml"];
                Assert.NotNull(xml);
                Assert.Equal(xml.Schema.Keywords.Count, targetSchema.Keywords.Count);
            }
        }

        [Fact]
        public void ShouldAllowComponentsThatJustContainAReference()
        {
            // Act
            var actual = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "ComponentRootReference.json")).OpenApiDocument;
            JsonSchema schema = actual.Components.Schemas["AllPets"];

            schema = actual.ResolveJsonSchemaReference(schema.GetRef()) ?? schema;

            // Assert
            if (schema.Keywords.Count.Equals(1) && schema.GetRef() != null)
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
