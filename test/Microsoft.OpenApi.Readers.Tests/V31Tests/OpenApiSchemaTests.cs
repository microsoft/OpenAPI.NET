﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
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
        public async Task ParseBasicV31SchemaShouldSucceed()
        {
            var expectedObject = new OpenApiSchema()
            {
                Id = "https://example.com/arrays.schema.json",
                Schema = "https://json-schema.org/draft/2020-12/schema",
                Description = "A representation of a person, company, organization, or place",
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["fruits"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    ["vegetables"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array
                    }
                },
                Definitions = new Dictionary<string, OpenApiSchema>
                {
                    ["veggie"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "veggieName",
                            "veggieLike"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["veggieName"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "The name of the vegetable."
                            },
                            ["veggieLike"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Boolean,
                                Description = "Do I like this vegetable?"
                            }
                        }
                    }
                }
            };

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(
                System.IO.Path.Combine(SampleFolderPath, "jsonSchema.json"), OpenApiSpecVersion.OpenApi3_1);

            // Assert
            schema.Element.Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        public async Task ParseSchemaWithTypeArrayWorks()
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
                Type = JsonSchemaType.Object | JsonSchemaType.Null
            };

            // Act
            var actual = await OpenApiModelFactory.ParseAsync<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual.Element.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestSchemaCopyConstructorWithTypeArrayWorks()
        {
            /* Arrange
            *  Test schema's copy constructor for deep-cloning type array
            */
            var schemaWithTypeArray = new OpenApiSchema()
            {
                Type = JsonSchemaType.Array | JsonSchemaType.Null,
                Items = new OpenApiSchema
                {
                    Type = JsonSchemaType.String
                }
            };

            var simpleSchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String
            };

            // Act
            var schemaWithArrayCopy = new OpenApiSchema(schemaWithTypeArray);
            schemaWithArrayCopy.Type = JsonSchemaType.String;

            var simpleSchemaCopy = new OpenApiSchema(simpleSchema)
            {
                Type = JsonSchemaType.String | JsonSchemaType.Null
            };

            // Assert
            schemaWithArrayCopy.Type.Should().NotBe(schemaWithTypeArray.Type);
            schemaWithTypeArray.Type = JsonSchemaType.String | JsonSchemaType.Null;

            simpleSchemaCopy.Type.Should().NotBe(simpleSchema.Type);
            simpleSchema.Type = JsonSchemaType.String;
        }

        [Fact]
        public async Task ParseV31SchemaShouldSucceed()
        {
            var path = Path.Combine(SampleFolderPath, "schema.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);
            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["one"] = new()
                    {
                        Description = "type array",
                        Type = JsonSchemaType.Integer | JsonSchemaType.String
                    }
                }
            };

            // Assert
            schema.Element.Should().BeEquivalentTo(expectedSchema);
        }

        [Fact]
        public async Task ParseAdvancedV31SchemaShouldSucceed()
        {
            // Arrange and Act
            var path = Path.Combine(SampleFolderPath, "advancedSchema.yaml");
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);

            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["one"] = new()
                    {
                        Description = "type array",
                        Type = JsonSchemaType.Integer | JsonSchemaType.String
                    },
                    ["two"] = new()
                    {
                        Description = "type 'null'",
                        Type = JsonSchemaType.Null
                    },
                    ["three"] = new()
                    {
                        Description = "type array including 'null'",
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["four"] = new()
                    {
                        Description = "array with no items",
                        Type = JsonSchemaType.Array
                    },
                    ["five"] = new()
                    {
                        Description = "singular example",
                        Type = JsonSchemaType.String,
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
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["eleven"] = new()
                    {
                        Description = "x-nullable string",
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["twelve"] = new()
                    {
                        Description = "file/binary"
                    }
                }
            };

            // Assert
            schema.Element.Should().BeEquivalentTo(expectedSchema, options => options
                    .IgnoringCyclicReferences()
                    .Excluding((IMemberInfo memberInfo) =>
                            memberInfo.Path.EndsWith("Parent")));
        }

        [Fact]
        public async Task ParseSchemaWithExamplesShouldSucceed()
        {
            // Arrange
            var input = @"
type: string
examples: 
 - fedora
 - ubuntu
";
            // Act
            var result = await OpenApiModelFactory.ParseAsync<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1);

            // Assert
            result.Element.Examples.Should().HaveCount(2);
        }

        [Fact]
        public void CloningSchemaWithExamplesAndEnumsShouldSucceed()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
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
        public async Task SerializeV31SchemaWithMultipleTypesAsV3Works()
        {
            // Arrange
            var expected = @"type: string
nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);

            var writer = new StringWriter();
            schema.Element.SerializeAsV3(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            schema1String.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV31SchemaWithMultipleTypesAsV2Works()
        {
            // Arrange
            var expected = @"type: string
x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);

            var writer = new StringWriter();
            schema.Element.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            schema1String.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV3SchemaWithNullableAsV31Works()
        {
            // Arrange
            var expected = @"type:
  - 'null'
  - string";

            var path = Path.Combine(SampleFolderPath, "schemaWithNullable.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_0);

            var writer = new StringWriter();
            schema.Element.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV2SchemaWithNullableExtensionAsV31Works()
        {
            // Arrange
            var expected = @"type:
  - 'null'
  - string
x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0);

            var writer = new StringWriter();
            schema.Element.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeSchemaWithTypeArrayAndNullableDoesntEmitType()
        {
            var input = @"type:
- ""string""
- ""int""
nullable: true";

            var expected = @"{ }";

            var schema = await OpenApiModelFactory.ParseAsync<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1);

            var writer = new StringWriter();
            schema.Element.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral()); 
        }

        [Theory]
        [InlineData("schemaWithNullable.yaml")]
        [InlineData("schemaWithNullableExtension.yaml")]
        public async Task LoadSchemaWithNullableExtensionAsV31Works(string filePath)
        {
            // Arrange
            var path = Path.Combine(SampleFolderPath, filePath);

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);

            // Assert
            schema.Element.Type.Should().Be(JsonSchemaType.String | JsonSchemaType.Null);
        }

        [Fact]
        public async Task SerializeSchemaWithJsonSchemaKeywordsWorks()
        {
            // Arrange
            var expected = @"$id: https://example.com/schemas/person.schema.yaml
$schema: https://json-schema.org/draft/2020-12/schema
$comment: A schema defining a person object with optional references to dynamic components.
$vocabulary:
  https://json-schema.org/draft/2020-12/vocab/core: true
  https://json-schema.org/draft/2020-12/vocab/applicator: true
  https://json-schema.org/draft/2020-12/vocab/validation: true
  https://json-schema.org/draft/2020-12/vocab/meta-data: false
  https://json-schema.org/draft/2020-12/vocab/format-annotation: false
$dynamicAnchor: addressDef
title: Person
required:
  - name
type: object
properties:
  name:
    $comment: The person's full name
    type: string
  age:
    $comment: Age must be a non-negative integer
    minimum: 0
    type: integer
  address:
    $comment: Reference to an address definition which can change dynamically
    $dynamicRef: '#addressDef'
description: Schema for a person object
";
            var path = Path.Combine(SampleFolderPath, "schemaWithJsonSchemaKeywords.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1);

            // serialization
            var writer = new StringWriter();
            schema.Element.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            // Assert
            schema.Element.Vocabulary.Keys.Count.Should().Be(5);
            schemaString.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
