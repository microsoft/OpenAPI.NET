// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models.References
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaReferenceTests
    {
        [Fact]
        public void SchemaReferenceWithAnnotationsShouldWork()
        {
            // Arrange
            var workingDocument = new OpenApiDocument()
            {
                Components = new OpenApiComponents(),
            };
            const string referenceId = "targetSchema";
            var targetSchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Title = "Target Title",
                Description = "Target Description",
                ReadOnly = false,
                WriteOnly = false,
                Deprecated = false,
                Default = JsonValue.Create("target default"),
                Examples = new List<JsonNode> { JsonValue.Create("target example") },
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["prop1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };
            workingDocument.Components.Schemas = new Dictionary<string, IOpenApiSchema>()
            {
                [referenceId] = targetSchema
            };
            workingDocument.Workspace.RegisterComponents(workingDocument);

            // Act
            var schemaReference = new OpenApiSchemaReference(referenceId, workingDocument)
            {
                Title = "Override Title",
                Description = "Override Description",
                ReadOnly = true,
                WriteOnly = true,
                Deprecated = true,
                Default = JsonValue.Create("override default"),
                Examples = new List<JsonNode> { JsonValue.Create("override example") },
            };

            // Assert
            Assert.Equal("Override Title", schemaReference.Title);
            Assert.Equal("Override Description", schemaReference.Description);
            Assert.True(schemaReference.ReadOnly);
            Assert.True(schemaReference.WriteOnly);
            Assert.True(schemaReference.Deprecated);
            Assert.Equal("override default", schemaReference.Default?.GetValue<string>());
            Assert.Single(schemaReference.Examples);
            Assert.Equal("override example", schemaReference.Examples.First()?.GetValue<string>());
        }

        [Fact]
        public void SchemaReferenceWithoutAnnotationsShouldFallbackToTarget()
        {
            // Arrange
            var workingDocument = new OpenApiDocument()
            {
                Components = new OpenApiComponents(),
            };
            const string referenceId = "targetSchema";
            var targetSchema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Title = "Target Title",
                Description = "Target Description",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("target default"),
                Examples = new List<JsonNode> { JsonValue.Create("target example") },
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["prop1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };
            workingDocument.Components.Schemas = new Dictionary<string, IOpenApiSchema>()
            {
                [referenceId] = targetSchema
            };
            workingDocument.Workspace.RegisterComponents(workingDocument);

            // Act
            var schemaReference = new OpenApiSchemaReference(referenceId, workingDocument);

            // Assert - should fallback to target values
            Assert.Equal("Target Title", schemaReference.Title);
            Assert.Equal("Target Description", schemaReference.Description);
            Assert.True(schemaReference.ReadOnly);
            Assert.False(schemaReference.WriteOnly);
            Assert.True(schemaReference.Deprecated);
            Assert.Equal("target default", schemaReference.Default?.GetValue<string>());
            Assert.Single(schemaReference.Examples);
            Assert.Equal("target example", schemaReference.Examples.First()?.GetValue<string>());
        }

        [Fact]
        public void SchemaReferenceExposesMissingPropertiesFromTarget()
        {
            var workingDocument = new OpenApiDocument
            {
                Components = new OpenApiComponents(),
            };
            const string referenceId = "targetSchema";
            workingDocument.Components.Schemas = new Dictionary<string, IOpenApiSchema>
            {
                [referenceId] = new OpenApiSchema
                {
                    Anchor = "root",
                    UnevaluatedProperties = false,
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
                }
            };
            workingDocument.Workspace.RegisterComponents(workingDocument);

            var schemaReference = new OpenApiSchemaReference(referenceId, workingDocument);
            var missingProperties = Assert.IsAssignableFrom<IOpenApiSchemaMissingProperties>(schemaReference);

            Assert.Equal("root", missingProperties.Anchor);
            Assert.False(missingProperties.UnevaluatedProperties);
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

        [Fact]
        public void SchemaReferenceWithKeywordSiblingsShouldOverrideTargetValues()
        {
            var workingDocument = new OpenApiDocument
            {
                Components = new OpenApiComponents(),
            };
            const string referenceId = "targetSchema";
            workingDocument.Components.Schemas = new Dictionary<string, IOpenApiSchema>
            {
                [referenceId] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Format = "target-format",
                    MaxLength = 20,
                    Required = new HashSet<string> { "target" },
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["target"] = new OpenApiSchema { Type = JsonSchemaType.String }
                    },
                    AdditionalPropertiesAllowed = true,
                    ContentEncoding = "gzip",
                    If = new OpenApiSchema { Required = new HashSet<string> { "target" } }
                }
            };
            workingDocument.Workspace.RegisterComponents(workingDocument);

            var schemaReference = new OpenApiSchemaReference(referenceId, workingDocument)
            {
                Type = JsonSchemaType.String,
                Format = "reference-format",
                MaxLength = 10,
                Required = new HashSet<string> { "reference" },
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["reference"] = new OpenApiSchema { Type = JsonSchemaType.Integer }
                },
                AdditionalPropertiesAllowed = false,
                ContentEncoding = "base64",
                If = new OpenApiSchema { Required = new HashSet<string> { "reference" } }
            };

            Assert.Equal(JsonSchemaType.String, schemaReference.Type);
            Assert.Equal("reference-format", schemaReference.Format);
            Assert.Equal(10, schemaReference.MaxLength);
            Assert.NotNull(schemaReference.Required);
            Assert.Contains("reference", schemaReference.Required);
            Assert.Equal(JsonSchemaType.Integer, schemaReference.Properties?["reference"].Type);
            Assert.False(schemaReference.AdditionalPropertiesAllowed);
            Assert.Equal("base64", schemaReference.ContentEncoding);
            Assert.NotNull(schemaReference.If?.Required);
            Assert.Contains("reference", schemaReference.If.Required);
        }

        [Fact]
        public void ParseSchemaReferenceWithKeywordSiblingsWorks()
        {
            var jsonContent = @"{
  ""openapi"": ""3.1.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""OK"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Pet"",
                  ""type"": ""string"",
                  ""format"": ""uuid"",
                  ""maxLength"": 36,
                  ""required"": [""id""],
                  ""properties"": {
                    ""id"": {
                      ""type"": ""string""
                    }
                  },
                  ""additionalProperties"": false,
                  ""contentEncoding"": ""base64"",
                  ""if"": {
                    ""required"": [""id""]
                  }
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Pet"": {
        ""type"": ""object"",
        ""format"": ""target-format"",
        ""maxLength"": 10,
        ""additionalProperties"": true
      }
    }
  }
}";

            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            Assert.Empty(readResult.Diagnostic.Errors);
            var schemaReference = Assert.IsType<OpenApiSchemaReference>(readResult.Document?.Paths["/test"].Operations[HttpMethod.Get]
                .Responses["200"].Content["application/json"].Schema);
            Assert.Equal(JsonSchemaType.String, schemaReference.Type);
            Assert.Equal("uuid", schemaReference.Format);
            Assert.Equal(36, schemaReference.MaxLength);
            Assert.NotNull(schemaReference.Required);
            Assert.Contains("id", schemaReference.Required);
            Assert.Equal(JsonSchemaType.String, schemaReference.Properties?["id"].Type);
            Assert.False(schemaReference.AdditionalPropertiesAllowed);
            Assert.Equal("base64", schemaReference.ContentEncoding);
            Assert.NotNull(schemaReference.If?.Required);
            Assert.Contains("id", schemaReference.If.Required);
        }

        [Fact]
        public void ParseV30SchemaReferenceWithJsonSchemaExtensionSiblingsWorks()
        {
            var jsonContent = @"{
  ""openapi"": ""3.0.4"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""OK"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Pet"",
                  ""x-jsonschema-type"": ""string"",
                  ""x-jsonschema-format"": ""uuid"",
                  ""x-jsonschema-maxLength"": 36,
                  ""x-jsonschema-contentEncoding"": ""base64""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Pet"": {
        ""type"": ""object"",
        ""format"": ""target-format"",
        ""maxLength"": 10
      }
    }
  }
}";

            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            Assert.Empty(readResult.Diagnostic.Errors);
            var schemaReference = Assert.IsType<OpenApiSchemaReference>(readResult.Document?.Paths["/test"].Operations[HttpMethod.Get]
                .Responses["200"].Content["application/json"].Schema);
            Assert.Equal(JsonSchemaType.String, schemaReference.Type);
            Assert.Equal("uuid", schemaReference.Format);
            Assert.Equal(36, schemaReference.MaxLength);
            Assert.Equal("base64", schemaReference.ContentEncoding);
        }

        [Fact]
        public void ParseV30SchemaReferenceWithJsonSchemaExtensionKeywordSiblingsFromJsonSchema202012Works()
        {
            var jsonContent = @"{
  ""openapi"": ""3.0.4"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""OK"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Pet"",
                  ""x-jsonschema-$id"": ""https://example.com/schemas/pet-response"",
                  ""x-jsonschema-$schema"": ""https://json-schema.org/draft/2020-12/schema"",
                  ""x-jsonschema-$comment"": ""reference comment"",
                  ""x-jsonschema-$vocabulary"": {
                    ""https://json-schema.org/draft/2020-12/vocab/core"": true
                  },
                  ""x-jsonschema-$defs"": {
                    ""LocalPet"": {
                      ""type"": ""object""
                    }
                  },
                  ""x-jsonschema-$anchor"": ""petResponse"",
                  ""x-jsonschema-$dynamicRef"": ""#pet"",
                  ""x-jsonschema-$dynamicAnchor"": ""pet""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Pet"": {
        ""type"": ""object""
      }
    }
  }
}";

            var readResult = OpenApiDocument.Parse(jsonContent, "json");

            Assert.Empty(readResult.Diagnostic.Errors);
            var schemaReference = Assert.IsType<OpenApiSchemaReference>(readResult.Document?.Paths["/test"].Operations[HttpMethod.Get]
                .Responses["200"].Content["application/json"].Schema);
            Assert.Equal("https://example.com/schemas/pet-response", schemaReference.Id);
            Assert.Equal(new Uri("https://json-schema.org/draft/2020-12/schema"), schemaReference.Schema);
            Assert.Equal("reference comment", schemaReference.Comment);
            Assert.NotNull(schemaReference.Vocabulary);
            Assert.True(schemaReference.Vocabulary["https://json-schema.org/draft/2020-12/vocab/core"]);
            Assert.NotNull(schemaReference.Definitions);
            Assert.Equal(JsonSchemaType.Object, schemaReference.Definitions["LocalPet"].Type);
            Assert.Equal("petResponse", schemaReference.Anchor);
            Assert.Equal("#pet", schemaReference.DynamicRef);
            Assert.Equal("pet", schemaReference.DynamicAnchor);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaReferenceAsV31JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Title = "Reference Title",
                Description = "Reference Description",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            reference.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaReferenceAsV32JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Title = "Reference Title",
                Description = "Reference Description",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            reference.SerializeAsV32(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange - Extensions should NOT appear in v3.0 output
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Title = "Reference Title",
                Description = "Reference Description",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            reference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaReferenceAsV2JsonWorks(bool produceTerseOutput)
        {
            // Arrange - Extensions should NOT appear in v2 output
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Title = "Reference Title",
                Description = "Reference Description",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") },
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            reference.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void ParseSchemaReferenceWithAnnotationsWorks()
        {
            // Arrange
            var jsonContent = @"{
  ""openapi"": ""3.1.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""OK"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Pet"",
                  ""title"": ""Pet Response Schema"",
                  ""description"": ""A pet object returned from the API"",
                  ""summary"": ""Pet Response"",
                  ""deprecated"": true,
                  ""readOnly"": true,
                  ""writeOnly"": false,
                  ""default"": {""name"": ""default pet""},
                  ""examples"": [{""name"": ""example pet""}]
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Pet"": {
        ""type"": ""object"",
        ""title"": ""Original Pet Title"",
        ""description"": ""Original Pet Description"",
        ""properties"": {
          ""name"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}";

            // Act
            var readResult = OpenApiDocument.Parse(jsonContent, "json");
            var document = readResult.Document;

            // Assert
            Assert.NotNull(document);
            Assert.Empty(readResult.Diagnostic.Errors);

            var schema = document.Paths["/test"].Operations[HttpMethod.Get]
                .Responses["200"].Content["application/json"].Schema;

            Assert.IsType<OpenApiSchemaReference>(schema);
            var schemaRef = (OpenApiSchemaReference)schema;

            // Test that reference annotations override target values
            Assert.Equal("Pet Response Schema", schemaRef.Title);
            Assert.Equal("A pet object returned from the API", schemaRef.Description);
            Assert.True(schemaRef.Deprecated);
            Assert.True(schemaRef.ReadOnly);
            Assert.False(schemaRef.WriteOnly);
            Assert.NotNull(schemaRef.Default);
            Assert.Single(schemaRef.Examples);

            // Test that target schema still has original values
            var targetSchema = schemaRef.Target;
            Assert.NotNull(targetSchema);
            Assert.Equal("Original Pet Title", targetSchema.Title);
            Assert.Equal("Original Pet Description", targetSchema.Description);
        }

        [Fact]
        public void ParseSchemaReferenceWithExtensionsWorks()
        {
            // Arrange
            var jsonContent = @"{
  ""openapi"": ""3.1.0"",
  ""info"": {
    ""title"": ""Test API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""OK"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Pet"",
                  ""description"": ""A pet object"",
                  ""x-custom-extension"": ""custom value"",
                  ""x-another-extension"": 42
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Pet"": {
        ""type"": ""object"",
        ""properties"": {
          ""name"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}";

            // Act
            var readResult = OpenApiDocument.Parse(jsonContent, "json");
            var document = readResult.Document;

            // Assert
            Assert.NotNull(document);
            Assert.Empty(readResult.Diagnostic.Errors);

            var schema = document.Paths["/test"].Operations[HttpMethod.Get]
                .Responses["200"].Content["application/json"].Schema;

            Assert.IsType<OpenApiSchemaReference>(schema);
            var schemaRef = (OpenApiSchemaReference)schema;

            // Test that reference-level extensions are parsed
            Assert.NotNull(schemaRef.Extensions);
            Assert.Contains("x-custom-extension", schemaRef.Extensions.Keys);
            Assert.Contains("x-another-extension", schemaRef.Extensions.Keys);
        }

        [Fact]
        public async Task SchemaReferenceCustomExtensionsNotWrittenInV30()
        {
            // Arrange
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Description = "Local description",
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = true });

            // Act
            reference.SerializeAsV3(writer);
            await writer.FlushAsync();
            var output = outputStringWriter.ToString();

            Assert.Equal(@"{""$ref"":""#/components/schemas/Pet"",""x-jsonschema-description"":""Local description""}", output);
        }

        [Fact]
        public async Task SchemaReferenceJsonSchema202012KeywordSiblingsWrittenAsExtensionsInV30()
        {
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Id = "https://example.com/schemas/pet-response",
                Schema = new Uri("https://json-schema.org/draft/2020-12/schema"),
                Comment = "reference comment",
                Vocabulary = new Dictionary<string, bool>
                {
                    ["https://json-schema.org/draft/2020-12/vocab/core"] = true
                },
                Definitions = new Dictionary<string, IOpenApiSchema>
                {
                    ["LocalPet"] = new OpenApiSchema { Type = JsonSchemaType.Object }
                },
                Anchor = "petResponse",
                DynamicRef = "#pet",
                DynamicAnchor = "pet"
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = true });

            reference.SerializeAsV3(writer);
            await writer.FlushAsync();

            Assert.Equal(@"{""$ref"":""#/components/schemas/Pet"",""x-jsonschema-$id"":""https://example.com/schemas/pet-response"",""x-jsonschema-$schema"":""https://json-schema.org/draft/2020-12/schema"",""x-jsonschema-$comment"":""reference comment"",""x-jsonschema-$vocabulary"":{""https://json-schema.org/draft/2020-12/vocab/core"":true},""x-jsonschema-$defs"":{""LocalPet"":{""type"":""object""}},""x-jsonschema-$anchor"":""petResponse"",""x-jsonschema-$dynamicRef"":""#pet"",""x-jsonschema-$dynamicAnchor"":""pet""}", outputStringWriter.ToString());
        }

        [Fact]
        public async Task SchemaReferenceExtensionsNotWrittenInV2()
        {
            // Arrange
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Description = "Local description",
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    ["x-custom"] = new JsonNodeExtension(JsonValue.Create("custom value"))
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = true });

            // Act
            reference.SerializeAsV2(writer);
            await writer.FlushAsync();
            var output = outputStringWriter.ToString();

            // Assert: In v2, ONLY $ref should appear - no description, no extensions
            Assert.Equal(@"{""$ref"":""#/definitions/Pet""}", output);
        }
    }
}
