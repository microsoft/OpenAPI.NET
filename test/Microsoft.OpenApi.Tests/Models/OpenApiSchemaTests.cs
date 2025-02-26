// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;
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
            Maximum = 42,
            ExclusiveMinimum = 10,
            Default = 15,
            Type = JsonSchemaType.Integer | JsonSchemaType.Null,

            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            },
            Annotations = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } }
        };

        public static readonly OpenApiSchema AdvancedSchemaObject = new()
        {
            Title = "title1",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["property1"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
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
            AllOf = new List<IOpenApiSchema>
            {
                new OpenApiSchema()
                {
                    Title = "title2",
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["property3"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
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
            },
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
            Maximum = 42,
            ExclusiveMinimum = 10,
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
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["property1"] = new OpenApiSchema()
                {
                    Required = new HashSet<string> { "property3" },
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, IOpenApiSchema>
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

        [Fact]
        public async Task SerializeBasicSchemaAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = await BasicSchema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

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
                OneOf = new List<IOpenApiSchema>
                {
                    new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "decimal"
                    },
                    new OpenApiSchema() { Type = JsonSchemaType.String },
                }
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
        public void OpenApiSchemaCopyConstructorWithAnnotationsSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Annotations = new Dictionary<string, object>
                {
                    ["key1"] = "value1",
                    ["key2"] = 2
                }
            };

            var actualSchema = baseSchema.CreateShallowCopy();

            Assert.Equal(baseSchema.Annotations["key1"], actualSchema.Annotations["key1"]);

            baseSchema.Annotations["key1"] = "value2";

            Assert.NotEqual(baseSchema.Annotations["key1"], actualSchema.Annotations["key1"]);
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
                Extensions =
                {
                    { "x-myextension", new OpenApiAny(42) }
                }
            };

            // Act && Assert
            var schemaCopy = schema.CreateShallowCopy() as OpenApiSchema;
            Assert.Single(schemaCopy.Extensions);

            // Act && Assert
            schemaCopy.Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "x-myextension" , new OpenApiAny(40) }
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
                        Parameters = new[]
                        {
                            new OpenApiParameter()
                            {
                                Name = "foo",
                                In = ParameterLocation.Query,
                                Schema = outerSchema,
                            }
                        }
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
