// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{


    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

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

        [Fact]
        public void ShouldParseProducesInAnyOrder()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "twoResponses.json")))
            {
                var reader = new OpenApiStreamReader();
                var doc = reader.Read(stream, out var diagnostic);

                var successSchema = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Array)
                            .Ref("Item")
                            .Items(new JsonSchemaBuilder()
                                .Ref("Item"));

                var okSchema = new JsonSchemaBuilder()
                        .Ref("Item")
                        .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Item identifier.")));

                var errorSchema = new JsonSchemaBuilder()
                        .Ref("Error")
                        .Properties(("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                        ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                        ("fields", new JsonSchemaBuilder().Type(SchemaValueType.String)));

                var okMediaType = new OpenApiMediaType
                {
                    Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(okSchema)
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

            var successSchema = new JsonSchemaBuilder()
                .Type(SchemaValueType.Array)
                .Items(new JsonSchemaBuilder()
                    .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.String).Description("Item identifier.")))
                    .Ref("Item"))
                .Build();

            var errorSchema = new JsonSchemaBuilder()
                    .Properties(("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                        ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                        ("fields", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                    .Ref("Error")
                    .Build();

            var responses = document.Paths["/items"].Operations[OperationType.Get].Responses;
            foreach (var response in responses)
            {
                var targetSchema = response.Key == "200" ? successSchema : errorSchema;

                var json = response.Value.Content["application/json"];
                Assert.NotNull(json);
                //Assert.Equal(json.Schema.Keywords.OfType<TypeKeyword>().FirstOrDefault().Type, targetSchema.Build().GetJsonType());
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
                JsonSchema schema1 = doc.Components.Schemas["AllPets"];
                //Assert.False(schema1.UnresolvedReference);
                //JsonSchema schema2 = doc.ResolveReferenceTo<JsonSchema>(schema1.GetRef());
                //if (schema1.GetRef() == schema2.GetRef())
                //{
                //    // detected a cycle - this code gets triggered
                //    Assert.True(false, "A cycle should not be detected");
                //}
            }
        }
    }
}
