// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiSchema/";


        public static MemoryStream GetMemoryStream(string fileName)
        {
            var filePath = Path.Combine(SampleFolderPath, fileName);
            var fileBytes = File.ReadAllBytes(filePath);
            return new MemoryStream(fileBytes);
        }

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
                Properties = new Dictionary<string, IOpenApiSchema>
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
                Definitions = new Dictionary<string, IOpenApiSchema>
                {
                    ["veggie"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "veggieName",
                            "veggieLike"
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>
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
                Path.Combine(SampleFolderPath, "jsonSchema.json"), OpenApiSpecVersion.OpenApi3_1, new());

            // Assert
            Assert.Equivalent(expectedObject, schema);
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
                Type = JsonSchemaType.Object | JsonSchemaType.Null
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
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
            var schemaWithArrayCopy = schemaWithTypeArray.CreateShallowCopy() as OpenApiSchema;
            schemaWithArrayCopy.Type = JsonSchemaType.String;

            var simpleSchemaCopy = simpleSchema.CreateShallowCopy() as OpenApiSchema;
            simpleSchemaCopy.Type = JsonSchemaType.String | JsonSchemaType.Null;

            // Assert
            Assert.NotEqual(schemaWithTypeArray.Type, schemaWithArrayCopy.Type);
            schemaWithTypeArray.Type = JsonSchemaType.String | JsonSchemaType.Null;

            Assert.NotEqual(simpleSchema.Type, simpleSchemaCopy.Type);
            simpleSchema.Type = JsonSchemaType.String;
        }

        [Fact]
        public async Task ParseV31SchemaShouldSucceed()
        {
            var path = Path.Combine(SampleFolderPath, "schema.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());
            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["one"] = new OpenApiSchema()
                    {
                        Description = "type array",
                        Type = JsonSchemaType.Integer | JsonSchemaType.String
                    }
                }
            };

            // Assert
            Assert.Equivalent(expectedSchema, schema);
        }

        [Fact]
        public async Task ParseAdvancedV31SchemaShouldSucceed()
        {
            // Arrange and Act
            var path = Path.Combine(SampleFolderPath, "advancedSchema.yaml");
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());

            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["one"] = new OpenApiSchema()
                    {
                        Description = "type array",
                        Type = JsonSchemaType.Integer | JsonSchemaType.String
                    },
                    ["two"] = new OpenApiSchema()
                    {
                        Description = "type 'null'",
                        Type = JsonSchemaType.Null
                    },
                    ["three"] = new OpenApiSchema()
                    {
                        Description = "type array including 'null'",
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["four"] = new OpenApiSchema()
                    {
                        Description = "array with no items",
                        Type = JsonSchemaType.Array
                    },
                    ["five"] = new OpenApiSchema()
                    {
                        Description = "singular example",
                        Type = JsonSchemaType.String,
                        Examples = new List<JsonNode>
                        {
                            "exampleValue"
                        }
                    },
                    ["six"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMinimum true",
                        V31ExclusiveMinimum = 10
                    },
                    ["seven"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMinimum false",
                        Minimum = 10
                    },
                    ["eight"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMaximum true",
                        V31ExclusiveMaximum = 20
                    },
                    ["nine"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMaximum false",
                        Maximum = 20
                    },
                    ["ten"] = new OpenApiSchema()
                    {
                        Description = "nullable string",
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["eleven"] = new OpenApiSchema()
                    {
                        Description = "x-nullable string",
                        Type = JsonSchemaType.String | JsonSchemaType.Null
                    },
                    ["twelve"] = new OpenApiSchema()
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
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, new(), out _, "yaml");

            // Assert
            Assert.Equal(2, schema.Examples.Count);
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

            var clone = schema.CreateShallowCopy() as OpenApiSchema;
            clone.Examples.Add(4);
            clone.Enum.Add(4);
            clone.Default = 6;

            // Assert
            Assert.Equivalent(new int[] {1, 2, 3, 4}, clone.Enum.Select(static x => x.GetValue<int>()).ToArray());
            Assert.Equivalent(new int[] {2, 3, 4}, clone.Examples.Select(static x => x.GetValue<int>()).ToArray());
            Assert.Equivalent(6, clone.Default.GetValue<int>());
        }

        [Fact]
        public async Task SerializeV31SchemaWithMultipleTypesAsV3Works()
        {
            // Arrange
            var expected = @"type: string
nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());

            var writer = new StringWriter();
            schema.SerializeAsV3(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schema1String.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV31SchemaWithMultipleTypesAsV2Works()
        {
            // Arrange
            var expected = @"type: string
x-nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schema1String = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schema1String.MakeLineBreaksEnvironmentNeutral());
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_0, new());

            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new());

            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeSchemaWithTypeArrayAndNullableDoesntEmitType()
        {
            var input = @"type:
- ""string""
- ""int""
nullable: true";

            var expected = @"{ }";

            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, new(), out _, "yaml");

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral()); 
        }

        [Theory]
        [InlineData("schemaWithNullable.yaml")]
        [InlineData("schemaWithNullableExtension.yaml")]
        public async Task LoadSchemaWithNullableExtensionAsV31Works(string filePath)
        {
            // Arrange
            var path = Path.Combine(SampleFolderPath, filePath);

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());

            // Assert
            Assert.Equal(JsonSchemaType.String | JsonSchemaType.Null, schema.Type);
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());

            // serialization
            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            // Assert
            Assert.Equal(5, schema.Vocabulary.Keys.Count);
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task ParseSchemaWithConstWorks()
        {
            var expected = @"{
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""required"": [
    ""status""
  ],
  ""type"": ""object"",
  ""properties"": {
    ""status"": {
      ""const"": ""active"",
      ""type"": ""string""
    },
    ""user"": {
      ""required"": [
        ""role""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""role"": {
          ""const"": ""admin"",
          ""type"": ""string""
        }
      }
    }
  }
}";

            var path = Path.Combine(SampleFolderPath, "schemaWithConst.json");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new());
            Assert.Equal("active", schema.Properties["status"].Const);
            Assert.Equal("admin", schema.Properties["user"].Properties["role"].Const);

            // serialization
            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiJsonWriter(writer));
            var schemaString = writer.ToString();
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void ParseSchemaWithUnrecognizedKeywordsWorks() 
        {
            var input = @"{
    ""type"": ""string"",
    ""format"": ""date-time"",
    ""customKeyword"": ""customValue"",
    ""anotherKeyword"": 42,
    ""x-test"": ""test""
}
";
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, new(), out _, "json");
            Assert.Equal(2, schema.UnrecognizedKeywords.Count);
        }

        [Fact]
        public void ParseSchemaExampleWithPrimitivesWorks()
        {
            var expected1 = @"{
  ""type"": ""string"",
  ""example"": ""2024-01-02""
}";

            var expected2 = @"{
  ""type"": ""string"",
  ""example"": ""3.14""
}";
            var schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String,
                Example = JsonValue.Create("2024-01-02")
            };

            var schema2 = new OpenApiSchema()
            {
                Type = JsonSchemaType.String,
                Example = JsonValue.Create("3.14")
            };

            var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);
            schema.SerializeAsV31(writer);
            var actual1 = textWriter.ToString();
            Assert.Equal(expected1.MakeLineBreaksEnvironmentNeutral(), actual1.MakeLineBreaksEnvironmentNeutral());

            textWriter = new StringWriter();
            writer = new OpenApiJsonWriter(textWriter);
            schema2.SerializeAsV31(writer);
            var actual2 = textWriter.ToString();
            Assert.Equal(expected2.MakeLineBreaksEnvironmentNeutral(), actual2.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
