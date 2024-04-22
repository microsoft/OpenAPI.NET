﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
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

        public OpenApiDocumentTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        public T Clone<T>(T element) where T : IOpenApiSerializable
        {
            using (var stream = new MemoryStream())
            {
                IOpenApiWriter writer;
                var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
                writer = new OpenApiJsonWriter(streamWriter, new OpenApiJsonWriterSettings()
                {
                    InlineLocalReferences = true
                });
                element.SerializeAsV3(writer);
                writer.Flush();
                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    return OpenApiModelFactory.Parse<T>(result, OpenApiSpecVersion.OpenApi3_0, out OpenApiDiagnostic diagnostic4);
                }
            }
        }

        public OpenApiSecurityScheme CloneSecurityScheme(OpenApiSecurityScheme element)
        {
            using (var stream = new MemoryStream())
            {
                IOpenApiWriter writer;
                var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
                writer = new OpenApiJsonWriter(streamWriter, new OpenApiJsonWriterSettings()
                {
                    InlineLocalReferences = true
                });
                element.SerializeAsV3WithoutReference(writer);
                writer.Flush();
                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    return OpenApiModelFactory.Parse<OpenApiSecurityScheme>(result, OpenApiSpecVersion.OpenApi3_0, out OpenApiDiagnostic diagnostic4);
                }
            }
        }

        [Fact]
        public void ParseDocumentFromInlineStringShouldSucceed()
        {
            var result = OpenApiDocument.Parse(
                @"
openapi : 3.0.0
info:
    title: Simple Document
    version: 0.9.1
paths: {}",
                OpenApiConstants.Yaml);

            result.OpenApiDocument.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
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
            var path = Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml");
            var result = OpenApiDocument.Load(path);

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0,
                    Errors = new List<OpenApiError>()
                    {
                        new OpenApiError("", "Paths is a REQUIRED field at #/")
                    }
                });

            result.OpenApiDocument.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
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
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));
        }
        [Fact]
        public void ParseBrokenMinimalDocumentShouldYieldExpectedDiagnostic()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "brokenMinimalDocument.yaml"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            result.OpenApiDocument.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Version = "0.9"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
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
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "minimalDocument.yaml"));

            result.OpenApiDocument.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
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
        public void ParseStandardPetStoreDocumentShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, JsonSchema>
                {
                    ["pet1"] = new JsonSchemaBuilder()
                                .Ref("#/components/schemas/pet1")
                                .Type(SchemaValueType.Object)
                                .Required("id", "name")
                                .Properties(
                                    ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                    ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                    ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                    ["newPet"] = new JsonSchemaBuilder()
                                    .Ref("#/components/schemas/newPet")
                                    .Type(SchemaValueType.Object)
                                    .Required("name")
                                    .Properties(
                                        ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                                        ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                                        ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                    ["errorModel"] = new JsonSchemaBuilder()
                                    .Ref("#/components/schemas/errorModel")
                                    .Type(SchemaValueType.Object)
                                    .Required("code", "message")
                                    .Properties(
                                        ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                                        ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                }
            };
            var petSchema = new JsonSchemaBuilder().Ref("#/components/schemas/pet1");

            var newPetSchema = new JsonSchemaBuilder().Ref("#/components/schemas/newPet");

            var errorModelSchema = new JsonSchemaBuilder().Ref("#/components/schemas/errorModel");

            var expectedDoc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                    Description =
                        "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                    TermsOfService = new Uri("http://helloreverb.com/terms/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Swagger API team",
                        Email = "foo@example.com",
                        Url = new Uri("http://swagger.io")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT")
                    }
                },
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = "http://petstore.swagger.io/api"
                    }
                },
                Paths = new OpenApiPaths
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
                                        Schema = new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Array)
                                        .Items(new JsonSchemaBuilder().Type(SchemaValueType.String))
                                    },
                                    new OpenApiParameter
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "maximum number of results to return",
                                        Required = false,
                                        Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build()
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
                                                Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(petSchema)
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(petSchema)
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation
                            {
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Pet to add to the store",
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
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            },
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/pets/{id}"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters = new List<OpenApiParameter>
                                {
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to fetch",
                                        Required = true,
                                        Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")
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
                                                Schema = petSchema
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Delete] = new OpenApiOperation
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters = new List<OpenApiParameter>
                                {
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to delete",
                                        Required = true,
                                        Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["204"] = new OpenApiResponse
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
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

            result.OpenApiDocument.Should().BeEquivalentTo(expectedDoc, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.OpenApiDiagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParseModifiedPetStoreDocumentWithTagAndSecurityShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStoreWithTagAndSecurity.yaml"));
            var actual = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, JsonSchema>
                {
                    ["pet1"] = new JsonSchemaBuilder()
                        .Ref("#/components/schemas/pet1")
                        .Type(SchemaValueType.Object)
                        .Required("id", "name")
                        .Properties(
                            ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                            ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                            ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                    ["newPet"] = new JsonSchemaBuilder()
                        .Ref("#/components/schemas/newPet")
                        .Type(SchemaValueType.Object)
                        .Required("name")
                        .Properties(
                            ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64")),
                            ("name", new JsonSchemaBuilder().Type(SchemaValueType.String)),
                            ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String))),
                    ["errorModel"] = new JsonSchemaBuilder()
                        .Ref("#/components/schemas/errorModel")
                        .Type(SchemaValueType.Object)
                        .Required("code", "message")
                        .Properties(
                            ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32")),
                            ("message", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                },
                SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["securitySchemeName1"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "apiKeyName1",
                        In = ParameterLocation.Header
                    },
                    ["securitySchemeName2"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OpenIdConnect,
                        OpenIdConnectUrl = new Uri("http://example.com")
                    }
                }
            };

            var petSchema = new JsonSchemaBuilder().Ref("#/components/schemas/pet1");

            var newPetSchema = new JsonSchemaBuilder().Ref("#/components/schemas/newPet");

            var errorModelSchema = new JsonSchemaBuilder().Ref("#/components/schemas/errorModel");

            var tag1 = new OpenApiTag
            {
                Name = "tagName1",
                Description = "tagDescription1",
                Reference = new OpenApiReference
                {
                    Id = "tagName1",
                    Type = ReferenceType.Tag
                }
            };


            var tag2 = new OpenApiTag
            {
                Name = "tagName2",
                Reference = new OpenApiReference
                {
                    Id = "tagName2",
                    Type = ReferenceType.Tag
                }
            };

            var securityScheme1 = CloneSecurityScheme(components.SecuritySchemes["securitySchemeName1"]);

            securityScheme1.Reference = new OpenApiReference
            {
                Id = "securitySchemeName1",
                Type = ReferenceType.SecurityScheme
            };

            var securityScheme2 = CloneSecurityScheme(components.SecuritySchemes["securitySchemeName2"]);

            securityScheme2.Reference = new OpenApiReference
            {
                Id = "securitySchemeName2",
                Type = ReferenceType.SecurityScheme
            };

            var expected = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                    Description =
                        "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                    TermsOfService = new Uri("http://helloreverb.com/terms/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Swagger API team",
                        Email = "foo@example.com",
                        Url = new Uri("http://swagger.io")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT")
                    }
                },
                Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer
                        {
                            Url = "http://petstore.swagger.io/api"
                        }
                    },
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
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
                                        new OpenApiParameter
                                        {
                                            Name = "tags",
                                            In = ParameterLocation.Query,
                                            Description = "tags to filter by",
                                            Required = false,
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Array)
                                                        .Items(new JsonSchemaBuilder().Type(SchemaValueType.String))
                                        },
                                        new OpenApiParameter
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
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.Array)
                                                    .Items(petSchema)
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.Array)
                                                    .Items(petSchema)
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation
                            {
                                Tags = new List<OpenApiTag>
                                    {
                                        tag1,
                                        tag2
                                    },
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Pet to add to the store",
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
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            },
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                },
                                Security = new List<OpenApiSecurityRequirement>
                                    {
                                        new OpenApiSecurityRequirement
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
                    ["/pets/{id}"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
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
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = petSchema
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Delete] = new OpenApiOperation
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters = new List<OpenApiParameter>
                                    {
                                        new OpenApiParameter
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
                                Responses = new OpenApiResponses
                                {
                                    ["204"] = new OpenApiResponse
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/html"] = new OpenApiMediaType
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
                        new OpenApiTag
                        {
                            Name = "tagName1",
                            Description = "tagDescription1"                            
                        }
                    },
                SecurityRequirements = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
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

            actual.OpenApiDocument.Should().BeEquivalentTo(expected, options => options
            .Excluding(x => x.HashCode)
            .Excluding(m => m.Tags[0].Reference)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[0].Reference)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[0].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Post].Tags[0].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[1].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Post].Tags[1].Reference.HostDocument)
            .Excluding(x => x.Workspace)
            .Excluding(y => y.BaseUri));

            actual.OpenApiDiagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParsePetStoreExpandedShouldSucceed()
        {
            var actual = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "petStoreExpanded.yaml"));

            // TODO: Create the object in memory and compare with the one read from YAML file.

            actual.OpenApiDiagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void GlobalSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "securedApi.yaml"));

            var securityRequirement = result.OpenApiDocument.SecurityRequirements.First();

            securityRequirement.Keys.First().Should().BeEquivalentTo(result.OpenApiDocument.Components.SecuritySchemes.First().Value,
                options => options.Excluding(x => x.Reference));
        }

        [Fact]
        public void HeaderParameterShouldAllowExample()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "apiWithFullHeaderComponent.yaml"));

            var exampleHeader = result.OpenApiDocument.Components?.Headers?["example-header"];
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
                                .Format(Formats.Uuid)
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Example.Node.Parent)
                .Excluding(x => x.Reference));

            var examplesHeader = result.OpenApiDocument.Components?.Headers?["examples-header"];
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
                                .Format(Formats.Uuid)
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Examples["uuid1"].Value.Node.Parent)
                .Excluding(e => e.Examples["uuid2"].Value.Node.Parent));
        }

        [Fact]
        public void ParseDocumentWithReferencedSecuritySchemeWorks()
        {
            // Act
            var settings = new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            };

            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "docWithSecuritySchemeReference.yaml"), settings);
            var securityScheme = result.OpenApiDocument.Components.SecuritySchemes["OAuth2"];

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
            var settings = new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            };
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml, settings);

            var actualSchema = result.OpenApiDocument.Paths["/users/{userId}"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new JsonSchemaBuilder()
                .Ref("#/components/schemas/User")
                .Build();

            // Assert
            actualSchema.Should().BeEquivalentTo(expectedSchema);
        }

        [Fact]
        public void ValidateExampleShouldNotHaveDataTypeMismatch()
        {
            // Act
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "documentWithDateExampleInSchema.yaml"), new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences

            });

            // Assert
            var warnings = result.OpenApiDiagnostic.Warnings;
            Assert.False(warnings.Any());
        }
        [Fact]
        public void ParseDocWithRefsUsingProxyReferencesSucceeds()
        {
            // Arrange
            var expected = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Pet Store with Referenceable Parameter",
                    Version = "1.0.0"
                },
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Summary = "Returns all pets",
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "Limit the number of pets returned",
                                        Required = false,
                                        Schema = new JsonSchemaBuilder()
                                            .Type(SchemaValueType.Integer)
                                            .Format("int32")
                                            .Default(10),
                                        Reference = new OpenApiReference
                                        {
                                            Id = "LimitParameter",
                                            Type = ReferenceType.Parameter
                                        }
                                    }
                                ],
                                Responses = new OpenApiResponses()
                            }
                        }
                    }
                },
                Components = new OpenApiComponents
                {
                    Parameters = new Dictionary<string, OpenApiParameter>
                    {
                        ["LimitParameter"] = new OpenApiParameter
                        {
                            Name = "limit",
                            In = ParameterLocation.Query,
                            Description = "Limit the number of pets returned",
                            Required = false,
                            Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Integer)
                                .Format("int32")
                                .Default(10)
                        }
                    }
                }
            };

            var expectedSerializedDoc = @"openapi: 3.0.1
info:
  title: Pet Store with Referenceable Parameter
  version: 1.0.0
paths:
  /pets:
    get:
      summary: Returns all pets
      parameters:
        - $ref: '#/components/parameters/LimitParameter'
      responses: { }
components:
  parameters:
    LimitParameter:
      name: limit
      in: query
      description: Limit the number of pets returned
      schema:
        type: integer
        format: int32
        default: 10";

            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minifiedPetStore.yaml"));

            // Act
            var doc = OpenApiDocument.Load(stream, "yaml").OpenApiDocument;
            var actualParam = doc.Paths["/pets"].Operations[OperationType.Get].Parameters.First();
            var outputDoc = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0).MakeLineBreaksEnvironmentNeutral();
            var output = actualParam.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
            var expectedParam = expected.Paths["/pets"].Operations[OperationType.Get].Parameters.First();

            // Assert
            actualParam.Should().BeEquivalentTo(expectedParam, options => options.Excluding(x => x.Reference.HostDocument));
            outputDoc.Should().BeEquivalentTo(expectedSerializedDoc.MakeLineBreaksEnvironmentNeutral());

        }
    }  
}
