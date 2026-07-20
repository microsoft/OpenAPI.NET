// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        private static readonly OpenApiSchema BasicSchema = new();

        public static readonly OpenApiSchema AdvancedSchemaNumber = new()
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = "42",
            ExclusiveMinimum = "10",
            Default = 15,
            Type = JsonSchemaType.Integer | JsonSchemaType.Null,

            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            },
            Metadata = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } }
        };

        public static readonly OpenApiSchema AdvancedSchemaObject = new()
        {
            Title = "title1",
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                ["property1"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15
                        }
                    },
                },
                ["property4"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property5"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>()
                            {
                                ["property6"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                },
            },
            Type = JsonSchemaType.Object | JsonSchemaType.Null,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        public static readonly OpenApiSchema AdvancedSchemaWithAllOf = new()
        {
            Title = "title1",
            AllOf =
            [
                new OpenApiSchema()
                {
                    Title = "title2",
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15
                        }
                    },
                },
                new OpenApiSchema()
                {
                    Title = "title3",
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property3"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>()
                            {
                                ["property4"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property5"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                    Type = JsonSchemaType.Object | JsonSchemaType.Null,
                },
            ],
            Type = JsonSchemaType.Object | JsonSchemaType.Null,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        public static readonly OpenApiSchema ReferencedSchema = new()
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = "42",
            ExclusiveMinimum = "10",
            Default = 15,
            Type = JsonSchemaType.Integer | JsonSchemaType.Null,

            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        public static readonly OpenApiSchema AdvancedSchemaWithRequiredPropertiesObject = new()
        {
            Title = "title1",
            Required = new HashSet<string> { "property1" },
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                ["property1"] = new OpenApiSchema()
                {
                    Required = new HashSet<string> { "property3" },
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15,
                            ReadOnly = true
                        }
                    },
                    ReadOnly = true,
                },
                ["property4"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, IOpenApiSchema>()
                    {
                        ["property5"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>()
                            {
                                ["property6"] = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                    ReadOnly = true,
                },
            },
            Type = JsonSchemaType.Object | JsonSchemaType.Null,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeBasicSchemaAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = await BasicSchema.SerializeAsJsonAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedSchemaNumberAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "title": "title1",
                  "multipleOf": 3,
                  "maximum": 42,
                  "minimum": 10,
                  "exclusiveMinimum": true,
                  "type": "integer",
                  "nullable": true,
                  "default": 15,
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = await AdvancedSchemaNumber.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdvancedSchemaObjectAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "title": "title1",
                  "type": "object",
                  "nullable": true,
                  "properties": {
                    "property1": {
                      "properties": {
                        "property2": {
                          "type": "integer"
                        },
                        "property3": {
                          "maxLength": 15,
                          "type": "string"
                        }
                      }
                    },
                    "property4": {
                      "properties": {
                        "property5": {
                          "properties": {
                            "property6": {
                              "type": "boolean"
                            }
                          }
                        },
                        "property7": {
                          "minLength": 2,
                          "type": "string"
                        }
                      }
                    }
                  },
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = await AdvancedSchemaObject.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdvancedSchemaWithAllOfAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "title": "title1",
                  "type": "object",
                  "nullable": true,
                  "allOf": [
                    {
                      "title": "title2",
                      "properties": {
                        "property1": {
                          "type": "integer"
                        },
                        "property2": {
                          "maxLength": 15,
                          "type": "string"
                        }
                      }
                    },
                    {
                      "title": "title3",
                      "type": "object",
                      "nullable": true,
                      "properties": {
                        "property3": {
                          "properties": {
                            "property4": {
                              "type": "boolean"
                            }
                          }
                        },
                        "property5": {
                          "minLength": 2,
                          "type": "string"
                        }
                      }
                    }
                  ],
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = await AdvancedSchemaWithAllOf.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonObject.DeepEquals(JsonObject.Parse(expected), JsonObject.Parse(actual)));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSchemaAsV3WithoutReferenceJsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedSchema.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSchemaAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedSchema.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaWRequiredPropertiesAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedSchemaWithRequiredPropertiesObject.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeAsV2ShouldSetFormatPropertyInParentSchemaIfPresentInChildrenSchema()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                OneOf =
                [
                    new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "decimal"
                    },
                    new OpenApiSchema() { Type = JsonSchemaType.String },
                ]
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var openApiJsonWriter = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            // Serialize as V2
            schema.SerializeAsV2(openApiJsonWriter);
            await openApiJsonWriter.FlushAsync();

            var v2Schema = outputStringWriter.GetStringBuilder().ToString().MakeLineBreaksEnvironmentNeutral();

            var expectedV2Schema =
                """
                {
                  "format": "decimal",
                  "allOf": [
                    {
                      "type": "number",
                      "format": "decimal"
                    }
                  ]
                }
                """.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(v2Schema, expectedV2Schema);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "date"
            };

            var actualSchema = baseSchema.CreateShallowCopy() as OpenApiSchema;
            actualSchema.Type |= JsonSchemaType.Null;

            Assert.Equal(JsonSchemaType.String, actualSchema.Type & JsonSchemaType.String);
            Assert.Equal(JsonSchemaType.Null, actualSchema.Type & JsonSchemaType.Null);
            Assert.Equal("date", actualSchema.Format);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorWithMetadataSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Metadata = new Dictionary<string, object>
                {
                    ["key1"] = "value1",
                    ["key2"] = 2
                }
            };

            var actualSchema = Assert.IsType<OpenApiSchema>(baseSchema.CreateShallowCopy());

            Assert.Equal(baseSchema.Metadata["key1"], actualSchema.Metadata["key1"]);

            baseSchema.Metadata["key1"] = "value2";

            Assert.NotEqual(baseSchema.Metadata["key1"], actualSchema.Metadata["key1"]);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorWithUnevaluatedPropertiesSchemaSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                UnevaluatedProperties = false,
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    MaxLength = 100
                }
            };

            var actualSchema = Assert.IsType<OpenApiSchema>(baseSchema.CreateShallowCopy());

            // Verify boolean property is copied
            Assert.Equal(baseSchema.UnevaluatedProperties, actualSchema.UnevaluatedProperties);
            
            // Verify schema property is copied
            Assert.NotNull(actualSchema.UnevaluatedPropertiesSchema);
            Assert.Equal(JsonSchemaType.String, actualSchema.UnevaluatedPropertiesSchema.Type);
            Assert.Equal(100, actualSchema.UnevaluatedPropertiesSchema.MaxLength);

            // Verify it's a shallow copy (different object reference)
            Assert.NotSame(baseSchema.UnevaluatedPropertiesSchema, actualSchema.UnevaluatedPropertiesSchema);
            
            // Verify that changing the copy doesn't affect the original
            var actualSchemaTyped = Assert.IsType<OpenApiSchema>(actualSchema.UnevaluatedPropertiesSchema);
            actualSchemaTyped.MaxLength = 200;
            Assert.Equal(100, baseSchema.UnevaluatedPropertiesSchema.MaxLength);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorWithMissingPropertiesSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Anchor = "root",
                UnevaluatedProperties = false,
                UnevaluatedPropertiesSchema = new OpenApiSchema { Type = JsonSchemaType.String },
                ContentEncoding = "base64",
                ContentMediaType = "application/jwt",
                ContentSchema = new OpenApiSchema { Type = JsonSchemaType.Array },
                PropertyNames = new OpenApiSchema { Pattern = "^[a-z]+$" },
                DependentSchemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["token"] = new OpenApiSchema { Type = JsonSchemaType.String }
                },
                If = new OpenApiSchema { Required = new HashSet<string> { "token" } },
                Then = new OpenApiSchema { MinProperties = 1 },
                Else = new OpenApiSchema { MaxProperties = 0 }
            };

            var actualSchema = Assert.IsType<OpenApiSchema>(baseSchema.CreateShallowCopy());
            var actualMissingProperties = Assert.IsAssignableFrom<IOpenApiSchemaMissingProperties>(actualSchema);

            Assert.Equal("root", actualMissingProperties.Anchor);
            Assert.False(actualMissingProperties.UnevaluatedProperties);
            Assert.NotNull(actualMissingProperties.UnevaluatedPropertiesSchema);
            Assert.Equal("base64", actualMissingProperties.ContentEncoding);
            Assert.Equal("application/jwt", actualMissingProperties.ContentMediaType);
            Assert.NotNull(actualMissingProperties.ContentSchema);
            Assert.NotNull(actualMissingProperties.PropertyNames);
            Assert.NotNull(actualMissingProperties.DependentSchemas);
            Assert.NotNull(actualMissingProperties.If);
            Assert.NotNull(actualMissingProperties.Then);
            Assert.NotNull(actualMissingProperties.Else);
            Assert.NotSame(baseSchema.ContentSchema, actualMissingProperties.ContentSchema);
            Assert.NotSame(baseSchema.PropertyNames, actualMissingProperties.PropertyNames);
            Assert.NotSame(baseSchema.If, actualMissingProperties.If);
            Assert.NotSame(baseSchema.Then, actualMissingProperties.Then);
            Assert.NotSame(baseSchema.Else, actualMissingProperties.Else);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorWithContainsSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Contains = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    MaxLength = 100
                },
                MinContains = 1,
                MaxContains = 5
            };

            var actualSchema = Assert.IsType<OpenApiSchema>(baseSchema.CreateShallowCopy());

            // Verify scalar properties are copied
            Assert.Equal(baseSchema.MinContains, actualSchema.MinContains);
            Assert.Equal(baseSchema.MaxContains, actualSchema.MaxContains);

            // Verify schema property is copied
            Assert.NotNull(actualSchema.Contains);
            Assert.Equal(JsonSchemaType.String, actualSchema.Contains.Type);
            Assert.Equal(100, actualSchema.Contains.MaxLength);

            // Verify it's a shallow copy (different object reference)
            Assert.NotSame(baseSchema.Contains, actualSchema.Contains);

            // Verify that changing the copy doesn't affect the original
            var actualContainsTyped = Assert.IsType<OpenApiSchema>(actualSchema.Contains);
            actualContainsTyped.MaxLength = 200;
            Assert.Equal(100, baseSchema.Contains.MaxLength);
        }

        public static TheoryData<JsonNode> SchemaExamples()
        {
            return new()
            {
                new JsonArray() { "example" },
                new JsonArray { 0, 1, 2 },   // Represent OpenApiBinary as JsonArray of bytes
                true,
                JsonValue.Create((byte)42),
                JsonValue.Create(new DateTime(2024, 07, 19, 12, 34, 56, DateTimeKind.Utc).ToString("o")), // DateTime object
                42.37,
                42.37f,
                42,
                null,
                JsonValue.Create("secret"), //Represent OpenApiPassword as string
                "example",
            };
        }

        [Theory]
        [MemberData(nameof(SchemaExamples))]
        public void CloningSchemaExamplesWorks(JsonNode example)
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Example = example
            };

            // Act && Assert
            var schemaCopy = schema.CreateShallowCopy();

            // Act && Assert
            schema.Example.Should().BeEquivalentTo(schemaCopy.Example, options => options
            .IgnoringCyclicReferences()
            .Excluding(x => x.Options));
        }

        [Fact]
        public void CloningSchemaExtensionsWorks()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    { "x-myextension", new JsonNodeExtension(42) }
                }
            };

            // Act && Assert
            var schemaCopy = schema.CreateShallowCopy() as OpenApiSchema;
            Assert.Single(schemaCopy.Extensions);

            // Act && Assert
            schemaCopy.Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                { "x-myextension" , new JsonNodeExtension(40) }
            };
            Assert.NotEqual(schema.Extensions, schemaCopy.Extensions);
        }

        [Fact]
        public void OpenApiWalkerVisitsOpenApiSchemaNot()
        {
            var outerSchema = new OpenApiSchema()
            {
                Title = "Outer Schema",
                Not = new OpenApiSchema()
                {
                    Title = "Inner Schema",
                    Type = JsonSchemaType.String,
                }
            };

            var document = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/foo"] = new OpenApiPathItem()
                    {
                        Parameters =
                        [
                            new OpenApiParameter()
                            {
                                Name = "foo",
                                In = ParameterLocation.Query,
                                Schema = outerSchema,
                            }
                        ]
                    }
                }
            };

            // Act
            var visitor = new SchemaVisitor();
            var walker = new OpenApiWalker(visitor);
            walker.Walk(document);

            // Assert
            Assert.Equal(2, visitor.Titles.Count);
        }

        [Fact]
        public async Task SerializeSchemaWithUnrecognizedPropertiesWorks()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                UnrecognizedKeywords = new Dictionary<string, JsonNode>()
                {
                    ["customKeyWord"] = "bar",
                    ["anotherKeyword"] = 42
                }
            };

            var expected = @"{
  ""unrecognizedKeywords"": {
    ""customKeyWord"": ""bar"",
    ""anotherKeyword"": 42
  }
}";

            // Act
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task WriteAsItemsPropertiesDoesNotWriteNull()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Number | JsonSchemaType.Null
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });
            writer.WriteStartObject();

            // Act
            schema.WriteAsItemsProperties(writer);
            writer.WriteEndObject();
            await writer.FlushAsync();

            // Assert
            var actual = outputStringWriter.GetStringBuilder().ToString();
            var expected =
            """
            {
                "type": "number"
            }
            """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }
        [Fact]
        public async Task SerializeConstAsEnumV30()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Const = "foo"
            };


            // Act
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            var v3Node = JsonNode.Parse(actual);
            Assert.NotNull(v3Node);
            Assert.True(v3Node["enum"] is JsonArray singleEnum && singleEnum.Count == 1 && singleEnum[0]?.ToString() == "foo");
            Assert.False(v3Node.AsObject().ContainsKey("const"));
        }

        [Fact]
        public async Task SerializeConstAsEnumV20()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Const = "foo"
            };

            // Act
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            var v2Node = JsonNode.Parse(actual);
            Assert.NotNull(v2Node);
            Assert.True(v2Node["enum"] is JsonArray singleEnum && singleEnum.Count == 1 && singleEnum[0]?.ToString() == "foo");
            Assert.False(v2Node.AsObject().ContainsKey("const"));
        }

        [Fact]
        public async Task SerializeAdditionalPropertiesAsV2WithEmptySchemaEmits()
        {
            var expected = @"{ ""additionalProperties"": { } }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalProperties = new OpenApiSchema()
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdditionalPropertiesAsV2WithRefSchemaEmits()
        {
            var expected = @"{ ""type"": ""object"", ""additionalProperties"": { ""$ref"": ""#/definitions/MyModel"" } }";
            // Given - schema with additionalProperties pointing to a ref (dictionary case)
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                AdditionalProperties = new OpenApiSchemaReference("MyModel", null)
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdditionalPropertiesAllowedAsV2DefaultDoesNotEmit()
        {
            var expected = @"{ }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalPropertiesAllowed = true
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeAdditionalPropertiesAllowedAsV2FalseEmits()
        {
            var expected = @"{ ""additionalProperties"": false }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalPropertiesAllowed = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeAdditionalPropertiesAllowedAsV3PlusDefaultDoesNotEmit(OpenApiSpecVersion version)
        {
            var expected = @"{ }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalPropertiesAllowed = true
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeAdditionalPropertiesAllowedAsV3PlusFalseEmits(OpenApiSpecVersion version)
        {
            var expected = @"{ ""additionalProperties"": false }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalPropertiesAllowed = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeAdditionalPropertiesAsV3PlusEmits(OpenApiSpecVersion version)
        {
            var expected = @"{ ""additionalProperties"": { } }";
            // Given
            var schema = new OpenApiSchema
            {
                AdditionalProperties = new OpenApiSchema()
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeOneOfWithNullAsV3ShouldUseNullableAsync()
        {
            // Arrange - oneOf with null and a reference-like schema
            var schema = new OpenApiSchema
            {
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        MaxLength = 10
                    }
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "oneOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "maxLength": 10,
                      "type": "string"
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeOneOfWithNullAndMultipleSchemasAsV3ShouldMarkItAsNullableWithoutType()
        {
            // Arrange - oneOf with null, string, and number
            var schema = new OpenApiSchema
            {
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    new OpenApiSchema { Type = JsonSchemaType.String },
                    new OpenApiSchema { Type = JsonSchemaType.Number },
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "oneOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "type": "string"
                    },
                    {
                      "type": "number"
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeAnyOfWithNullAsV3ShouldUseNullableAsync()
        {
            // Arrange - anyOf with null and object schema
            var schema = new OpenApiSchema
            {
                AnyOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema { Type = JsonSchemaType.Integer }
                        }
                    }
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "anyOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "type": "object",
                      "properties": {
                        "id": {
                          "type": "integer"
                        }
                      }
                    }
                  ]
                }
                """;
            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeAnyOfWithNullAndMultipleSchemasAsV3ShouldApplyNullable()
        {
            // Arrange - anyOf with null and multiple schemas
            var schema = new OpenApiSchema
            {
                AnyOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    new OpenApiSchema { Type = JsonSchemaType.String, MinLength = 1 },
                    new OpenApiSchema { Type = JsonSchemaType.Integer }
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "anyOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "minLength": 1,
                      "type": "string"
                    },
                    {
                      "type": "integer"
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeOneOfWithOnlyNullAsV3ShouldJustBeNullableAsync()
        {
            // Arrange - oneOf with only null
            var schema = new OpenApiSchema
            {
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null }
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "oneOf": [
                    {
                      "enum": [
                        null
                      ]
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeOneOfWithNullAsV31ShouldNotChangeAsync()
        {
            // Arrange - oneOf with null should remain unchanged in v3.1
            var schema = new OpenApiSchema
            {
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    new OpenApiSchema { Type = JsonSchemaType.String }
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV31(writer);
            await writer.FlushAsync();

            var v31Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV31Schema =
                """
                {
                  "oneOf": [
                    {
                      "type": "null"
                    },
                    {
                      "type": "string"
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV31Schema), JsonNode.Parse(v31Schema)));
        }

        [Fact]
        public async Task SerializeOneOfWithNullAndRefAsV3ShouldUseNullableAsync()
        {
            // Arrange - oneOf with null and a $ref to a schema component
            var document = new OpenApiDocument
            {
                Components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, IOpenApiSchema>
                    {
                        ["Pet"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["id"] = new OpenApiSchema { Type = JsonSchemaType.Integer },
                                ["name"] = new OpenApiSchema { Type = JsonSchemaType.String }
                            }
                        }
                    }
                }
            };

            // Register components so references can be resolved
            document.Workspace.RegisterComponents(document);

            var schemaRef = new OpenApiSchemaReference("Pet", document);

            var schema = new OpenApiSchema
            {
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema { Type = JsonSchemaType.Null },
                    schemaRef
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "oneOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "$ref": "#/components/schemas/Pet"
                    }
                  ]
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        [Fact]
        public async Task SerializeContainsKeywordsAsV31Works()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Contains = new OpenApiSchema { Type = JsonSchemaType.String },
                MinContains = 1,
                MaxContains = 5
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV31(writer);
            await writer.FlushAsync();

            var v31Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV31Schema =
                """
                {
                  "type": "array",
                  "contains": {
                    "type": "string"
                  },
                  "maxContains": 5,
                  "minContains": 1
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV31Schema), JsonNode.Parse(v31Schema)));
        }

        [Fact]
        public async Task SerializeContainsKeywordsAsV3EmitsCompatibilityExtensions()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Contains = new OpenApiSchema { Type = JsonSchemaType.String },
                MinContains = 1,
                MaxContains = 5
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();

            var v3Schema = outputStringWriter.GetStringBuilder().ToString();

            var expectedV3Schema =
                """
                {
                  "type": "array",
                  "x-jsonschema-contains": {
                    "type": "string"
                  },
                  "x-jsonschema-maxContains": 5,
                  "x-jsonschema-minContains": 1
                }
                """;

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expectedV3Schema), JsonNode.Parse(v3Schema)));
        }

        // UnevaluatedProperties tests - similar to AdditionalProperties pattern
        [Fact]
        public async Task SerializeUnevaluatedPropertiesBooleanDefaultDoesNotEmit()
        {
            var expected = @"{ }";
            // Given - default (not explicitly set) should not emit
            var schema = new OpenApiSchema();

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeUnevaluatedPropertiesBooleanTrueDoesNotEmit()
        {
            var expected = @"{ }";
            // Given - true (allowing all) is the default, no need to emit
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = true
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeUnevaluatedPropertiesBooleanFalseEmitsInV31()
        {
            var expected = @"{ ""unevaluatedProperties"": false }";
            // Given
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeUnevaluatedPropertiesSchemaEmits(OpenApiSpecVersion version)
        {
            var expected = @"{ ""unevaluatedProperties"": { ""type"": ""string"" } }";
            // Given
            var schema = new OpenApiSchema
            {
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String
                }
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeUnevaluatedPropertiesSchemaTakesPrecedenceOverBoolean()
        {
            var expected = @"{ ""unevaluatedProperties"": { ""type"": ""integer"" } }";
            // Given - schema should take precedence over boolean
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = false, // This should be ignored
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer
                }
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializeUnevaluatedPropertiesAsExtensionInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = @"{ ""x-jsonschema-unevaluatedProperties"": false }";
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = false
            };

            var actual = await schema.SerializeAsJsonAsync(version);

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializeUnevaluatedPropertiesSchemaAsExtensionInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = @"{ ""x-jsonschema-unevaluatedProperties"": { ""type"": ""string"" } }";
            var schema = new OpenApiSchema
            {
                UnevaluatedPropertiesSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String
                }
            };

            var actual = await schema.SerializeAsJsonAsync(version);

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializeUnevaluatedPropertiesTrueNotEmittedInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = @"{ }";
            // Given - UnevaluatedProperties true (default) should not be emitted even as extension
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = true
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeMissingPropertiesEmitsJsonSchemaKeywordsInV31()
        {
            var expected = JsonNode.Parse("""
                {
                  "$anchor": "root",
                  "contentEncoding": "base64",
                  "contentMediaType": "application/jwt",
                  "contentSchema": {
                    "type": "array"
                  },
                  "propertyNames": {
                    "pattern": "^[a-z]+$"
                  },
                  "dependentSchemas": {
                    "token": {
                      "type": "string"
                    }
                  },
                  "if": {
                    "required": [
                      "token"
                    ]
                  },
                  "then": {
                    "minProperties": 1
                  },
                  "else": {
                    "maxProperties": 0
                  }
                }
                """);

            var schema = new OpenApiSchema
            {
                Anchor = "root",
                ContentEncoding = "base64",
                ContentMediaType = "application/jwt",
                ContentSchema = new OpenApiSchema { Type = JsonSchemaType.Array },
                PropertyNames = new OpenApiSchema { Pattern = "^[a-z]+$" },
                DependentSchemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["token"] = new OpenApiSchema { Type = JsonSchemaType.String }
                },
                If = new OpenApiSchema { Required = new HashSet<string> { "token" } },
                Then = new OpenApiSchema { MinProperties = 1 },
                Else = new OpenApiSchema { MaxProperties = 0 }
            };

            var actual = JsonNode.Parse(await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1));

            Assert.True(JsonNode.DeepEquals(expected, actual));
        }

        [Fact]
        public async Task SerializeMissingPropertiesEmitsOaiExtensionsInV3()
        {
            var expected = JsonNode.Parse("""
                {
                  "x-jsonschema-$anchor": "root",
                  "x-jsonschema-contentEncoding": "base64",
                  "x-jsonschema-contentMediaType": "application/jwt",
                  "x-jsonschema-contentSchema": {
                    "type": "array"
                  },
                  "x-jsonschema-contains": {
                    "type": "string"
                  },
                  "x-jsonschema-maxContains": 3,
                  "x-jsonschema-minContains": 1,
                  "x-jsonschema-propertyNames": {
                    "pattern": "^[a-z]+$"
                  },
                  "x-jsonschema-dependentSchemas": {
                    "token": {
                      "type": "string"
                    }
                  },
                  "x-jsonschema-if": {
                    "required": [
                      "token"
                    ]
                  },
                  "x-jsonschema-then": {
                    "minProperties": 1
                  },
                  "x-jsonschema-else": {
                    "maxProperties": 0
                  }
                }
                """);

            var schema = new OpenApiSchema
            {
                Anchor = "root",
                ContentEncoding = "base64",
                ContentMediaType = "application/jwt",
                ContentSchema = new OpenApiSchema { Type = JsonSchemaType.Array },
                Contains = new OpenApiSchema { Type = JsonSchemaType.String },
                MaxContains = 3,
                MinContains = 1,
                PropertyNames = new OpenApiSchema { Pattern = "^[a-z]+$" },
                DependentSchemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["token"] = new OpenApiSchema { Type = JsonSchemaType.String }
                },
                If = new OpenApiSchema { Required = new HashSet<string> { "token" } },
                Then = new OpenApiSchema { MinProperties = 1 },
                Else = new OpenApiSchema { MaxProperties = 0 }
            };

            var actual = JsonNode.Parse(await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0));

            Assert.True(JsonNode.DeepEquals(expected, actual));
        }

        [Theory]
        [InlineData(JsonSchemaType.Array, "array")]
        [InlineData(JsonSchemaType.String, "string")]
        [InlineData(JsonSchemaType.Number, "number")]
        [InlineData(JsonSchemaType.Integer, "integer")]
        [InlineData(JsonSchemaType.Boolean, "boolean")]
        [InlineData(JsonSchemaType.Null, "null")]
        public async Task SerializeUnevaluatedPropertiesFalseNotEmittedForNonObjectType(JsonSchemaType nonObjectType, string typeName)
        {
            var expected = $@"{{ ""type"": ""{typeName}"" }}";
            // Given - unevaluatedProperties should not be emitted when type is explicitly set to a non-object type
            var schema = new OpenApiSchema
            {
                Type = nonObjectType,
                UnevaluatedProperties = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(JsonSchemaType.Array, "array")]
        [InlineData(JsonSchemaType.String, "string")]
        [InlineData(JsonSchemaType.Number, "number")]
        [InlineData(JsonSchemaType.Integer, "integer")]
        [InlineData(JsonSchemaType.Boolean, "boolean")]
        [InlineData(JsonSchemaType.Null, "null")]
        public async Task SerializeUnevaluatedPropertiesSchemaNotEmittedForNonObjectType(JsonSchemaType nonObjectType, string typeName)
        {
            var expected = $@"{{ ""type"": ""{typeName}"" }}";
            // Given - unevaluatedProperties schema should not be emitted when type is explicitly set to a non-object type
            var schema = new OpenApiSchema
            {
                Type = nonObjectType,
                UnevaluatedPropertiesSchema = new OpenApiSchema { Type = JsonSchemaType.String }
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, JsonSchemaType.Array)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, JsonSchemaType.String)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, JsonSchemaType.Array)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, JsonSchemaType.String)]
        public async Task SerializeUnevaluatedPropertiesNotEmittedAsExtensionForNonObjectType(OpenApiSpecVersion version, JsonSchemaType nonObjectType)
        {
            // Given - unevaluatedProperties should not be emitted as extension when type is a non-object type
            var schema = new OpenApiSchema
            {
                Type = nonObjectType,
                UnevaluatedProperties = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then - should not contain unevaluatedProperties extension
            var parsed = JsonNode.Parse(actual)!.AsObject();
            Assert.False(parsed.ContainsKey(OpenApiConstants.UnevaluatedPropertiesExtension));
        }

        [Fact]
        public async Task SerializeUnevaluatedPropertiesFalseStillEmittedForObjectType()
        {
            var expected = @"{ ""type"": ""object"", ""unevaluatedProperties"": false }";
            // Given - unevaluatedProperties should still be emitted for object type
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                UnevaluatedProperties = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeUnevaluatedPropertiesFalseStillEmittedWhenTypeNotSet()
        {
            var expected = @"{ ""unevaluatedProperties"": false }";
            // Given - unevaluatedProperties should still be emitted when type is not explicitly set
            var schema = new OpenApiSchema
            {
                UnevaluatedProperties = false
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        // PatternProperties tests
        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializePatternPropertiesAsKeywordInV31AndV32(OpenApiSpecVersion version)
        {
            var expected = @"{ ""patternProperties"": { ""^[a-z]+"": { ""type"": ""string"" } } }";
            // Given - patternProperties should be emitted as a standard keyword in v3.1+
            var schema = new OpenApiSchema
            {
                PatternProperties = new Dictionary<string, IOpenApiSchema>
                {
                    ["^[a-z]+"] = new OpenApiSchema { Type = JsonSchemaType.String }
                }
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializePatternPropertiesAsExtensionInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = @"{ ""x-jsonschema-patternProperties"": { ""^[a-z]+"": { ""type"": ""string"" } } }";
            // Given - patternProperties should be emitted as extension in versions < 3.1
            var schema = new OpenApiSchema
            {
                PatternProperties = new Dictionary<string, IOpenApiSchema>
                {
                    ["^[a-z]+"] = new OpenApiSchema { Type = JsonSchemaType.String }
                }
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializeExamplesAsExtensionInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = """
                {
                  "x-jsonschema-examples": [
                    "example value",
                    42
                  ]
                }
                """;
            var schema = new OpenApiSchema
            {
                Examples =
                [
                    JsonValue.Create("example value")!,
                    JsonValue.Create(42)!
                ]
            };

            var actual = await schema.SerializeAsJsonAsync(version);

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2)]
        public async Task SerializeExamplesAsJsonSchemaKeywordInV31AndLater(OpenApiSpecVersion version)
        {
            var expected = """
                {
                  "examples": [
                    "example value",
                    42
                  ]
                }
                """;
            var schema = new OpenApiSchema
            {
                Examples =
                [
                    JsonValue.Create("example value")!,
                    JsonValue.Create(42)!
                ]
            };

            var actual = await schema.SerializeAsJsonAsync(version);

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        public async Task SerializeEmptyPatternPropertiesNotEmittedInEarlierVersions(OpenApiSpecVersion version)
        {
            var expected = @"{ }";
            // Given - empty patternProperties should not emit extension
            var schema = new OpenApiSchema
            {
                PatternProperties = new Dictionary<string, IOpenApiSchema>()
            };

            // When
            var actual = await schema.SerializeAsJsonAsync(version);

            // Then
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public void DeserializePatternPropertiesExtensionInV2AssignsPatternPropertiesProperty()
        {
            // Given - a V2 document with x-jsonschema-patternProperties extension in a definition
            var jsonContent = """
            {
              "swagger": "2.0",
              "info": { "title": "Test", "version": "1.0" },
              "paths": {},
              "definitions": {
                "TestSchema": {
                  "type": "object",
                  "x-jsonschema-patternProperties": {
                    "^[a-z]+": { "type": "string" }
                  }
                }
              }
            }
            """;

            // When
            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            // Then
            Assert.Empty(readResult.Diagnostic.Errors);
            var schema = readResult.Document.Components.Schemas["TestSchema"];
            Assert.NotNull(schema);
            Assert.NotNull(schema.PatternProperties);
            Assert.Single(schema.PatternProperties);
            Assert.True(schema.PatternProperties.ContainsKey("^[a-z]+"));
            Assert.Equal(JsonSchemaType.String, schema.PatternProperties["^[a-z]+"].Type);
            // Extension should NOT be present on the schema (it was consumed)
            Assert.True(schema.Extensions is null || !schema.Extensions.ContainsKey("x-jsonschema-patternProperties"));
        }

        [Fact]
        public void DeserializePatternPropertiesExtensionInV3AssignsPatternPropertiesProperty()
        {
            // Given - a V3 document with x-jsonschema-patternProperties extension in a component schema
            var jsonContent = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test", "version": "1.0" },
              "paths": {},
              "components": {
                "schemas": {
                  "TestSchema": {
                    "type": "object",
                    "x-jsonschema-patternProperties": {
                      "^[a-z]+": { "type": "string" }
                    }
                  }
                }
              }
            }
            """;

            // When
            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            // Then
            Assert.Empty(readResult.Diagnostic.Errors);
            var schema = readResult.Document.Components.Schemas["TestSchema"];
            Assert.NotNull(schema);
            Assert.NotNull(schema.PatternProperties);
            Assert.Single(schema.PatternProperties);
            Assert.True(schema.PatternProperties.ContainsKey("^[a-z]+"));
            Assert.Equal(JsonSchemaType.String, schema.PatternProperties["^[a-z]+"].Type);
            // Extension should NOT be present on the schema (it was consumed)
            Assert.True(schema.Extensions is null || !schema.Extensions.ContainsKey("x-jsonschema-patternProperties"));
        }

        [Fact]
        public void DeserializeContainsExtensionsInV3AssignsContainsProperties()
        {
            var jsonContent = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Test", "version": "1.0" },
              "paths": {},
              "components": {
                "schemas": {
                  "TestSchema": {
                    "type": "array",
                    "x-jsonschema-contains": {
                      "type": "string"
                    },
                    "x-jsonschema-maxContains": 5,
                    "x-jsonschema-minContains": 1
                  }
                }
              }
            }
            """;

            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            Assert.Empty(readResult.Diagnostic.Errors);
            var schema = readResult.Document.Components.Schemas["TestSchema"];
            var missingProperties = Assert.IsAssignableFrom<IOpenApiSchemaMissingProperties>(schema);
            Assert.Equal(JsonSchemaType.String, missingProperties.Contains?.Type);
            Assert.Equal((uint?)5, missingProperties.MaxContains);
            Assert.Equal((uint?)1, missingProperties.MinContains);
            Assert.True(schema.Extensions is null || !schema.Extensions.ContainsKey(OpenApiConstants.ContainsExtension));
            Assert.True(schema.Extensions is null || !schema.Extensions.ContainsKey(OpenApiConstants.MaxContainsExtension));
            Assert.True(schema.Extensions is null || !schema.Extensions.ContainsKey(OpenApiConstants.MinContainsExtension));
        }

        [Fact]
        public async Task SerializeNullableEnumWith3_0()
        {
            // https://spec.openapis.org/oas/v3.0.4.html#fixed-fields-20
            // Documentation for nullable states:
            // This keyword only takes effect if type is explicitly defined within the same Schema Object.
            // So, we want to ensure that we emit the type property if we will be adding nullable property.
            // In addition, we need to still keep 'null' in the enum array.
            // Otherwise, validators will consider null as invalid even if nullable is set to true.
            // It's unclear if it's an issue of the validators or not, but it's safer to do it that way.
            var schema = CreateNullableEnumSchema();
            var result = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            var expected = """
                {
                  "oneOf": [
                    {
                      "enum": [
                        null
                      ]
                    },
                    {
                      "enum": [
                        "A",
                        "B"
                      ]
                    }
                  ]
                }
                """;

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(result)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeNullableEnumWith3_1_And_Later(OpenApiSpecVersion version)
        {
            var schema = CreateNullableEnumSchema();
            var result = await schema.SerializeAsJsonAsync(version);
            var expected = """
                {
                  "oneOf": [
                    {
                      "type": "null"
                    },
                    {
                      "enum": [
                        "A",
                        "B"
                      ]
                    }
                  ]
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(result)));
        }

        [Fact]
        public async Task SerializeNullableTypeWith3_0()
        {
            var schema = CreateTypeNullSchema();
            var result = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            var expected = """
                {
                  "enum": [
                    null
                  ]
                }
                """;

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(result)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeNullableTypeWith3_1_And_Later(OpenApiSpecVersion version)
        {
            var schema = CreateTypeNullSchema();
            var result = await schema.SerializeAsJsonAsync(version);
            var expected = """
                {
                  "type": "null"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(result)));
        }

        private OpenApiSchema CreateNullableEnumSchema()
        {
            var schema = new OpenApiSchema();
            schema.OneOf ??= [];
            schema.OneOf.Add(new OpenApiSchema() { Type = JsonSchemaType.Null });
            schema.OneOf.Add(new OpenApiSchema()
            {
                Enum = new List<JsonNode>
                {
                    JsonValue.Create("A"),
                    JsonValue.Create("B")
                }
            });
            return schema;
        }

        private OpenApiSchema CreateTypeNullSchema()
        {
            var schema = new OpenApiSchema();
            schema.Type = JsonSchemaType.Null;
            return schema;
        }

        internal class SchemaVisitor : OpenApiVisitorBase
        {
            public List<string> Titles = new();

            public override void Visit(IOpenApiSchema schema)
            {
                Titles.Add(schema.Title);
            }
        }
    }
}
