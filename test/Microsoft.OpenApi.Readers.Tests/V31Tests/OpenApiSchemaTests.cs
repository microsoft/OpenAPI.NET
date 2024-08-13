// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V31;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiSchema/";

        [Fact]
        public void ParseBasicV31SchemaShouldSucceed()
        {
            var expectedObject = new OpenApiSchema()
            {
                Id = "https://example.com/arrays.schema.json",
                Schema = "https://json-schema.org/draft/2020-12/schema",
                Description = "A representation of a person, company, organization, or place",
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["fruits"] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string"
                        }
                    },
                    ["vegetables"] = new OpenApiSchema
                    {
                        Type = "array"
                    }
                },
                Definitions = new Dictionary<string, OpenApiSchema>
                {
                    ["veggie"] = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string>
                        {
                            "veggieName",
                            "veggieLike"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["veggieName"] = new OpenApiSchema
                            {
                                Type = "string",
                                Description = "The name of the vegetable."
                            },
                            ["veggieLike"] = new OpenApiSchema
                            {
                                Type = "boolean",
                                Description = "Do I like this vegetable?"
                            }
                        }
                    }
                }
            };

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(
                Path.Combine(SampleFolderPath, "jsonSchema.json"), OpenApiSpecVersion.OpenApi3_1, out _);

            // Assert
            schema.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public void ParseSchemaWithTypeArrayWorks()
        {
            // Arrange
            var schema = @"{
  ""$id"": ""https://example.com/arrays.schema.json"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""description"": ""A representation of a person, company, organization, or place"",
  ""type"": [""object"", ""null""]
}";

            var expected = new OpenApiSchema()
            {
                Id = "https://example.com/arrays.schema.json",
                Schema = "https://json-schema.org/draft/2020-12/schema",
                Description = "A representation of a person, company, organization, or place",
                Type = new string[] { "object", "null" }
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, out _);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSchemaCopyConstructorWithTypeArrayWorks()
        {
            /* Arrange
            *  Test schema's copy constructor for deep-cloning type array
            */
            var schemaWithTypeArray = new OpenApiSchema()
            {
                Type = new string[] { "array", "null" },
                Items = new OpenApiSchema
                {
                    Type = "string"
                }
            };

            var simpleSchema = new OpenApiSchema()
            {
                Type = "string"
            };

            // Act
            var schemaWithArrayCopy = new OpenApiSchema(schemaWithTypeArray);
            schemaWithArrayCopy.Type = "string";

            var simpleSchemaCopy = new OpenApiSchema(simpleSchema);
            simpleSchemaCopy.Type = new string[] { "string", "null" };

            // Assert
            schemaWithArrayCopy.Type.Should().NotBeEquivalentTo(schemaWithTypeArray.Type);
            schemaWithTypeArray.Type = new string[] { "string", "null" };

            simpleSchemaCopy.Type.Should().NotBeEquivalentTo(simpleSchema.Type);
            simpleSchema.Type = "string";
        }

        [Fact]
        public void ParseV31SchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV31Deserializer.LoadSchema(node);
            var expectedSchema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["one"] = new()
                    {
                        Description = "type array",
                        Type = new HashSet<string> { "integer", "string" }
                    }
                }
            };

            // Assert
            Assert.Equal(schema, expectedSchema);
        }

        [Fact]
        public void ParseAdvancedV31SchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedSchema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV31Deserializer.LoadSchema(node);

            var expectedSchema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["one"] = new()
                    {
                        Description = "type array",
                        Type = new HashSet<string> { "integer", "string" }
                    },
                    ["two"] = new()
                    {
                        Description = "type 'null'",
                        Type = "null"
                    },
                    ["three"] = new()
                    {
                        Description = "type array including 'null'",
                        Type = new HashSet<string> { "string", "null" }
                    },
                    ["four"] = new()
                    {
                        Description = "array with no items",
                        Type = "array"
                    },
                    ["five"] = new()
                    {
                        Description = "singular example",
                        Type = "string",
                        Examples = new List<JsonNode>
                        {
                            new OpenApiAny("exampleValue").Node
                        }
                    },
                    ["six"] = new()
                    {
                        Description = "exclusiveMinimum true",
                        V31ExclusiveMinimum = 10
                    },
                    ["seven"] = new()
                    {
                        Description = "exclusiveMinimum false",
                        Minimum = 10
                    },
                    ["eight"] = new()
                    {
                        Description = "exclusiveMaximum true",
                        V31ExclusiveMaximum = 20
                    },
                    ["nine"] = new()
                    {
                        Description = "exclusiveMaximum false",
                        Maximum = 20
                    },
                    ["ten"] = new()
                    {
                        Description = "nullable string",
                        Type = new HashSet<string> { "string", "null" }
                    },
                    ["eleven"] = new()
                    {
                        Description = "x-nullable string",
                        Type = new HashSet<string> { "string", "null" }
                    },
                    ["twelve"] = new()
                    {
                        Description = "file/binary"
                    }
                }
            };

            // Assert
            schema.Should().BeEquivalentTo(expectedSchema);
        }
    }
}
