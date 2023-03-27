// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Draft4Support;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSchema/";

        [Fact]
        public void ParsePrimitiveSchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "primitiveSchema.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var node = new MapNode(context, (YamlMappingNode)yamlNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new OpenApiSchema
                    {
                        Type = "string",
                        Format = "email"
                    });
            }
        }

        [Fact]
        public void ParsePrimitiveSchemaFragmentShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "primitiveSchema.yaml")))
            {
                var reader = new OpenApiStreamReader();
                var diagnostic = new OpenApiDiagnostic();

                // Act
                var schema = reader.ReadFragment<OpenApiSchema>(stream, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new OpenApiSchema
                    {
                        Type = "string",
                        Format = "email"
                    });
            }
        }

        [Fact]
        public void ParsePrimitiveStringSchemaFragmentShouldSucceed()
        {
            var input = @"
{ ""type"": ""integer"",
""format"": ""int64"",
""default"": 88
}
";
            var reader = new OpenApiStringReader();
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var schema = reader.ReadFragment<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            schema.Should().BeEquivalentTo(
                new OpenApiSchema
                {
                    Type = "integer",
                    Format = "int64",
                    Default = new OpenApiLong(88)
                });
        }

        [Fact]
        public void ParseExampleStringFragmentShouldSucceed()
        {
            var input = @"
{ 
  ""foo"": ""bar"",
  ""baz"": [ 1,2]
}";
            var reader = new OpenApiStringReader();
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = reader.ReadFragment<IOpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(
                new OpenApiObject
                {
                    ["foo"] = new OpenApiString("bar"),
                    ["baz"] = new OpenApiArray() {
                        new OpenApiInteger(1),
                        new OpenApiInteger(2)
                    }
                });
        }

        [Fact]
        public void ParseEnumFragmentShouldSucceed()
        {
            var input = @"
[ 
  ""foo"",
  ""baz""
]";
            var reader = new OpenApiStringReader();
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = reader.ReadFragment<IOpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(
                new OpenApiArray
                {
                    new OpenApiString("foo"),
                    new OpenApiString("baz")
                });
        }

        [Fact]
        public void ParseSimpleSchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "simpleSchema.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var node = new MapNode(context, (YamlMappingNode)yamlNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new OpenApiSchema
                    {
                        Type = "object",
                        Required =
                        {
                            "name"
                        },
                        Properties =
                        {
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["address"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["age"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int32",
                                Minimum = 0
                            }
                        },
                        AdditionalPropertiesAllowed = false
                    });
            }
        }

        [Fact]
        public void ParsePathFragmentShouldSucceed()
        {
            var input = @"
summary: externally referenced path item
get:
  responses:
    '200':
      description: Ok
";
            var reader = new OpenApiStringReader();
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = reader.ReadFragment<OpenApiPathItem>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(
                new OpenApiPathItem
                {
                    Summary = "externally referenced path item",
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation()
                        {
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Ok"
                                }
                            }
                        }
                    }
                });
        }

        [Fact]
        public void ParseDictionarySchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "dictionarySchema.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var node = new MapNode(context, (YamlMappingNode)yamlNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalProperties = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    });
            }
        }

        [Fact]
        public void ParseBasicSchemaWithExampleShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicSchemaWithExample.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var node = new MapNode(context, (YamlMappingNode)yamlNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        },
                        Required =
                        {
                            "name"
                        },
                        Example = new OpenApiObject
                        {
                            ["name"] = new OpenApiString("Puma"),
                            ["id"] = new OpenApiLong(1)
                        }
                    });
            }
        }

        [Fact]
        public void ParseBasicSchemaWithReferenceShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicSchemaWithReference.yaml"));
            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // Assert
            var components = openApiDoc.Components;

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                    {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                    }
                });

            components.Should().BeEquivalentTo(
                new OpenApiComponents
                {
                    Schemas =
                    {
                        ["ErrorModel"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Properties(
                                ("code", new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Integer)
                                    .Minimum(100)
                                    .Maximum(600)
                                ),
                                ("message", new JsonSchemaBuilder()
                                    .Type(SchemaValueType.String))
                            )
                            .Ref("#/components/schemas/ErrorModel")
                            .Required("message", "code"),
                        ["ExtendedErrorModel"] = new JsonSchemaBuilder()
                            .Ref("#/components/schemas/ExtendedErrorModel")
                            .AllOf(
                                new JsonSchemaBuilder()
                                    .Ref("#/components/schemas/ErrorModel")
                                    .Type(SchemaValueType.Object)
                                    .Properties(
                                        ("code", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.Integer)
                                            .Minimum(100)
                                            .Maximum(600)
                                        ),
                                        ("message", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String))
                                    )
                                    .Required("message", "code"),
                                new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Object)
                                    .Required("rootCause")
                                    .Properties(
                                        ("rootCause", new JsonSchemaBuilder()
                                            .Type(SchemaValueType.String)
                                        )
                                    )
                            )
                    }
                }, options => options.Excluding(m => m.Name == "HostDocument"));
        }

        [Fact]
        public void ParseAdvancedSchemaWithReferenceShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedSchemaWithReference.yaml"));
            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // Assert
            var components = openApiDoc.Components;

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                    {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                    }
                });

            components.Should().BeEquivalentTo(
                new OpenApiComponents
                {
                    Schemas =
                    {
                            ["Pet"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Discriminator("petType", null, null)
                                .Properties(
                                    ("name", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.String)
                                    ),
                                    ("petType", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.String)
                                    )
                                )
                                .Required("name", "petType")
                                .Ref("#/components/schemas/Pet"),
                            ["Cat"] = new JsonSchemaBuilder()
                                .Description("A representation of a cat")
                                .AllOf(
                                    new JsonSchemaBuilder()
                                        .Ref("#/components/schemas/Pet")
                                        .Type(SchemaValueType.Object)
                                        .Discriminator("petType", null, null)
                                        .Properties(
                                            ("name", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.String)
                                            ),
                                            ("petType", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.String)
                                            )
                                        )
                                        .Required("name", "petType"),
                                    new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Object)
                                        .Required("huntingSkill")
                                        .Properties(
                                            ("huntingSkill", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.String)
                                                .Description("The measured skill for hunting")
                                                .Enum("clueless", "lazy", "adventurous", "aggressive")
                                            )
                                        )
                                )
                                .Ref("#/components/schemas/Cat"),
                            ["Dog"] = new JsonSchemaBuilder()
                                .Description("A representation of a dog")
                                .AllOf(
                                    new JsonSchemaBuilder()
                                        .Ref("#/components/schemas/Pet")
                                        .Type(SchemaValueType.Object)
                                        .Discriminator("petType", null, null)
                                        .Properties(
                                            ("name", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.String)
                                            ),
                                            ("petType", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.String)
                                            )
                                        )
                                        .Required("name", "petType"),
                                    new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Object)
                                        .Required("packSize")
                                        .Properties(
                                            ("packSize", new JsonSchemaBuilder()
                                                .Type(SchemaValueType.Integer)
                                                .Format("int32")
                                                .Description("the size of the pack the dog is from")
                                                .Default(0)
                                                .Minimum(0)
                                            )
                                        )
                                )
                                .Ref("#/components/schemas/Dog")
                    }
                }, options => options.Excluding(m => m.Name == "HostDocument"));
        }


        [Fact]
        public void ParseSelfReferencingSchemaShouldNotStackOverflow()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "selfReferencingSchema.yaml"));
            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // Assert
            var components = openApiDoc.Components;

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                { 
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                        {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                        }
                });

            var schemaExtension = new JsonSchemaBuilder()
                .AllOf(
                    new JsonSchemaBuilder()
                        .Title("schemaExtension")
                        .Type(SchemaValueType.Object)
                        .Properties(
                            ("description", new JsonSchemaBuilder().Type(SchemaValueType.String).Nullable(true)),
                            ("targetTypes", new JsonSchemaBuilder()
                                .Type(SchemaValueType.Array)
                                .Items(new JsonSchemaBuilder()
                                    .Type(SchemaValueType.String)
                                )
                            ),
                            ("status", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                            ("owner", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                            ("child", null) // TODO (GSD): this isn't valid
                        )
                );
            //{
            //    AllOf = { new OpenApiSchema()
            //        {
            //            Title = "schemaExtension",
            //            Type = "object",
            //            Properties = {
            //                            ["description"] = new OpenApiSchema() { Type = "string", Nullable = true},
            //                            ["targetTypes"] = new OpenApiSchema() {
            //                                Type = "array",
            //                                Items = new OpenApiSchema() {
            //                                    Type = "string"
            //                                }
            //                            },
            //                            ["status"] = new OpenApiSchema() { Type = "string"},
            //                            ["owner"] = new OpenApiSchema() { Type = "string"},
            //                            ["child"] = null
            //                        }
            //            }
            //        },
            //    Reference = new OpenApiReference()
            //    {
            //        Type = ReferenceType.Schema,
            //        Id = "microsoft.graph.schemaExtension"
            //    }
            //};

            //schemaExtension.AllOf[0].Properties["child"] = schemaExtension;

            components.Schemas["microsoft.graph.schemaExtension"].Should().BeEquivalentTo(components.Schemas["microsoft.graph.schemaExtension"].GetAllOf().ElementAt(0).GetProperties()["child"]);
        }
    }
}
