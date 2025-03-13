﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
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
using System.Threading.Tasks;
using System.Net.Http;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSchema/";

        [Fact]
        public void ParsePrimitiveSchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "primitiveSchema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV3Deserializer.LoadSchema(node, new());

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

            Assert.Equivalent(
                new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "email"
                }, schema);
        }       

        [Fact]
        public void ParseExampleStringFragmentShouldSucceed()
        {
            var input = @"
{ 
  ""foo"": ""bar"",
  ""baz"": [ 1,2]
}";

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, new(), out var diagnostic, settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

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

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, new(), out var diagnostic, settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

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

            // Act
            var openApiAny = OpenApiModelFactory.Parse<OpenApiPathItem>(input, OpenApiSpecVersion.OpenApi3_0, new(), out var diagnostic, "yaml", SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

            Assert.Equivalent(
                new OpenApiPathItem
                {
                    Summary = "externally referenced path item",
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new OpenApiOperation()
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
                }, openApiAny);
        }

        [Fact]
        public void ParseDictionarySchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "dictionarySchema.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents[0].RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

                // Act
                var schema = OpenApiV3Deserializer.LoadSchema(node, new());

                // Assert
                Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

                Assert.Equivalent(
                new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    AdditionalProperties = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }, schema);
            }
        }

        [Fact]
        public void ParseBasicSchemaWithExampleShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicSchemaWithExample.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV3Deserializer.LoadSchema(node, new());

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

            schema.Should().BeEquivalentTo(
            new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties =
                {
                        ["id"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int64"
                        },
                        ["name"] = new OpenApiSchema()
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
        public async Task ParseBasicSchemaWithReferenceShouldSucceed()
        {
            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "basicSchemaWithReference.yaml"), SettingsFixture.ReaderSettings);

            // Assert
            var components = result.Document.Components;

            Assert.Equivalent(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                {
                    ["ErrorModel"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Properties =
                        {
                            ["code"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Minimum = 100,
                                Maximum = 600
                            },
                            ["message"] = new OpenApiSchema()
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
                    ["ExtendedErrorModel"] = new OpenApiSchema()
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
                                    ["rootCause"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Assert.Equivalent(expectedComponents, components);
        }

        [Fact]
        public async Task ParseAdvancedSchemaWithReferenceShouldSucceed()
        {
            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "advancedSchemaWithReference.yaml"), SettingsFixture.ReaderSettings);

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                {
                    ["Pet"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Discriminator = new()
                        {
                            PropertyName = "petType"
                        },
                        Properties =
                        {
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["petType"] = new OpenApiSchema()
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
                    ["Cat"] = new OpenApiSchema()
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
                                    ["huntingSkill"] = new OpenApiSchema()
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
                    ["Dog"] = new OpenApiSchema()
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
                                    ["packSize"] = new OpenApiSchema()
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
            var actual = await result.Document.Components.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);
            var expected = await expectedComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ParseExternalReferenceSchemaShouldSucceed()
        {
            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalReferencesSchema.yaml"), SettingsFixture.ReaderSettings);

            // Assert
            var components = result.Document.Components;

            Assert.Equivalent(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);

            var expectedComponents = new OpenApiComponents
            {
                Schemas =
                {
                    ["RelativePathModel"] = new OpenApiSchema()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ExternalRelativePathModel", result.Document, "./FirstLevel/SecondLevel/ThridLevel/File.json")
                        }
                    },
                    ["SimpleRelativePathModel"] = new OpenApiSchema()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ExternalSimpleRelativePathModel", result.Document, "File.json")
                        }
                    },
                    ["AbsoluteWindowsPathModel"] = new OpenApiSchema()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ExternalAbsWindowsPathModel", result.Document, @"A:\Dir\File.json")
                        }
                    },
                    ["AbsoluteUnixPathModel"] = new OpenApiSchema()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ExternalAbsUnixPathModel", result.Document, "/Dir/File.json")
                        }
                    },
                    ["HttpsUrlModel"] = new OpenApiSchema()
                    {
                        AllOf =
                        {
                            new OpenApiSchemaReference("ExternalHttpsModel", result.Document, "https://host.lan:1234/path/to/file/resource.json")
                        }
                    }
                }
            };

            Assert.Equivalent(expectedComponents, components);
        }
    }
}
