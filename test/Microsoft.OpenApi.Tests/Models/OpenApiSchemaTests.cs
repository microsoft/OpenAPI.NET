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
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = 15,
            Type = JsonSchemaType.Integer,

            Nullable = true,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            },
            Annotations = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } }
        };

        public static readonly OpenApiSchema AdvancedSchemaObject = new()
        {
            Title = "title1",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15
                        }
                    },
                },
                ["property4"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new()
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property7"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                },
            },
            Nullable = true,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        public static readonly OpenApiSchema AdvancedSchemaWithAllOf = new()
        {
            Title = "title1",
            AllOf = new List<OpenApiSchema>
            {
                new()
                {
                    Title = "title2",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15
                        }
                    },
                },
                new()
                {
                    Title = "title3",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property3"] = new()
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property4"] = new()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property5"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                    Nullable = true
                },
            },
            Nullable = true,
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
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = 15,
            Type = JsonSchemaType.Integer,

            Nullable = true,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        public static readonly OpenApiSchema AdvancedSchemaWithRequiredPropertiesObject = new()
        {
            Title = "title1",
            Required = new HashSet<string> { "property1" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new()
                {
                    Required = new HashSet<string> { "property3" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15,
                            ReadOnly = true
                        }
                    },
                    ReadOnly = true,
                },
                ["property4"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new()
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new()
                                {
                                    Type = JsonSchemaType.Boolean
                                }
                            }
                        },
                        ["property7"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MinLength = 2
                        }
                    },
                    ReadOnly = true,
                },
            },
            Nullable = true,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            }
        };

        [Fact]
        public void SerializeBasicSchemaAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaNumberAsV3JsonWorks()
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
                  "default": 15,
                  "nullable": true,
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = AdvancedSchemaNumber.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaObjectAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "title": "title1",
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
                  "nullable": true,
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = AdvancedSchemaObject.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaWithAllOfAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "title": "title1",
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
                      },
                      "nullable": true
                    }
                  ],
                  "nullable": true,
                  "externalDocs": {
                    "url": "http://example.com/externalDocs"
                  }
                }
                """;

            // Act
            var actual = AdvancedSchemaWithAllOf.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
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
            writer.Flush();

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
            writer.Flush();

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
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeAsV2ShouldSetFormatPropertyInParentSchemaIfPresentInChildrenSchema()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                OneOf = new List<OpenApiSchema>
                {
                    new()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "decimal"
                    },
                    new() { Type = JsonSchemaType.String },
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var openApiJsonWriter = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            // Serialize as V2
            schema.SerializeAsV2(openApiJsonWriter);
            openApiJsonWriter.Flush();

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
            expectedV2Schema.Should().BeEquivalentTo(v2Schema);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "date"
            };

            var actualSchema = new OpenApiSchema(baseSchema)
            {
                Nullable = true
            };

            Assert.Equal(JsonSchemaType.String, actualSchema.Type);
            Assert.Equal("date", actualSchema.Format);
            Assert.True(actualSchema.Nullable);
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

            var actualSchema = new OpenApiSchema(baseSchema);

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
            var schemaCopy = new OpenApiSchema(schema);

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
            var schemaCopy = new OpenApiSchema(schema);
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
            visitor.Titles.Count.Should().Be(2);
        }

        [Fact]
        public void SerializeSchemaWithUnrecognizedPropertiesWorks()
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
            var actual = schema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        internal class SchemaVisitor : OpenApiVisitorBase
        {
            public List<string> Titles = new();

            public override void Visit(OpenApiSchema schema)
            {
                Titles.Add(schema.Title);
            }
        }
    }
}
