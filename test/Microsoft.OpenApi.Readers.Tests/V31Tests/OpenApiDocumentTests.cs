using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Writers;
using Xunit;
using VerifyXunit;
using Microsoft.OpenApi.Models.Interfaces;
using System;
using System.Net.Http;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiDocument/";

        [Fact]
        public async Task ParseDocumentWithWebhooksShouldSucceed()
        {
            // Arrange and Act
            var actual = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "documentWithWebhooks.yaml"), SettingsFixture.ReaderSettings);
            var petSchema = new OpenApiSchemaReference("petSchema", actual.Document);

            var newPetSchema = new OpenApiSchemaReference("newPetSchema", actual.Document);

            var components = new OpenApiComponents
            {
                Schemas = new()
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
                        Properties = new()
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
                        Properties = new()
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
                                        Content = new()
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
                                    Content = new()
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
                                        Content = new()
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
            Assert.Equivalent(new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1 }, actual.Diagnostic);
            actual.Document.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));
        }

        [Fact]
        public async Task ParseDocumentsWithReusablePathItemInWebhooksSucceeds()
        {
            // Arrange && Act
            var actual = await OpenApiDocument.LoadAsync("V31Tests/Samples/OpenApiDocument/documentWithReusablePaths.yaml", SettingsFixture.ReaderSettings);

            var components = new OpenApiComponents
            {
                Schemas = new()
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
                        Properties = new()
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
                        Properties = new()
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
                                    Content = new()
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
                                Content = new()
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
                                    Content = new()
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
            actual.Document.Should().BeEquivalentTo(expected, options => options
            .Excluding(x => x.Workspace)
            .Excluding(y => y.BaseUri));
            Assert.Equivalent(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1 }, actual.Diagnostic);
        }

        [Fact]
        public async Task ParseDocumentWithExampleInSchemaShouldSucceed()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = false });

            // Act
            var actual = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithExample.yaml"), SettingsFixture.ReaderSettings);
            actual.Document.SerializeAsV31(writer);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ParseDocumentWithPatternPropertiesInSchemaWorks()
        {
            // Arrange and Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithPatternPropertiesInSchema.yaml"), SettingsFixture.ReaderSettings);
            var actualSchema = result.Document.Paths["/example"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new()
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
                PatternProperties = new()
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

            var actualMediaType = await mediaType.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equivalent(expectedSchema, actualSchema);
            Assert.Equal(expectedMediaType.MakeLineBreaksEnvironmentNeutral(), actualMediaType.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task ParseDocumentWithReferenceByIdGetsResolved()
        {
            // Arrange and Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithReferenceById.yaml"), SettingsFixture.ReaderSettings);

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
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalRefByJsonPointer.yaml"), settings);
            var responseSchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            // Assert
            result.Document.Workspace.Contains("./externalResource.yaml");
            Assert.Equal(2, responseSchema.Properties.Count); // reference has been resolved
        }

        [Fact]
        public async Task ParseExternalDocumentDereferenceToOpenApiDocumentByIdWorks()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), SampleFolderPath);

            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                BaseUrl = new(path),
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalRefById.yaml"), settings);
            var doc2 = (await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "externalResource.yaml"), SettingsFixture.ReaderSettings)).Document;

            var requestBodySchema = result.Document.Paths["/resource"].Operations[HttpMethod.Get].Parameters[0].Schema;
            result.Document.Workspace.RegisterComponents(doc2);

            // Assert
            Assert.Equal(2, requestBodySchema.Properties.Count); // reference has been resolved
        }

        [Fact]
        public async Task ParseDocumentWith31PropertiesWorks()
        {
            var path = Path.Combine(SampleFolderPath, "documentWith31Properties.yaml");
            var doc = (await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings)).Document;
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
            var path = Path.Combine(SampleFolderPath, "documentWithEmptyTags.json");
            var doc = (await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings)).Document;

            doc.Paths["/groups"].Operations[HttpMethod.Get].Tags.Should().BeNull("Empty tags are ignored, so we should not have any tags");
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
            var path = Path.Combine(SampleFolderPath, "docWithReferencedExampleInSchemaWorks.yaml");
            var result = await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings);
            var actualSchemaExample = result.Document.Components.Schemas["DiffCreatedEvent"].Properties["updatedAt"].Example;
            var targetSchemaExample = result.Document.Components.Schemas["Timestamp"].Example;

            // Assert
            Assert.Equal(targetSchemaExample, actualSchemaExample);
            Assert.Empty(result.Diagnostic.Errors);
            Assert.Empty(result.Diagnostic.Warnings);            
        }
    }
}
