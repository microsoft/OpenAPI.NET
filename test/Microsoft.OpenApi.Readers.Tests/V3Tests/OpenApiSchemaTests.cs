// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
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

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);
                
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
                    Default = new OpenApiAny(88)
                }, options => options.IgnoringCyclicReferences()
                .Excluding(s => s.Default.Node.Parent));
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
            var openApiAny = reader.ReadFragment<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);
            
            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            openApiAny.Should().BeEquivalentTo(new OpenApiAny(
                new JsonObject
                {
                    ["foo"] = "bar",
                    ["baz"] = new JsonArray() {1, 2}
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
            var reader = new OpenApiStringReader();
            var diagnostic = new OpenApiDiagnostic();
            
            // Act
            var openApiAny = reader.ReadFragment<OpenApiAny>(input, OpenApiSpecVersion.OpenApi3_0, out diagnostic);

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
        public void ParseSimpleSchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "simpleSchema.yaml")))
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

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);
                
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

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);
                
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
                        Example = new OpenApiAny(new JsonObject { ["name"] = "Puma", ["id"] = 1 })
                    },
                    options => options.IgnoringCyclicReferences()
                    .Excluding(s => s.Example.Node["name"].Parent)
                    .Excluding(s => s.Example.Node["name"].Root)
                    .Excluding(s => s.Example.Node["id"].Parent)
                    .Excluding(s => s.Example.Node["id"].Root));
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
                            ["ErrorModel"] = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["code"] = new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Minimum = 100,
                                        Maximum = 600
                                    },
                                    ["message"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                },
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "ErrorModel",
                                    HostDocument = openApiDoc
                                },
                                Required =
                                {
                                    "message",
                                    "code"
                                }
                            },
                            ["ExtendedErrorModel"] = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "ExtendedErrorModel",
                                    HostDocument = openApiDoc
                                },
                                AllOf =
                                {
                                    new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "ErrorModel",
                                            HostDocument = openApiDoc
                                        },
                                        // Schema should be dereferenced in our model, so all the properties
                                        // from the ErrorModel above should be propagated here.
                                        Type = "object",
                                        Properties =
                                        {
                                            ["code"] = new OpenApiSchema
                                            {
                                                Type = "integer",
                                                Minimum = 100,
                                                Maximum = 600
                                            },
                                            ["message"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        },
                                        Required =
                                        {
                                            "message",
                                            "code"
                                        }
                                    },
                                    new OpenApiSchema
                                    {
                                        Type = "object",
                                        Required = {"rootCause"},
                                        Properties =
                                        {
                                            ["rootCause"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }, options => options.Excluding(m => m.Name == "HostDocument")
                                     .IgnoringCyclicReferences());
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
                            ["Pet"] = new OpenApiSchema
                            {
                                Type = "object",
                                Discriminator = new OpenApiDiscriminator
                                {
                                    PropertyName = "petType"
                                },
                                Properties =
                                {
                                    ["name"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    },
                                    ["petType"] = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                },
                                Required =
                                {
                                    "name",
                                    "petType"
                                },
                                Reference = new OpenApiReference()
                                {
                                    Id= "Pet",
                                    Type = ReferenceType.Schema,
                                    HostDocument = openApiDoc
                                }
                            },
                            ["Cat"] = new OpenApiSchema
                            {
                                Description = "A representation of a cat",
                                AllOf =
                                {
                                    new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "Pet",
                                            HostDocument = openApiDoc
                                        },
                                        // Schema should be dereferenced in our model, so all the properties
                                        // from the Pet above should be propagated here.
                                        Type = "object",
                                        Discriminator = new OpenApiDiscriminator
                                        {
                                            PropertyName = "petType"
                                        },
                                        Properties =
                                        {
                                            ["name"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            },
                                            ["petType"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        },
                                        Required =
                                        {
                                            "name",
                                            "petType"
                                        }
                                    },
                                    new OpenApiSchema
                                    {
                                        Type = "object",
                                        Required = {"huntingSkill"},
                                        Properties =
                                        {
                                            ["huntingSkill"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "The measured skill for hunting",
                                                Enum = 
                                                { 
                                                    new OpenApiAny("clueless"),
                                                    new OpenApiAny("lazy"),
                                                    new OpenApiAny("adventurous"),
                                                    new OpenApiAny("aggressive")
                                                }
                                            }
                                        }
                                    }
                                },
                                Reference = new OpenApiReference()
                                {
                                    Id= "Cat",
                                    Type = ReferenceType.Schema,
                                    HostDocument = openApiDoc
                                }
                            },
                            ["Dog"] = new OpenApiSchema
                            {
                                Description = "A representation of a dog",
                                AllOf =
                                {
                                    new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "Pet",
                                            HostDocument = openApiDoc
                                        },
                                        // Schema should be dereferenced in our model, so all the properties
                                        // from the Pet above should be propagated here.
                                        Type = "object",
                                        Discriminator = new OpenApiDiscriminator
                                        {
                                            PropertyName = "petType"
                                        },
                                        Properties =
                                        {
                                            ["name"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            },
                                            ["petType"] = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        },
                                        Required =
                                        {
                                            "name",
                                            "petType"
                                        }
                                    },
                                    new OpenApiSchema
                                    {
                                        Type = "object",
                                        Required = {"packSize"},
                                        Properties =
                                        {
                                            ["packSize"] = new OpenApiSchema
                                            {
                                                Type = "integer",
                                                Format = "int32",
                                                Description = "the size of the pack the dog is from",
                                                Default = new OpenApiAny(0),
                                                Minimum = 0
                                            }
                                        }
                                    }
                                },
                                Reference = new OpenApiReference()
                                {
                                    Id= "Dog",
                                    Type = ReferenceType.Schema,
                                    HostDocument = openApiDoc
                                }
                            }
                    }
                }, options => options.Excluding(m => m.Name == "HostDocument").IgnoringCyclicReferences()
                .Excluding(c => c.Schemas["Cat"].AllOf[1].Properties["huntingSkill"].Enum[0].Node.Parent)
                .Excluding(c => c.Schemas["Cat"].AllOf[1].Properties["huntingSkill"].Enum[1].Node.Parent)
                .Excluding(c => c.Schemas["Cat"].AllOf[1].Properties["huntingSkill"].Enum[2].Node.Parent)
                .Excluding(c => c.Schemas["Cat"].AllOf[1].Properties["huntingSkill"].Enum[3].Node.Parent)
                .Excluding(c => c.Schemas["Dog"].AllOf[1].Properties["packSize"].Default.Node.Parent));
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

            var schemaExtension = new OpenApiSchema()
            {
                AllOf = { new OpenApiSchema()
                    {
                        Title = "schemaExtension",
                        Type = "object",
                        Properties = {
                                        ["description"] = new OpenApiSchema() { Type = "string", Nullable = true},
                                        ["targetTypes"] = new OpenApiSchema() {
                                            Type = "array",
                                            Items = new OpenApiSchema() {
                                                Type = "string"
                                            }
                                        },
                                        ["status"] = new OpenApiSchema() { Type = "string"},
                                        ["owner"] = new OpenApiSchema() { Type = "string"},
                                        ["child"] = null
                                    }
                        }
                    },
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.Schema,
                    Id = "microsoft.graph.schemaExtension"
                }
            };

            schemaExtension.AllOf[0].Properties["child"] = schemaExtension;

            components.Schemas["microsoft.graph.schemaExtension"].Should().BeEquivalentTo(components.Schemas["microsoft.graph.schemaExtension"].AllOf[0].Properties["child"]);
        }
    }
}
