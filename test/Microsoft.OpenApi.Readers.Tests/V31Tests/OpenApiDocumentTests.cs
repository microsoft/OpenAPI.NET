using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;
using VerifyXunit;
using System;
using System.Net.Http;

#pragma warning disable CS0618

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiDocument/";

        [Fact]
        public void ParseDocumentWithMalformedReferenceShouldYieldExpectedDiagnostic()
        {
            var result = OpenApiDocument.Parse(
                """
                openapi: 3.1.1
                info:
                  title: test
                  version: 1.0.0
                paths:
                  /test:
                    get:
                      responses:
                        '200':
                          description: successful operation
                          content:
                            application/json:
                              schema:
                                $ref: '#components/schemas/Item'
                components:
                  schemas:
                    Item:
                      type: object
                      properties:
                        id:
                          type: integer
                """,
                OpenApiConstants.Yaml,
                SettingsFixture.ReaderSettings);

            var error = Assert.Single(result.Diagnostic.Errors);
            Assert.Equal("The reference string '#components/schemas/Item' has invalid format.", error.Message);
        }

        [Fact]
        public async Task ParseDocumentWithWebhooksShouldSucceed()
        {
            // Arrange and Act
            var actual = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "documentWithWebhooks.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var petSchema = new OpenApiSchemaReference("petSchema", actual.Document);

            var newPetSchema = new OpenApiSchemaReference("newPetSchema", actual.Document);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>()
                {
                    ["petSchema"] =  new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "id",
                            "name"
                        },
                        DependentRequired = new Dictionary<string, HashSet<string>>
                        {
                            { "tag", new HashSet<string> { "category" } }
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["category"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String,
                            },
                        }
                    },
                    ["newPetSchema"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "name"
                        },
                        DependentRequired = new Dictionary<string, HashSet<string>>
                        {
                            { "tag", new HashSet<string> { "category" } }
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["category"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String,
                            },
                        }
                    }
                }
            };

            var expected = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Webhook Example"
                },
                Webhooks = new Dictionary<string, IOpenApiPathItem>
                {
                    ["pets"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Parameters =
                                    [
                                        new OpenApiParameter
                                        {
                                            Name = "tags",
                                            In = ParameterLocation.Query,
                                            Description = "tags to filter by",
                                            Required = false,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.String
                                                }
                                            }
                                        },
                                        new OpenApiParameter
                                        {
                                            Name = "limit",
                                            In = ParameterLocation.Query,
                                            Description = "maximum number of results to return",
                                            Required = false,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Integer,
                                                Format = "int32"
                                            }
                                        }
                                    ],
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Post] = new OpenApiOperation
                            {
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Information about a new pet in the system",
                                    Required = true,
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = newPetSchema
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "Return a 200 status to indicate that the data was received successfully",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Components = components
            };

            // Assert            
            Assert.Equivalent(new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1, Format = OpenApiConstants.Yaml }, actual.Diagnostic);
            OpenApiTestAssert.Equivalent(expected, actual.Document, "Workspace", "BaseUri");
        }

        [Fact]
        public async Task ParseDocumentsWithReusablePathItemInWebhooksSucceeds()
        {
            // Arrange && Act
            var actual = await OpenApiDocument.LoadAsync("V31Tests/Samples/OpenApiDocument/documentWithReusablePaths.yaml", SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>()
                {
                    ["petSchema"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "id",
                            "name"
                        },
                        DependentRequired = new Dictionary<string, HashSet<string>>
                        {
                            { "tag", new HashSet<string> { "category" } }
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["category"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String,
                            },
                        }
                    },
                    ["newPetSchema"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "name"
                        },
                        DependentRequired = new Dictionary<string, HashSet<string>>
                        {
                            { "tag", new HashSet<string> { "category" } }
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["category"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String,
                            },
                        }
                    }
                }
            };

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema = new OpenApiSchemaReference("petSchema", actual.Document);

            var newPetSchema = new OpenApiSchemaReference("newPetSchema", actual.Document);

            components.PathItems = new Dictionary<string, IOpenApiPathItem>
            {
                ["pets"] = new OpenApiPathItem
                {
                    Operations = new()
                    {
                        [HttpMethod.Get] = new OpenApiOperation
                        {
                            Description = "Returns all pets from the system that the user has access to",
                            OperationId = "findPets",
                            Parameters =
                            [
                                new OpenApiParameter
                                {
                                    Name = "tags",
                                    In = ParameterLocation.Query,
                                    Description = "tags to filter by",
                                    Required = false,
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.String
                                        }
                                    }
                                },
                                new OpenApiParameter
                                {
                                    Name = "limit",
                                    In = ParameterLocation.Query,
                                    Description = "maximum number of results to return",
                                    Required = false,
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Format = "int32"
                                    }
                                }
                            ],
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "pet response",
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = petSchema
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = petSchema
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        [HttpMethod.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Information about a new pet in the system",
                                Required = true,
                                Content = new Dictionary<string, OpenApiMediaType>()
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = newPetSchema
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Return a 200 status to indicate that the data was received successfully",
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = petSchema
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var expected = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Webhook Example",
                    Version = "1.0.0"
                },
                JsonSchemaDialect = new Uri("http://json-schema.org/draft-07/schema#"),
                Webhooks = new Dictionary<string, IOpenApiPathItem>
                {
                    ["pets"] = components.PathItems["pets"]
                },
                Components = components
            };

            // Assert
            OpenApiTestAssert.Equivalent(expected, actual.Document, "Workspace", "BaseUri");
            Assert.Equivalent(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1, Format = OpenApiConstants.Yaml }, actual.Diagnostic);
        }

        [Fact]
        public async Task ParseDocumentWithExampleInSchemaShouldSucceed()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = false });

            // Act
            var actual = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "docWithExample.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            actual.Document.SerializeAsV31(writer);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ParseDocumentWithPatternPropertiesInSchemaWorks()
        {
            // Arrange and Act
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "docWithPatternPropertiesInSchema.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var actualSchema = result.Document.Paths["/example"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["prop1"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["prop2"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["prop3"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                },
                PatternProperties = new Dictionary<string, IOpenApiSchema>()
                {
                    ["^x-.*$"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };

            // Serialization
            var mediaType = result.Document.Paths["/example"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"];

            var expectedMediaType = @"schema:
  patternProperties:
    ^x-.*$:
      type: string
  type: object
  properties:
    prop1:
      type: string
    prop2:
      type: string
    prop3:
      type: string";

            var actualMediaType = await mediaType.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equivalent(expectedSchema, actualSchema);
            Assert.Equal(expectedMediaType.MakeLineBreaksEnvironmentNeutral(), actualMediaType.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task ParseDocumentWithReferenceByIdGetsResolved()
        {
            // Arrange and Act
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "docWithReferenceById.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            var responseSchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;
            var requestBodySchema = result.Document.Paths["/resource"].Operations[HttpMethod.Post].RequestBody.Content["application/json"].Schema;
            var parameterSchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Parameters[0].Schema;

            // Assert
            Assert.Equal(JsonSchemaType.Object, responseSchema.Type);
            Assert.Equal(JsonSchemaType.Object, requestBodySchema.Type);
            Assert.Equal(JsonSchemaType.String, parameterSchema.Type);
        }

        [Fact]
        public async Task ExternalDocumentDereferenceToOpenApiDocumentUsingJsonPointerWorks()
        {
            // Arrange
            var documentName = "externalRefByJsonPointer.yaml";
            var path = Path.Join(Directory.GetCurrentDirectory(), SampleFolderPath, documentName);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, documentName), settings, token: TestContext.Current.CancellationToken);
            var responseSchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            var externalResourceUri = new Uri(
                "file://" + 
                Path.Join(Path.GetFullPath(SampleFolderPath), 
                "externalResource.yaml#/components/schemas/todo")).AbsoluteUri;

            Assert.True(result.Document.Workspace.Contains(externalResourceUri));
            Assert.Equal(2, responseSchema.Properties.Count); // reference has been resolved
        }

        [Fact]
        public async Task ParseExternalDocumentDereferenceToOpenApiDocumentByIdWorks()
        {
            // Arrange
            var path = Path.Join(Directory.GetCurrentDirectory(), SampleFolderPath);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "externalRefById.yaml"), settings, token: TestContext.Current.CancellationToken);
            var doc2 = (await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "externalResource.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken)).Document;

            var requestBodySchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Parameters[0].Schema;
            result.Document.Workspace.RegisterComponents(doc2);

            // Assert
            Assert.Equal(2, requestBodySchema.Properties.Count); // reference has been resolved
        }

        [Fact]
        public async Task ParseDocumentWith31PropertiesWorks()
        {
            var path = Path.Join(SampleFolderPath, "documentWith31Properties.yaml");
            var doc = (await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken)).Document;
            var outputStringWriter = new StringWriter();
            doc.SerializeAsV31(new OpenApiYamlWriter(outputStringWriter));
            await outputStringWriter.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual);
        }

        [Fact]
        public async Task ParseDocumentWithEmptyTagsWorks()
        {
            var path = Path.Join(SampleFolderPath, "documentWithEmptyTags.json");
            var doc = (await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken)).Document;

            Assert.Null(doc.Paths["/groups"].Operations[HttpMethod.Get].Tags);
        }
        [Fact]
        public async Task DocumentWithSchemaResultsInWarning()
        {
            var path = Path.Join(SampleFolderPath, "documentWithSchema.json");
            var (doc, diag) = await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            Assert.NotNull(doc);
            Assert.NotNull(diag);
            Assert.Empty(diag.Errors);
            Assert.Single(diag.Warnings);
            Assert.StartsWith("$schema is not a valid property", diag.Warnings[0].Message);
        }

        [Fact]
        public void ParseEmptyMemoryStreamThrowsAnArgumentException()
        {
            Assert.Throws<ArgumentException>(() => OpenApiDocument.Load(new MemoryStream()));
        }

        [Fact]
        public async Task ValidateReferencedExampleInSchemaWorks()
        {
            // Arrange && Act
            var path = Path.Join(SampleFolderPath, "docWithReferencedExampleInSchemaWorks.yaml");
            var result = await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var actualSchemaExample = result.Document.Components.Schemas["DiffCreatedEvent"].Properties["updatedAt"].Example;
            var targetSchemaExample = result.Document.Components.Schemas["Timestamp"].Example;

            // Assert
            Assert.Equal(targetSchemaExample, actualSchemaExample);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Empty(result.Diagnostic.Warnings);            
        }

        [Fact]
        public void LoadDocumentWithBooleanSchemaShouldNotThrowNullReferenceException()
        {
            // Arrange - OpenAPI 3.1 with a boolean schema in components/schemas (spec-valid per JSON Schema 2020-12)
            var bytes = "{\"openapi\":\"3.1.0\",\"components\":{\"schemas\":{\"X\":true}}}"u8.ToArray();
            using var ms = new MemoryStream(bytes);

            // Act & Assert - should not throw NullReferenceException
            var exception = Record.Exception(() => OpenApiDocument.Load(ms, format: null, new OpenApiReaderSettings()));
            
            // The parser should handle the boolean schema gracefully
            // Either accepting it or surfacing a structured diagnostic, but not throwing NullReferenceException
            if (exception != null)
            {
                Assert.IsNotType<NullReferenceException>(exception);
            }
        }
    }
}
