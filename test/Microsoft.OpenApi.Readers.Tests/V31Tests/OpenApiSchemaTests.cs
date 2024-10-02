// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiSchema/";

        public OpenApiSchemaTests()
        {
           OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

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
                System.IO.Path.Combine(SampleFolderPath, "jsonSchema.json"), OpenApiSpecVersion.OpenApi3_1, out _);

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
            var path = System.IO.Path.Combine(SampleFolderPath, "schema.yaml");

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, out _);
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
            schema.Should().BeEquivalentTo(expectedSchema);
        }

        [Fact]
        public void ParseAdvancedV31SchemaShouldSucceed()
        {
            // Arrange and Act
            var path = System.IO.Path.Combine(SampleFolderPath, "advancedSchema.yaml");
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, out _);

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
                            "exampleValue"
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
            schema.Should().BeEquivalentTo(expectedSchema, options => options
                    .IgnoringCyclicReferences()
                    .Excluding((IMemberInfo memberInfo) =>
                            memberInfo.Path.EndsWith("Parent")));
        }

        [Fact]
        public void ParseSchemaWithExamplesShouldSucceed()
        {
            // Arrange
            var input = @"
type: string
examples: 
 - fedora
 - ubuntu
";
            // Act
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, out _, "yaml");

            // Assert
            schema.Examples.Should().HaveCount(2);
        }

        [Fact]
        public void CloningSchemaWithExamplesAndEnumsShouldSucceed()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = "int",
                Default = 5,
                Examples = [2, 3],
                Enum = [1, 2, 3]
            };

            var clone = new OpenApiSchema(schema);
            clone.Examples.Add(4);
            clone.Enum.Add(4);
            clone.Default = 6;

            // Assert
            clone.Enum.Should().NotBeEquivalentTo(schema.Enum);
            clone.Examples.Should().NotBeEquivalentTo(schema.Examples);
            clone.Default.Should().NotBeEquivalentTo(schema.Default);
        }

        [Fact]
        public void SerializeV31SchemaWithMultipleTypesAsV3Works()
        {
            // Arrange
            var expected = @"type: string
nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, out _);

            var writer = new StringWriter();
            schema.SerializeAsV3(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            schema1String.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeV31SchemaWithMultipleTypesAsV2Works()
        {
            // Arrange
            var expected = @"type: string
x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, out _);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            schema1String.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeV3SchemaWithNullableAsV31Works()
        {
            // Arrange
            var expected = @"type:
  - string
  - null";

            var path = Path.Combine(SampleFolderPath, "schemaWithNullable.yaml");

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_0, out _);

            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeV2SchemaWithNullableExtensionAsV31Works()
        {
            // Arrange
            var expected = @"type:
  - string
  - null
x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithNullableExtension.yaml");

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, out _);

            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeSchemaWithTypeArrayAndNullableDoesntEmitType()
        {
            var input = @"type:
- ""string""
- ""int""
nullable: true";

            var expected = @"{ }";

            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, out _, "yaml");

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral()); 
        }

        [Theory]
        [InlineData("schemaWithNullable.yaml")]
        [InlineData("schemaWithNullableExtension.yaml")]
        public void LoadSchemaWithNullableExtensionAsV31Works(string filePath)
        {
            // Arrange
            var path = Path.Combine(SampleFolderPath, filePath);

            // Act
            var schema = OpenApiModelFactory.Load<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, out _);

            // Assert
            schema.Type.Should().BeEquivalentTo(new string[] { "string", "null" });
        }
    }
}
