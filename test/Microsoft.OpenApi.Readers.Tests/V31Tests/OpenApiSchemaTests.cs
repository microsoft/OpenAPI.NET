// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;

#pragma warning disable CS0618

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

        [Fact]
        public async Task ParseBasicV31SchemaShouldSucceed()
        {
            var expectedObject = new OpenApiSchema()
            {
                Id = "https://example.com/arrays.schema.json",
                Schema = new Uri("https://json-schema.org/draft/2020-12/schema"),
                Description = "A representation of a person, company, organization, or place",
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["fruits"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        },
                        Contains = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        },
                        MinContains = 1,
                        MaxContains = 5
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
                        DependentRequired = new Dictionary<string, HashSet<string>>
                        {
                            { "veggieType", new HashSet<string> { "veggieColor", "veggieSize" } }
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
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
                            },
                            ["veggieType"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "The type of vegetable (e.g., root, leafy, etc.)."
                            },
                            ["veggieColor"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "The color of the vegetable."
                            },
                            ["veggieSize"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "The size of the vegetable."
                            }
                        }
                    }
                }
            };

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(
                Path.Combine(SampleFolderPath, "jsonSchema.json"), OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

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
                Schema = new Uri("https://json-schema.org/draft/2020-12/schema"),
                Description = "A representation of a person, company, organization, or place",
                Type = JsonSchemaType.Object | JsonSchemaType.Null
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Theory]
        [InlineData(@"{ ""nullable"": true, ""type"": ""string"" }")]
        [InlineData(@"{ ""type"": ""string"", ""nullable"": true }")]
        public void ParseSchemaWithNullableBeforeOrAfterTypeDoesNotPreserveNullFlag(string schemaJson)
        {
            // "nullable" is only for 3.0.

            // Act
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(schemaJson, OpenApiSpecVersion.OpenApi3_1, new(), out _, "json", SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equal(JsonSchemaType.String, schema.Type);
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);
            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
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
                        ExclusiveMinimum = "10"
                    },
                    ["seven"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMinimum false",
                        Minimum = "10"
                    },
                    ["eight"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMaximum true",
                        ExclusiveMaximum = "20"
                    },
                    ["nine"] = new OpenApiSchema()
                    {
                        Description = "exclusiveMaximum false",
                        Maximum = "20"
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
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, new(), out _, "yaml", SettingsFixture.ReaderSettings);

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
            Assert.Equivalent(new int[] { 1, 2, 3, 4 }, clone.Enum.Select(static x => x.GetValue<int>()).ToArray());
            Assert.Equivalent(new int[] { 2, 3, 4 }, clone.Examples.Select(static x => x.GetValue<int>()).ToArray());
            Assert.Equivalent(6, clone.Default.GetValue<int>());
        }

        [Fact]
        public void DefaultEmptyCollectionShouldRoundTrip()
        {
            // Given
            var serializedSchema =
            """
            {
                "type": "array",
                "items": {
                    "type": "string",
                    "default": []
                }
            }
            """;
            using var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);

            // When
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(serializedSchema, OpenApiSpecVersion.OpenApi3_1, new(), out _, "json", SettingsFixture.ReaderSettings);

            var deserializedArray = Assert.IsType<JsonArray>(schema.Items.Default);
            Assert.Empty(deserializedArray);

            schema.SerializeAsV31(writer);
            var roundTrippedSchema = textWriter.ToString();

            // Then
            var parsedResult = JsonNode.Parse(roundTrippedSchema);
            var parsedExpected = JsonNode.Parse(serializedSchema);
            Assert.True(JsonNode.DeepEquals(parsedExpected, parsedResult));
            var resultingArray = Assert.IsType<JsonArray>(parsedResult["items"]?["default"]);
            Assert.Empty(resultingArray);
        }

        [Fact]
        public void DefaultNullIsLossyDuringRoundTripJson()
        {
            // Given
            var serializedSchema =
            """
            {
                "type": ["string", "null"],
                "default": null
            }
            """;
            using var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);

            // When
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(serializedSchema, OpenApiSpecVersion.OpenApi3_1, new(), out _, "json", SettingsFixture.ReaderSettings);

            Assert.True(schema.Default.IsJsonNullSentinel());

            schema.SerializeAsV31(writer);
            var roundTrippedSchema = textWriter.ToString();

            // Then
            var parsedResult = Assert.IsType<JsonObject>(JsonNode.Parse(roundTrippedSchema));
            var parsedExpected = JsonNode.Parse(serializedSchema);
            Assert.False(JsonNode.DeepEquals(parsedExpected, parsedResult));
            Assert.True(parsedResult.TryGetPropertyValue("default", out var resultingDefault));
            Assert.Null(resultingDefault);
        }

        [Fact]
        public void DefaultNullIsLossyDuringRoundTripYaml()
        {
            // Given
            var serializedSchema =
            """
            type:
              - string
              - 'null'
            default: null
            """;
            using var textWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(textWriter);

            // When
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(serializedSchema, OpenApiSpecVersion.OpenApi3_1, new(), out _, "yaml", SettingsFixture.ReaderSettings);

            Assert.True(schema.Default.IsJsonNullSentinel());

            schema.SerializeAsV31(writer);
            var roundTrippedSchema = textWriter.ToString();

            // Then
            Assert.Equal(
            """
            type:
              - 'null'
              - string
            default: null
            """.MakeLineBreaksEnvironmentNeutral(),
            roundTrippedSchema.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV31SchemaWithMultipleTypesAsV3Works()
        {
            // Arrange
            var expected = @"type: string
nullable: true";

            var path = Path.Combine(SampleFolderPath, "schemaWithTypeArray.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

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
  - string";

            var path = Path.Combine(SampleFolderPath, "schemaWithNullableExtension.yaml");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi2_0, new(), SettingsFixture.ReaderSettings);

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

            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(input, OpenApiSpecVersion.OpenApi3_1, new(), out _, "yaml", SettingsFixture.ReaderSettings);

            var writer = new StringWriter();
            schema.SerializeAsV2(new OpenApiYamlWriter(writer));
            var schemaString = writer.ToString();

            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Theory]
        [InlineData("schemaWithNullable.yaml")]
        [InlineData("schemaWithNullableExtension.yaml")]
        public async Task LoadSchemaWithNullableExtensionAsV31ShouldNotWork(string filePath)
        {
            // "nullable" is only for 3.0.
            // and "x-nullable" is only for 2.0.
            // Arrange
            var path = Path.Combine(SampleFolderPath, filePath);

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equal(JsonSchemaType.String, schema.Type);
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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

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
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);
            Assert.Equal("active", schema.Properties["status"].Const);
            Assert.Equal("admin", schema.Properties["user"].Properties["role"].Const);

            // serialization
            var writer = new StringWriter();
            schema.SerializeAsV31(new OpenApiJsonWriter(writer));
            var schemaString = writer.ToString();
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), schemaString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task ParseSchemaWithConstNullWorks()
        {
            var expected = @"{
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""required"": [
    ""status""
  ],
  ""type"": ""object"",
  ""properties"": {
    ""status"": {
      ""const"": null,
      ""type"": ""string""
    },
    ""user"": {
      ""required"": [
        ""role""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""role"": {
          ""const"": null,
          ""type"": ""string""
        }
      }
    }
  }
}";

            var path = Path.Combine(SampleFolderPath, "schemaWithConstNull.json");

            // Act
            var schema = await OpenApiModelFactory.LoadAsync<OpenApiSchema>(path, OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            var statusSchema = Assert.IsType<OpenApiSchema>(schema.Properties["status"]);
            Assert.Null(statusSchema.Const);
            Assert.True(statusSchema.WasConstExplicitlySet);

            var userRoleSchema = Assert.IsType<OpenApiSchema>(schema.Properties["user"].Properties["role"]);
            Assert.Null(userRoleSchema.Const);
            Assert.True(userRoleSchema.WasConstExplicitlySet);

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

        [Theory]
        [InlineData(JsonSchemaType.Integer | JsonSchemaType.String, new[] { "integer", "string" })]
        [InlineData(JsonSchemaType.Integer | JsonSchemaType.Null, new[] { "integer", "null" })]
        [InlineData(JsonSchemaType.Integer, new[] { "integer" })]
        public void NormalizeFlaggableJsonSchemaTypeEnumWorks(JsonSchemaType type, string[] expected)
        {
            var schema = new OpenApiSchema
            {
                Type = type
            };

            var actual = schema.Type.ToIdentifiers();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(new[] { "integer", "string" }, JsonSchemaType.Integer | JsonSchemaType.String)]
        [InlineData(new[] { "integer", "null" }, JsonSchemaType.Integer | JsonSchemaType.Null)]
        [InlineData(new[] { "integer" }, JsonSchemaType.Integer)]
        public void ArrayIdentifierToEnumConversionWorks(string[] type, JsonSchemaType expected)
        {
            var actual = type.ToJsonSchemaType();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringIdentifierToEnumConversionWorks()
        {
            var actual = "integer".ToJsonSchemaType();
            Assert.Equal(JsonSchemaType.Integer, actual);
        }

        [Fact]
        public void ReturnSingleIdentifierWorks()
        {
            var type = JsonSchemaType.Integer;
            var types = JsonSchemaType.Integer | JsonSchemaType.Null;

            Assert.Equal("integer", type.ToSingleIdentifier());
            Assert.Throws<InvalidOperationException>(() => types.ToSingleIdentifier());
        }

        // UnevaluatedProperties deserialization tests
        [Fact]
        public void ParseSchemaWithUnevaluatedPropertiesBooleanFalse()
        {
            // Arrange
            var schema = @"{
  ""type"": ""object"",
  ""unevaluatedProperties"": false
}";

            var expected = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                UnevaluatedProperties = false
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void ParseSchemaWithUnevaluatedPropertiesBooleanTrue()
        {
            // Arrange - true should be parsed but is the default, effectively a no-op
            var schema = @"{
  ""type"": ""object"",
  ""unevaluatedProperties"": true
}";

            var expected = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                UnevaluatedProperties = true
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void ParseSchemaWithUnevaluatedPropertiesSchema()
        {
            // Arrange
            var schema = @"{
  ""type"": ""object"",
  ""unevaluatedProperties"": {
    ""type"": ""string""
  }
}";

            var expected = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String
                }
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void ParseSchemaWithUnevaluatedPropertiesComplexSchema()
        {
            // Arrange
            var schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""name"": { ""type"": ""string"" }
  },
  ""unevaluatedProperties"": {
    ""type"": ""number"",
    ""minimum"": ""0""
  }
}";

            var expected = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = JsonSchemaType.String }
                },
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Number,
                    Minimum = "0"
                }
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Fact]
        public void ParseSchemaWithoutUnevaluatedPropertiesDefaultsToTrue()
        {
            // Arrange - no unevaluatedProperties property should default to true (allow all)
            var schema = @"{
  ""type"": ""object"",
  ""properties"": {
    ""name"": { ""type"": ""string"" }
  }
}";

            var expected = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = JsonSchemaType.String }
                },
                UnevaluatedProperties = true // Default value
            };

            // Act
            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);

            // Assert
            Assert.Equivalent(expected, actual);
            Assert.True(actual.UnevaluatedProperties); // Explicitly verify the default
        }

        [Fact]
        public void ParseSchemaWithMissingJsonSchemaProperties()
        {
            var schema = @"{
  ""$anchor"": ""root"",
  ""contentEncoding"": ""base64"",
  ""contentMediaType"": ""application/jwt"",
  ""contentSchema"": {
    ""type"": ""array""
  },
  ""propertyNames"": {
    ""pattern"": ""^[a-z]+$""
  },
  ""dependentSchemas"": {
    ""token"": {
      ""type"": ""string""
    }
  },
  ""if"": {
    ""required"": [""token""]
  },
  ""then"": {
    ""minProperties"": 1
  },
  ""else"": {
    ""maxProperties"": 0
  }
}";

            var actual = OpenApiModelFactory.Parse<OpenApiSchema>(schema, OpenApiSpecVersion.OpenApi3_1, new(), out _);
            var missingProperties = Assert.IsAssignableFrom<IOpenApiSchemaMissingProperties>(actual);

            Assert.Equal("root", missingProperties.Anchor);
            Assert.Equal("base64", missingProperties.ContentEncoding);
            Assert.Equal("application/jwt", missingProperties.ContentMediaType);
            Assert.Equal(JsonSchemaType.Array, missingProperties.ContentSchema?.Type);
            Assert.Equal("^[a-z]+$", missingProperties.PropertyNames?.Pattern);
            Assert.Equal(JsonSchemaType.String, missingProperties.DependentSchemas?["token"].Type);
            Assert.NotNull(missingProperties.If?.Required);
            Assert.Contains("token", missingProperties.If.Required);
            Assert.Equal(1, missingProperties.Then?.MinProperties);
            Assert.Equal(0, missingProperties.Else?.MaxProperties);
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("true")]
        public void DeserializeTrueSchemaParsesAsEmptySchema(string schemaSource)
        {
            // Arrange & Act
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(schemaSource, OpenApiSpecVersion.OpenApi3_1, new(), out _, OpenApiConstants.Json);

            // Assert - schema should deserialize without error
            Assert.NotNull(schema);
        }

        [Fact]
        public void DeserializeFalseSchemaParsesAsNotEmptySchema()
        {
            // Arrange
            var schemaSource = "false";

            // Act
            var schema = OpenApiModelFactory.Parse<OpenApiSchema>(schemaSource, OpenApiSpecVersion.OpenApi3_1, new(), out _, OpenApiConstants.Json);

            // Assert - false schema should deserialize to not: {}
            Assert.NotNull(schema);
            Assert.NotNull(schema.Not);
            Assert.Empty(schema.Not.AnyOf ?? []);
            Assert.Empty(schema.Not.AllOf ?? []);
            Assert.Empty(schema.Not.OneOf ?? []);
        }

        [Fact]
        public async Task ParseSchemaReferencePreservesJsonSchema2020KeywordSiblings()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Sibling preservation repro
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                      properties:
                        name:
                          type: string
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      description: Sibling description
                      $dynamicAnchor: anchor
                      $defs:
                        sibling:
                          $dynamicAnchor: inner
                          $ref: '#/components/schemas/Target'
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];

            // Assert — siblings are preserved on the OpenApiSchemaReference
            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.Equal("Sibling description", referencing.Description);
            Assert.Equal("anchor", referencing.DynamicAnchor);
            Assert.NotNull(referencing.Definitions);
            Assert.True(referencing.Definitions.ContainsKey("sibling"));
            Assert.Equal("inner", referencing.Definitions["sibling"].DynamicAnchor);
        }

        [Fact]
        public async Task SerializeSchemaReferencePreservesJsonSchema2020KeywordSiblings()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Sibling preservation repro
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                      properties:
                        name:
                          type: string
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $dynamicAnchor: anchor
                      $defs:
                        itemType:
                          $dynamicAnchor: itemType
                          $ref: '#/components/schemas/Target'
                """;

            // Act — parse then serialize back
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            var writer = new StringWriter();
            result.Document.SerializeAsV31(new OpenApiYamlWriter(writer));
            var output = writer.ToString();

            // Assert — round-trip preserves $dynamicAnchor and $defs alongside $ref
            using var roundTripStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(output));
            var roundTripResult = await OpenApiDocument.LoadAsync(roundTripStream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(roundTripResult.Document.Components);
            Assert.NotNull(roundTripResult.Document.Components.Schemas);
            var referencing = roundTripResult.Document.Components.Schemas["Referencing"];

            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.Equal("anchor", referencing.DynamicAnchor);
            Assert.NotNull(referencing.Definitions);
            Assert.True(referencing.Definitions.ContainsKey("itemType"));
            Assert.Equal("itemType", referencing.Definitions["itemType"].DynamicAnchor);
        }

        [Fact]
        public async Task ParseSchemaReferencePreservesScalarKeywordSiblings()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Scalar sibling repro
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $id: 'https://example.com/referencing.json'
                      $schema: 'https://json-schema.org/draft/2020-12/schema'
                      $comment: A comment sibling
                      $anchor: myAnchor
                      $dynamicRef: '#myAnchor'
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];

            // Assert
            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.Equal("https://example.com/referencing.json", referencing.Id);
            Assert.Equal(new Uri("https://json-schema.org/draft/2020-12/schema"), referencing.Schema);
            Assert.Equal("A comment sibling", referencing.Comment);
            Assert.Equal("myAnchor", ((IOpenApiSchemaMissingProperties)referencing).Anchor);
            Assert.Equal("#myAnchor", referencing.DynamicRef);
        }

        [Fact]
        public async Task SerializeSchemaReferencePreservesScalarKeywordSiblings()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Scalar round-trip
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $id: 'https://example.com/referencing.json'
                      $schema: 'https://json-schema.org/draft/2020-12/schema'
                      $comment: A comment sibling
                      $anchor: myAnchor
                      $dynamicRef: '#myAnchor'
                """;

            // Act — parse then serialize back
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            var writer = new StringWriter();
            result.Document.SerializeAsV31(new OpenApiYamlWriter(writer));
            var output = writer.ToString();

            // Assert — round-trip preserves scalar siblings alongside $ref
            using var roundTripStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(output));
            var roundTripResult = await OpenApiDocument.LoadAsync(roundTripStream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(roundTripResult.Document.Components);
            Assert.NotNull(roundTripResult.Document.Components.Schemas);
            var referencing = roundTripResult.Document.Components.Schemas["Referencing"];

            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.Equal("https://example.com/referencing.json", referencing.Id);
            Assert.Equal(new Uri("https://json-schema.org/draft/2020-12/schema"), referencing.Schema);
            Assert.Equal("A comment sibling", referencing.Comment);
            Assert.Equal("myAnchor", ((IOpenApiSchemaMissingProperties)referencing).Anchor);
            Assert.Equal("#myAnchor", referencing.DynamicRef);
        }

        [Fact]
        public async Task ParseSchemaReferencePreservesVocabularySibling()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Vocabulary sibling repro
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $vocabulary:
                        'https://json-schema.org/draft/2020-12/vocab/core': true
                        'https://json-schema.org/draft/2020-12/vocab/applicator': false
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];

            // Assert
            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.NotNull(referencing.Vocabulary);
            Assert.Equal(2, referencing.Vocabulary.Count);
            Assert.True(referencing.Vocabulary["https://json-schema.org/draft/2020-12/vocab/core"]);
            Assert.False(referencing.Vocabulary["https://json-schema.org/draft/2020-12/vocab/applicator"]);
        }

        [Fact]
        public async Task ParseSchemaReferencePreservesDynamicAnchorInsideDefsInAllOf()
        {
            // Arrange — the allOf-based binding variant: $defs sits inside allOf[0],
            // and the nested schema has $ref + $dynamicAnchor (the binding entry).
            // This was called out as a real-world pattern that hits the same root cause
            // because the inner schema is an OpenApiSchemaReference whose sibling was dropped.
            var yaml = """
                openapi: 3.1.0
                info:
                  title: allOf binding variant
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Asset:
                      type: object
                      properties:
                        id:
                          type: string
                    Paged:
                      type: object
                      properties:
                        items:
                          type: array
                    AssetPaged:
                      allOf:
                        - $defs:
                            contentType:
                              $dynamicAnchor: contentType
                              $ref: '#/components/schemas/Asset'
                        - $ref: '#/components/schemas/Paged'
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var assetPaged = result.Document.Components.Schemas["AssetPaged"];

            // Assert — the binding entry inside $defs/allOf[0] is reachable
            // allOf[0] is a regular OpenApiSchema (no $ref at top level), so $defs is parsed normally.
            // The nested contentType schema is an OpenApiSchemaReference ($ref: Asset),
            // and its $dynamicAnchor sibling must be preserved.
            Assert.NotNull(assetPaged.AllOf);
            Assert.Equal(2, assetPaged.AllOf.Count);
            var defsHolder = assetPaged.AllOf[0];
            Assert.NotNull(defsHolder.Definitions);
            Assert.True(defsHolder.Definitions.ContainsKey("contentType"));
            var contentType = defsHolder.Definitions["contentType"];
            Assert.IsType<OpenApiSchemaReference>(contentType);
            Assert.Equal("contentType", contentType.DynamicAnchor);
        }

        [Fact]
        public async Task EmptySiblingCollectionsFallThroughToTarget()
        {
            // Arrange — Target has $defs and $vocabulary; Referencing has empty siblings.
            // Empty collections must NOT suppress the target's values via the ?? getter.
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Empty sibling fallthrough
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                      $defs:
                        targetDef:
                          type: string
                      $vocabulary:
                        'https://json-schema.org/draft/2020-12/vocab/core': true
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $defs: {}
                      $vocabulary: {}
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];

            // Assert — empty siblings fall through to Target's values
            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.NotNull(referencing.Definitions);
            Assert.True(referencing.Definitions.ContainsKey("targetDef"));
            Assert.NotNull(referencing.Vocabulary);
            Assert.True(referencing.Vocabulary.ContainsKey("https://json-schema.org/draft/2020-12/vocab/core"));
        }

        [Fact]
        public async Task SiblingsOnRefAreDroppedForOpenApi30()
        {
            // Arrange — 3.0 spec requires $ref siblings to be ignored.
            // The fix must not change 3.0 behavior.
            var yaml = """
                openapi: 3.0.3
                info:
                  title: 3.0 version safety
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                      properties:
                        name:
                          type: string
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $dynamicAnchor: anchor
                      $defs:
                        sibling:
                          $dynamicAnchor: inner
                          $ref: '#/components/schemas/Target'
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];

            // Assert — siblings are dropped for 3.0 (per spec: $ref siblings MUST be ignored)
            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.Null(referencing.DynamicAnchor);
            Assert.Null(referencing.Definitions);
        }

        [Fact]
        public async Task SerializeSchemaReferencePreservesVocabularySibling()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Vocabulary round-trip
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $vocabulary:
                        'https://json-schema.org/draft/2020-12/vocab/core': true
                        'https://json-schema.org/draft/2020-12/vocab/applicator': false
                """;

            // Act — parse then serialize back
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            var writer = new StringWriter();
            result.Document.SerializeAsV31(new OpenApiYamlWriter(writer));
            var output = writer.ToString();

            // Assert — round-trip preserves $vocabulary alongside $ref
            using var roundTripStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(output));
            var roundTripResult = await OpenApiDocument.LoadAsync(roundTripStream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(roundTripResult.Document.Components);
            Assert.NotNull(roundTripResult.Document.Components.Schemas);
            var referencing = roundTripResult.Document.Components.Schemas["Referencing"];

            Assert.IsType<OpenApiSchemaReference>(referencing);
            Assert.NotNull(referencing.Vocabulary);
            Assert.Equal(2, referencing.Vocabulary.Count);
            Assert.True(referencing.Vocabulary["https://json-schema.org/draft/2020-12/vocab/core"]);
            Assert.False(referencing.Vocabulary["https://json-schema.org/draft/2020-12/vocab/applicator"]);
        }

        [Fact]
        public async Task CreateShallowCopyPreservesKeywordSiblings()
        {
            // Arrange
            var yaml = """
                openapi: 3.1.0
                info:
                  title: Shallow copy repro
                  version: 1.0.0
                paths: {}
                components:
                  schemas:
                    Target:
                      type: object
                    Referencing:
                      $ref: '#/components/schemas/Target'
                      $dynamicAnchor: anchor
                      $defs:
                        sibling:
                          $dynamicAnchor: inner
                          $ref: '#/components/schemas/Target'
                """;

            // Act
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(yaml));
            var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
            Assert.NotNull(result.Document.Components);
            Assert.NotNull(result.Document.Components.Schemas);
            var referencing = result.Document.Components.Schemas["Referencing"];
            var copy = referencing.CreateShallowCopy();

            // Assert — CreateShallowCopy preserves sibling values via the JsonSchemaReference copy constructor
            Assert.IsType<OpenApiSchemaReference>(copy);
            Assert.Equal("anchor", copy.DynamicAnchor);
            Assert.NotNull(copy.Definitions);
            Assert.True(copy.Definitions.ContainsKey("sibling"));
            Assert.Equal("inner", copy.Definitions["sibling"].DynamicAnchor);
        }
    }
}
