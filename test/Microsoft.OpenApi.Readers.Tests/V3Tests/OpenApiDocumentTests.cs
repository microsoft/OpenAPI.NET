// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        public T Clone<T>(T element) where T : IOpenApiSerializable
        {
            using var stream = new MemoryStream();
            IOpenApiWriter writer;
            var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
            writer = new OpenApiJsonWriter(streamWriter, new()
            {
                InlineLocalReferences = true
            });
            element.SerializeAsV3(writer);
            writer.Flush();
            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            var result = streamReader.ReadToEnd();
            return new OpenApiStringReader().ReadFragment<T>(result, OpenApiSpecVersion.OpenApi3_0, out var diagnostic4);
        }

        public OpenApiSecurityScheme CloneSecurityScheme(OpenApiSecurityScheme element)
        {
            using var stream = new MemoryStream();
            IOpenApiWriter writer;
            var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
            writer = new OpenApiJsonWriter(streamWriter, new()
            {
                InlineLocalReferences = true
            });
            element.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            var result = streamReader.ReadToEnd();
            return new OpenApiStringReader().ReadFragment<OpenApiSecurityScheme>(result, OpenApiSpecVersion.OpenApi3_0, out var diagnostic4);
        }

        [Fact]
        public void ParseDocumentFromInlineStringShouldSucceed()
        {
            var openApiDoc = new OpenApiStringReader().Read(
                """

                openapi : 3.0.0
                info:
                    title: Simple Document
                    version: 0.9.1
                paths: {}
                """,
                out var context);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new()
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new()
                });

            context.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                        {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                        }
                });
        }

        [Fact]
        public void ParseBasicDocumentWithMultipleServersShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

                diagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic()
                    {
                        SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                        Errors = new List<OpenApiError>()
                        {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                        }
                    });

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new()
                    {
                        Title = "The API",
                        Version = "0.9.1",
                    },
                    Servers =
                    {
                        new OpenApiServer
                        {
                            Url = new Uri("http://www.example.org/api").ToString(),
                            Description = "The http endpoint"
                        },
                        new OpenApiServer
                        {
                            Url = new Uri("https://www.example.org/api").ToString(),
                            Description = "The https endpoint"
                        }
                    },
                    Paths = new()
                });
        }

        [Fact]
        public void ParseBrokenMinimalDocumentShouldYieldExpectedDiagnostic()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "brokenMinimalDocument.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Version = "0.9"
                    },
                    Paths = new OpenApiPaths()
                });

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                            new OpenApiError("", "Paths is a REQUIRED field at #/"),
                            new OpenApiValidatorError(nameof(OpenApiInfoRules.InfoRequiredFields),"#/info/title", "The field 'title' in 'info' object is REQUIRED.")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });
        }

        [Fact]
        public void ParseMinimalDocumentShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minimalDocument.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new()
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new()
                });

                diagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic()
                    {
                        SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                        Errors = new List<OpenApiError>()
                        {
                            new OpenApiError("", "Paths is a REQUIRED field at #/")
                        }
                    });
            }
        }

        [Fact]
        public void ParseStandardPetStoreDocumentShouldSucceed()
        {
            OpenApiDiagnostic context;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml")))
            {
                var actual = new OpenApiStreamReader().Read(stream, out context);

                var components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, JsonSchema>
                    {
                        ["pet"] = new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Object)
                                    .Required("id", "name")
                                    .Properties(
                                        ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                        ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                        ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                        ["newPet"] = new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Object)
                                        .Required("name")
                                        .Properties(
                                            ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                            ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                            ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                        ["errorModel"] = new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Object)
                                        .Required("code", "message")
                                        .Properties(
                                            ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                                            ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                    }
                };

                var petSchema = components.Schemas["pet"];

                var newPetSchema = components.Schemas["newPet"];

                var errorModelSchema = components.Schemas["errorModel"];

                var expected = new OpenApiDocument
                {
                    Info = new()
                    {
                        Version = "1.0.0",
                        Title = "Swagger Petstore (Simple)",
                        Description =
                            "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                        TermsOfService = new("http://helloreverb.com/terms/"),
                        Contact = new()
                        {
                            Name = "Swagger API team",
                            Email = "foo@example.com",
                            Url = new("http://swagger.io")
                        },
                        License = new()
                        {
                            Name = "MIT",
                            Url = new("http://opensource.org/licenses/MIT")
                        }
                    },
                    Servers = new List<OpenApiServer>
                    {
                        new()
                        {
                            Url = "http://petstore.swagger.io/api"
                        }
                    },
                    Paths = new()
                    {
                        ["/pets"] = new()
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                [OperationType.Get] = new()
                                {
                                    Description = "Returns all pets from the system that the user has access to",
                                    OperationId = "findPets",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "tags",
                                            In = ParameterLocation.Query,
                                            Description = "tags to filter by",
                                            Required = false,
                                            Schema = new JsonSchemaBuilder()
                                            .Type(SchemaValueType.Array)
                                            .Items(new JsonSchemaBuilder().Type(SchemaValueType.String))
                                        },
                                        new()
                                        {
                                            Name = "limit",
                                            In = ParameterLocation.Query,
                                            Description = "maximum number of results to return",
                                            Required = false,
                                            Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build()
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(petSchema)
                                                },
                                                ["application/xml"] = new()
                                                {
                                                    Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(petSchema)
                                                }
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                },
                                [OperationType.Post] = new()
                                {
                                    Description = "Creates a new pet in the store.  Duplicates are allowed",
                                    OperationId = "addPet",
                                    RequestBody = new()
                                    {
                                        Description = "Pet to add to the store",
                                        Required = true,
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = newPetSchema                        }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = petSchema
                                                },
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        ["/pets/{id}"] = new()
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                [OperationType.Get] = new()
                                {
                                    Description =
                                        "Returns a user based on a single ID, if the user does not have access to the pet",
                                    OperationId = "findPetById",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to fetch",
                                            Required = true,
                                            Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = petSchema
                                                },
                                                ["application/xml"] = new()
                                                {
                                                    Schema = petSchema
                                                }
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                },
                                [OperationType.Delete] = new()
                                {
                                    Description = "deletes a single pet based on the ID supplied",
                                    OperationId = "deletePet",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to delete",
                                            Required = true,
                                            Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["204"] = new()
                                        {
                                            Description = "pet deleted"
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
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

                actual.Should().BeEquivalentTo(expected);
            }

            context.Should().BeEquivalentTo(
                new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParseModifiedPetStoreDocumentWithTagAndSecurityShouldSucceed()
        {
            OpenApiDiagnostic context;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStoreWithTagAndSecurity.yaml")))
            {
                var actual = new OpenApiStreamReader().Read(stream, out context);

                var components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, JsonSchema>
                    {
                        ["pet1"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Required("id", "name")
                            .Properties(
                                ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                        ["newPet"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Required("name")
                            .Properties(
                                ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                        ["errorModel"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Required("code", "message")
                            .Properties(
                                ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                                ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                    },
                    SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["securitySchemeName1"] = new()
                        {
                            Type = SecuritySchemeType.ApiKey,
                            Name = "apiKeyName1",
                            In = ParameterLocation.Header,
                            Reference = new()
                            {
                                Id = "securitySchemeName1",
                                Type = ReferenceType.SecurityScheme,
                                HostDocument = actual
                            }

                        },
                        ["securitySchemeName2"] = new()
                        {
                            Type = SecuritySchemeType.OpenIdConnect,
                            OpenIdConnectUrl = new("http://example.com"),
                            Reference = new()
                            {
                                Id = "securitySchemeName2",
                                Type = ReferenceType.SecurityScheme,
                                HostDocument = actual
                            }
                        }
                    }
                };

                var petSchema = components.Schemas["pet1"];

                var newPetSchema = components.Schemas["newPet"];

                var errorModelSchema = components.Schemas["errorModel"];

                var tag1 = new OpenApiTag
                {
                    Name = "tagName1",
                    Description = "tagDescription1",
                    Reference = new()
                    {
                        Id = "tagName1",
                        Type = ReferenceType.Tag
                    }
                };

                var tag2 = new OpenApiTag
                {
                    Name = "tagName2"
                };

                var securityScheme1 = CloneSecurityScheme(components.SecuritySchemes["securitySchemeName1"]);

                securityScheme1.Reference = new()
                {
                    Id = "securitySchemeName1",
                    Type = ReferenceType.SecurityScheme
                };

                var securityScheme2 = CloneSecurityScheme(components.SecuritySchemes["securitySchemeName2"]);

                securityScheme2.Reference = new()
                {
                    Id = "securitySchemeName2",
                    Type = ReferenceType.SecurityScheme
                };

                var expected = new OpenApiDocument
                {
                    Info = new()
                    {
                        Version = "1.0.0",
                        Title = "Swagger Petstore (Simple)",
                        Description =
                            "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                        TermsOfService = new("http://helloreverb.com/terms/"),
                        Contact = new()
                        {
                            Name = "Swagger API team",
                            Email = "foo@example.com",
                            Url = new("http://swagger.io")
                        },
                        License = new()
                        {
                            Name = "MIT",
                            Url = new("http://opensource.org/licenses/MIT")
                        }
                    },
                    Servers = new List<OpenApiServer>
                    {
                        new()
                        {
                            Url = "http://petstore.swagger.io/api"
                        }
                    },
                    Paths = new()
                    {
                        ["/pets"] = new()
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                [OperationType.Get] = new()
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        tag1,
                                        tag2
                                    },
                                    Description = "Returns all pets from the system that the user has access to",
                                    OperationId = "findPets",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "tags",
                                            In = ParameterLocation.Query,
                                            Description = "tags to filter by",
                                            Required = false,
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Array)
                                                        .Items(new JsonSchemaBuilder().Type(SchemaValueType.String))
                                        },
                                        new()
                                        {
                                            Name = "limit",
                                            In = ParameterLocation.Query,
                                            Description = "maximum number of results to return",
                                            Required = false,
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Integer)
                                                        .Format("int32")
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Array)
                                                        .Items(petSchema)
                                                },
                                                ["application/xml"] = new()
                                                {
                                                    Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Array)
                                                        .Items(petSchema) 
                                                }
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                },
                                [OperationType.Post] = new()
                                {
                                    Tags = new List<OpenApiTag>
                                    {
                                        tag1,
                                        tag2
                                    },
                                    Description = "Creates a new pet in the store.  Duplicates are allowed",
                                    OperationId = "addPet",
                                    RequestBody = new()
                                    {
                                        Description = "Pet to add to the store",
                                        Required = true,
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = newPetSchema
                                            }
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = petSchema
                                                },
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    },
                                    Security = new List<OpenApiSecurityRequirement>
                                    {
                                        new()
                                        {
                                            [securityScheme1] = new List<string>(),
                                            [securityScheme2] = new List<string>
                                            {
                                                "scope1",
                                                "scope2"
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        ["/pets/{id}"] = new()
                        {
                            Operations = new Dictionary<OperationType, OpenApiOperation>
                            {
                                [OperationType.Get] = new()
                                {
                                    Description =
                                        "Returns a user based on a single ID, if the user does not have access to the pet",
                                    OperationId = "findPetById",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to fetch",
                                            Required = true,
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Integer)
                                                        .Format("int64")
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["200"] = new()
                                        {
                                            Description = "pet response",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new()
                                                {
                                                    Schema = petSchema
                                                },
                                                ["application/xml"] = new()
                                                {
                                                    Schema = petSchema
                                                }
                                            }
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                },
                                [OperationType.Delete] = new()
                                {
                                    Description = "deletes a single pet based on the ID supplied",
                                    OperationId = "deletePet",
                                    Parameters = new List<OpenApiParameter>
                                    {
                                        new()
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to delete",
                                            Required = true,
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Integer)
                                                        .Format("int64")
                                        }
                                    },
                                    Responses = new()
                                    {
                                        ["204"] = new()
                                        {
                                            Description = "pet deleted"
                                        },
                                        ["4XX"] = new()
                                        {
                                            Description = "unexpected client error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        },
                                        ["5XX"] = new()
                                        {
                                            Description = "unexpected server error",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["text/html"] = new()
                                                {
                                                    Schema = errorModelSchema
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Components = components,
                    Tags = new List<OpenApiTag>
                    {
                        new()
                        {
                            Name = "tagName1",
                            Description = "tagDescription1",
                            Reference = new()
                            {
                                Id = "tagName1",
                                Type = ReferenceType.Tag
                            }
                        }
                    },
                    SecurityRequirements = new List<OpenApiSecurityRequirement>
                    {
                        new()
                        {
                            [securityScheme1] = new List<string>(),
                            [securityScheme2] = new List<string>
                            {
                                "scope1",
                                "scope2",
                                "scope3"
                            }
                        }
                    }
                };

                actual.Should().BeEquivalentTo(expected, options => options.Excluding(m => m.Name == "HostDocument"));
            }

            context.Should().BeEquivalentTo(
                    new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParsePetStoreExpandedShouldSucceed()
        {
            OpenApiDiagnostic context;

            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStoreExpanded.yaml")))
            {
                var actual = new OpenApiStreamReader().Read(stream, out context);

                // TODO: Create the object in memory and compare with the one read from YAML file.
            }

            context.Should().BeEquivalentTo(
                    new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void GlobalSecurityRequirementShouldReferenceSecurityScheme()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "securedApi.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            var securityRequirement = openApiDoc.SecurityRequirements.First();

            Assert.Same(securityRequirement.Keys.First(), openApiDoc.Components.SecuritySchemes.First().Value);
        }

        [Fact]
        public void HeaderParameterShouldAllowExample()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "apiWithFullHeaderComponent.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            var exampleHeader = openApiDoc.Components?.Headers?["example-header"];
            Assert.NotNull(exampleHeader);
            exampleHeader.Should().BeEquivalentTo(
                new OpenApiHeader()
                {
                    Description = "Test header with example",
                    Required = true,
                    Deprecated = true,
                    AllowEmptyValue = true,
                    AllowReserved = true,
                    Style = ParameterStyle.Simple,
                    Explode = true,
                    Example = new OpenApiAny("99391c7e-ad88-49ec-a2ad-99ddcb1f7721"),
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .Format(Formats.Uuid),
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Header,
                        Id = "example-header"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Example.Node.Parent));

            var examplesHeader = openApiDoc.Components?.Headers?["examples-header"];
            Assert.NotNull(examplesHeader);
            examplesHeader.Should().BeEquivalentTo(
                new OpenApiHeader()
                {
                    Description = "Test header with example",
                    Required = true,
                    Deprecated = true,
                    AllowEmptyValue = true,
                    AllowReserved = true,
                    Style = ParameterStyle.Simple,
                    Explode = true,
                    Examples = new Dictionary<string, OpenApiExample>()
                    {
                            { "uuid1", new OpenApiExample()
                                {
                                    Value = new OpenApiAny("99391c7e-ad88-49ec-a2ad-99ddcb1f7721")
                                }
                            },
                            { "uuid2", new OpenApiExample()
                                {
                                    Value = new OpenApiAny("99391c7e-ad88-49ec-a2ad-99ddcb1f7721")
                                }
                            }
                    },
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                                .Format(Formats.Uuid),
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Header,
                        Id = "examples-header"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Examples["uuid1"].Value.Node.Parent)
                .Excluding(e => e.Examples["uuid2"].Value.Node.Parent));
        }

        [Fact]
        public void ParseDocumentWithReferencedSecuritySchemeWorks()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "docWithSecuritySchemeReference.yaml"));

            // Act
            var doc = new OpenApiStreamReader(new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            }).Read(stream, out var diagnostic);

            var securityScheme = doc.Components.SecuritySchemes["OAuth2"];

            // Assert
            Assert.False(securityScheme.UnresolvedReference);
            Assert.NotNull(securityScheme.Flows);
        }

        [Fact]
        public void ParseDocumentWithJsonSchemaReferencesWorks() 
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "docWithJsonSchema.yaml"));

            // Act
            var doc = new OpenApiStreamReader(new()
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            }).Read(stream, out var diagnostic);

            var actualSchema = doc.Paths["/users/{userId}"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .Properties(
                    ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer)),
                    ("username", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                    ("email", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                .Build();

            // Assert
            actualSchema.Should().BeEquivalentTo(expectedSchema);
        }

    }
}
