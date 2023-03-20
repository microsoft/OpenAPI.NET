using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiDocument/";

        public T Clone<T>(T element) where T : IOpenApiSerializable
        {
            using var stream = new MemoryStream();
            IOpenApiWriter writer;
            var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
            writer = new OpenApiJsonWriter(streamWriter, new OpenApiJsonWriterSettings()
            {
                InlineLocalReferences = true
            });
            element.SerializeAsV31(writer);
            writer.Flush();
            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            var result = streamReader.ReadToEnd();
            return new OpenApiStringReader().ReadFragment<T>(result, OpenApiSpecVersion.OpenApi3_1, out OpenApiDiagnostic diagnostic4);
        }
        
        [Fact]
        public void ParseDocumentWithWebhooksShouldSucceed()
        {
            // Arrange and Act
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "documentWithWebhooks.yaml"));
            var actual = new OpenApiStreamReader().Read(stream, out var diagnostic);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["pet"] = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "pet",
                            HostDocument = actual
                        }
                    },
                    ["newPet"] = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string>
                            {
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "newPet",
                            HostDocument = actual
                        }
                    }
                }
            };

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema = Clone(components.Schemas["pet"]);

            petSchema.Reference = new OpenApiReference
            {
                Id = "pet",
                Type = ReferenceType.Schema,
                HostDocument = actual
            };

            var newPetSchema = Clone(components.Schemas["newPet"]);

            newPetSchema.Reference = new OpenApiReference
            {
                Id = "newPet",
                Type = ReferenceType.Schema,
                HostDocument = actual
            };

            var expected = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Webhook Example"
                },
                Webhooks = new Dictionary<string, OpenApiPathItem>
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
                                        {
                                            Name = "tags",
                                            In = ParameterLocation.Query,
                                            Description = "tags to filter by",
                                            Required = false,
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
                                                Items = new OpenApiSchema
                                                {
                                                    Type = "string"
                                                }
                                            }
                                        },
                                        new OpenApiParameter
                                        {
                                            Name = "limit",
                                            In = ParameterLocation.Query,
                                            Description = "maximum number of results to return",
                                            Required = false,
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer",
                                                Format = "int32"
                                            }
                                        }
                                    },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation
                            {
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Information about a new pet in the system",
                                    Required = true,
                                    Content = new Dictionary<string, OpenApiMediaType>
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
                                        Content = new Dictionary<string, OpenApiMediaType>
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
                },
                Components = components
            };

            // Assert
            //diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1 });
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ParseDocumentsWithReusablePathItemInWebhooksSucceeds()
        {
            // Arrange && Act
            using var stream = Resources.GetStream("V31Tests/Samples/OpenApiDocument/documentWithReusablePaths.yaml");
            var actual = new OpenApiStreamReader().Read(stream, out var context);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["pet"] = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "pet",
                            HostDocument = actual
                        }
                    },
                    ["newPet"] = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string>
                            {
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "newPet",
                            HostDocument = actual
                        }
                    }
                }
            };

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema = Clone(components.Schemas["pet"]);

            petSchema.Reference = new OpenApiReference
            {
                Id = "pet",
                Type = ReferenceType.Schema,
                HostDocument = actual
            };

            var newPetSchema = Clone(components.Schemas["newPet"]);

            newPetSchema.Reference = new OpenApiReference
            {
                Id = "newPet",
                Type = ReferenceType.Schema,
                HostDocument = actual
            };
            components.PathItems = new Dictionary<string, OpenApiPathItem>
            {
                ["/pets"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            Description = "Returns all pets from the system that the user has access to",
                            OperationId = "findPets",
                            Parameters = new List<OpenApiParameter>
                                {
                                    new OpenApiParameter
                                    {
                                        Name = "tags",
                                        In = ParameterLocation.Query,
                                        Description = "tags to filter by",
                                        Required = false,
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema
                                            {
                                                Type = "string"
                                            }
                                        }
                                    },
                                    new OpenApiParameter
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "maximum number of results to return",
                                        Required = false,
                                        Schema = new OpenApiSchema
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        }
                                    }
                                },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "pet response",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
                                                Items = petSchema
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
                                                Items = petSchema
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        [OperationType.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Information about a new pet in the system",
                                Required = true,
                                Content = new Dictionary<string, OpenApiMediaType>
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
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = petSchema
                                        },
                                    }
                                }
                            }
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.PathItem,
                        Id = "/pets",
                        HostDocument = actual
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
                JsonSchemaDialect = "http://json-schema.org/draft-07/schema#",
                Webhooks = components.PathItems,
                Components = components
            };

            // Assert
            actual.Should().BeEquivalentTo(expected);
            context.Should().BeEquivalentTo(
    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1 });

        }

        [Fact]
        public void ParseDocumentWithDescriptionInDollarRefsShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "documentWithSummaryAndDescriptionInReference.yaml"));

            // Act
            var actual = new OpenApiStreamReader().Read(stream, out var diagnostic);
            var schema = actual.Paths["/pets"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema;
            var header = actual.Components.Responses["Test"].Headers["X-Test"];

            // Assert
            Assert.True(header.Description == "A referenced X-Test header"); /*response header #ref's description overrides the header's description*/
            Assert.True(schema.UnresolvedReference == false && schema.Type == "object"); /*schema reference is resolved*/
            Assert.Equal("A pet in a petstore", schema.Description); /*The reference object's description overrides that of the referenced component*/
        }
    }
}
