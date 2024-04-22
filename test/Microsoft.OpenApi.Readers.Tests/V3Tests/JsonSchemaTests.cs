﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Extensions;
using SharpYaml.Serialization;
using Xunit;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V3;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class JsonSchemaTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSchema/";

        public JsonSchemaTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParsePrimitiveSchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "primitiveSchema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV3Deserializer.LoadSchema(node);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            schema.Should().BeEquivalentTo(
                new JsonSchemaBuilder()
                    .Type(SchemaValueType.String)
                    .Format("email")
                    .Build());
        }       

        [Fact]
        public void ParseExampleStringFragmentShouldSucceed()
        {
            var input = @"
{ 
  ""foo"": ""bar"",
  ""baz"": [ 1,2]
}";
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(new OpenApiAny(
                new JsonObject
                {
                    ["foo"] = "bar",
                    ["baz"] = new JsonArray() { 1, 2 }
                }), options => options.IgnoringCyclicReferences());
        }

        [Fact]
        public void ParseEnumFragmentShouldSucceed()
        {
            var input = @"
[ 
  ""foo"",
  ""baz""
]";
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(new OpenApiAny(
                new JsonArray
                {
                    "foo",
                    "baz"
                }), options => options.IgnoringCyclicReferences());
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
            var diagnostic = new OpenApiDiagnostic();

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiPathItem>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic, "yaml");

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

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Object)
                        .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.String))
                        .Build());
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

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node);

                // Assert
                diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                schema.Should().BeEquivalentTo(
                    new JsonSchemaBuilder()
                    .Type(SchemaValueType.Object)
                    .Properties(
                        ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                        ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                    .Required("name")
                    .Example(new JsonObject { ["name"] = "Puma", ["id"] = 1 })
                    .Build(),
                    options => options.IgnoringCyclicReferences());
            }
        }

        [Fact]
        public void ParseBasicSchemaWithReferenceShouldSucceed()
        {
            // Act
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "basicSchemaWithReference.yaml"));

            // Assert
            var components = result.OpenApiDocument.Components;

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                    {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                    }
                });

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                    {
                            ["ErrorModel"] = new JsonSchemaBuilder()
                                .Ref("#/components/schemas/ErrorModel")
                                .Type(SchemaValueType.Object)
                                .Required("message", "code")
                                .Properties(
                                    ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                    ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Minimum(100).Maximum(600))),
                            ["ExtendedErrorModel"] = new JsonSchemaBuilder()
                                .Ref("#/components/schemas/ExtendedErrorModel")
                                .AllOf(
                                    new JsonSchemaBuilder()
                                        .Ref("#/components/schemas/ErrorModel"),
                                    new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Object)
                                        .Required("rootCause")
                                        .Properties(("rootCause", new JsonSchemaBuilder().Type(SchemaValueType.String))))
                    }
            };

            components.Should().BeEquivalentTo(expectedComponents);
        }

        [Fact]
        public void ParseAdvancedSchemaWithReferenceShouldSucceed()
        {
            // Act
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "advancedSchemaWithReference.yaml"));

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                {
                    ["Pet1"] = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Object)
                        .Discriminator(new OpenApiDiscriminator { PropertyName = "petType" })
                        .Properties(
                            ("name", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                            ),
                            ("petType", new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                            )
                        )
                        .Required("name", "petType"),
                    ["Cat"] = new JsonSchemaBuilder()
                        .Description("A representation of a cat")
                        .AllOf(
                            new JsonSchemaBuilder()
                                .Ref("#/components/schemas/Pet1")
                                .Type(SchemaValueType.Object)
                                .Discriminator(new OpenApiDiscriminator { PropertyName = "petType" })
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
                        ),
                    ["Dog"] = new JsonSchemaBuilder()
                        .Description("A representation of a dog")
                        .AllOf(
                            new JsonSchemaBuilder()
                                .Ref("#/components/schemas/Pet1")
                                .Type(SchemaValueType.Object)
                                .Discriminator(new OpenApiDiscriminator { PropertyName = "petType" })
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
                }
            };

            // We serialize so that we can get rid of the schema BaseUri properties which show up as diffs
            var actual = result.OpenApiDocument.Components.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
            var expected = expectedComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
