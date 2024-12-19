// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Extensions;
using SharpYaml.Serialization;
using Xunit;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V3;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Models.References;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSchema/";

        public OpenApiSchemaTests()
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
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "email"
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
                new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    AdditionalProperties = new()
                    {
                        Type = JsonSchemaType.String
                    }
                });
            }
        }

        [Fact]
        public void ParseBasicSchemaWithExampleShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicSchemaWithExample.yaml"));
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
            new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties =
                {
                        ["id"] = new()
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int64"
                        },
                        ["name"] = new()
                        {
                            Type = JsonSchemaType.String
                        }
                },
                Required =
                {
                        "name"
                },
                Example = new JsonObject
                {
                    ["name"] = new OpenApiAny("Puma").Node,
                    ["id"] = new OpenApiAny(1).Node
                }
            }, options => options
            .IgnoringCyclicReferences()
            .Excluding((IMemberInfo memberInfo) =>
                memberInfo.Path.EndsWith("Parent"))
            .Excluding((IMemberInfo memberInfo) =>
                memberInfo.Path.EndsWith("Root")));
        }

        [Fact]
        public void ParseBasicSchemaWithReferenceShouldSucceed()
        {
            // Act
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "basicSchemaWithReference.yaml"));

            // Assert
            var components = result.Document.Components;

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                {
                    ["ErrorModel"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Properties =
                        {
                            ["code"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Minimum = 100,
                                Maximum = 600
                            },
                            ["message"] = new()
                            {
                                Type = JsonSchemaType.String
                            }
                        },
                        Required =
                        {
                            "message",
                            "code"
                        }
                    },
                    ["ExtendedErrorModel"] = new()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ErrorModel", result.Document),
                            new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Required = {"rootCause"},
                                Properties =
                                {
                                    ["rootCause"] = new()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    }
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
                    ["Pet"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Discriminator = new()
                        {
                            PropertyName = "petType"
                        },
                        Properties =
                        {
                            ["name"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["petType"] = new()
                            {
                                Type = JsonSchemaType.String
                            }
                        },
                        Required =
                        {
                            "name",
                            "petType"
                        }
                    },
                    ["Cat"] = new()
                    {
                        Description = "A representation of a cat",
                        AllOf =
                        {
                            new OpenApiSchemaReference("Pet", result.Document),
                            new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Required = {"huntingSkill"},
                                Properties =
                                {
                                    ["huntingSkill"] = new()
                                    {
                                        Type = JsonSchemaType.String,
                                        Description = "The measured skill for hunting",
                                        Enum =
                                        {
                                            "clueless",
                                            "lazy",
                                            "adventurous",
                                            "aggressive"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["Dog"] = new()
                    {
                        Description = "A representation of a dog",
                        AllOf =
                        {
                            new OpenApiSchemaReference("Pet", result.Document),
                            new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Required = {"packSize"},
                                Properties =
                                {
                                    ["packSize"] = new()
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Format = "int32",
                                        Description = "the size of the pack the dog is from",
                                        Default = 0,
                                        Minimum = 0
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // We serialize so that we can get rid of the schema BaseUri properties which show up as diffs
            var actual = result.Document.Components.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
            var expected = expectedComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
