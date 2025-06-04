// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
                Summary = "Reference Summary"
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
            Assert.Equal("Reference Summary", schemaReference.Summary);
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
            Assert.Null(schemaReference.Summary); // Summary has no target fallback
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
                Summary = "Reference Summary",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") }
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
        public async Task SerializeSchemaReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var reference = new OpenApiSchemaReference("Pet", null)
            {
                Title = "Reference Title",
                Description = "Reference Description",
                Summary = "Reference Summary",
                ReadOnly = true,
                WriteOnly = false,
                Deprecated = true,
                Default = JsonValue.Create("reference default"),
                Examples = new List<JsonNode> { JsonValue.Create("reference example") }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            reference.SerializeAsV3(writer);
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
            Assert.Equal("Pet Response", schemaRef.Summary);
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
    }
}