// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using Microsoft.OpenApi.Writers;
using SharpYaml.Model;
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
                element.SerializeAsV3(writer);
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

            result.Document.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });
        }

        [Fact]
        public void ParseBasicDocumentWithMultipleServersShouldSucceed()
        {
            var path = System.IO.Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml");
            var result = OpenApiDocument.Load(path);

            result.Diagnostic.Errors.Should().BeEmpty();
            result.Document.Should().BeEquivalentTo(
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
            using var stream = Resources.GetStream(System.IO.Path.Combine(SampleFolderPath, "brokenMinimalDocument.yaml"));
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            result.Document.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Version = "0.9"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                            new OpenApiValidatorError(nameof(OpenApiInfoRules.InfoRequiredFields),"#/info/title", "The field 'title' in 'info' object is REQUIRED.")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });
        }

        [Fact]
        public void ParseMinimalDocumentShouldSucceed()
        {
            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "minimalDocument.yaml"));

            result.Document.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });
        }

        [Fact]
        public void ParseStandardPetStoreDocumentShouldSucceed()
        {
            using var stream = Resources.GetStream(System.IO.Path.Combine(SampleFolderPath, "petStore.yaml"));
            var actual = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["pet1"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                        }
                    },
                    ["newPet"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                        }
                    },
                    ["errorModel"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["code"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32"
                            },
                            ["message"] = new()
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
                }
            };

            var petSchema = new OpenApiSchemaReference("pet1", actual.Document);
            var newPetSchema = new OpenApiSchemaReference("newPet", actual.Document);

            var errorModelSchema = new OpenApiSchemaReference("errorModel", actual.Document);

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
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.Array,
                                            Items = new()
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
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.Integer,
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
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
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int64"
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
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int64"
                                        }
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

            actual.Document.Should().BeEquivalentTo(expectedDoc, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            actual.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParseModifiedPetStoreDocumentWithTagAndSecurityShouldSucceed()
        {
            using var stream = Resources.GetStream(System.IO.Path.Combine(SampleFolderPath, "petStoreWithTagAndSecurity.yaml"));
            var actual = OpenApiDocument.Load(stream, OpenApiConstants.Yaml);

            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["pet1"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                        }
                    },
                    ["newPet"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "name"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["name"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tag"] = new()
                            {
                                Type = JsonSchemaType.String
                            },
                        }
                    },
                    ["errorModel"] = new()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["code"] = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32"
                            },
                            ["message"] = new()
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
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

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema = Clone(components.Schemas["pet1"]);
            petSchema.Reference = new()
            {
                Id = "pet1",
                Type = ReferenceType.Schema,
                HostDocument = actual.Document
            };

            var newPetSchema = Clone(components.Schemas["newPet"]);

            newPetSchema.Reference = new()
            {
                Id = "newPet",
                Type = ReferenceType.Schema,
                HostDocument = actual.Document
            };

            var errorModelSchema = Clone(components.Schemas["errorModel"]);

            errorModelSchema.Reference = new()
            {
                Id = "errorModel",
                Type = ReferenceType.Schema,
                HostDocument = actual.Document
            };

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
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = new()
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
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Integer,
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchema
                                                }
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
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Integer,
                                                Format = "int64"
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
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Integer,
                                                Format = "int64"
                                            }
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

            actual.Document.Should().BeEquivalentTo(expected, options => options
            .Excluding(x => x.HashCode)
            .Excluding(m => m.Tags[0].Reference)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[0].Reference)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[0].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Post].Tags[0].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Get].Tags[1].Reference.HostDocument)
            .Excluding(x => x.Paths["/pets"].Operations[OperationType.Post].Tags[1].Reference.HostDocument)
            .Excluding(x => x.Workspace)
            .Excluding(y => y.BaseUri));

            actual.Diagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void ParsePetStoreExpandedShouldSucceed()
        {
            var actual = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "petStoreExpanded.yaml"));

            // TODO: Create the object in memory and compare with the one read from YAML file.

            actual.Diagnostic.Should().BeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void GlobalSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "securedApi.yaml"));

            var securityRequirement = result.Document.SecurityRequirements[0];

            securityRequirement.Keys.First().Should().BeEquivalentTo(result.Document.Components.SecuritySchemes.First().Value,
                options => options.Excluding(x => x.Reference));
        }

        [Fact]
        public void HeaderParameterShouldAllowExample()
        {
            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "apiWithFullHeaderComponent.yaml"));

            var exampleHeader = result.Document.Components?.Headers?["example-header"];
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
                    Example = "99391c7e-ad88-49ec-a2ad-99ddcb1f7721",
                    Schema = new()
                    {
                        Type = JsonSchemaType.String,
                        Format = "uuid"
                    },
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Example.Parent)
                .Excluding(x => x.Reference));

            var examplesHeader = result.Document.Components?.Headers?["examples-header"];
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
                                    Value = "99391c7e-ad88-49ec-a2ad-99ddcb1f7721"
                                }
                            },
                            { "uuid2", new OpenApiExample()
                                {
                                    Value = "99391c7e-ad88-49ec-a2ad-99ddcb1f7721"
                                }
                            }
                    },
                    Schema = new()
                    {
                        Type = JsonSchemaType.String,
                        Format = "uuid"
                    },
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Examples["uuid1"].Value.Parent)
                .Excluding(e => e.Examples["uuid2"].Value.Parent));
        }

        [Fact]
        public void ParseDocumentWithReferencedSecuritySchemeWorks()
        {
            // Act
            var settings = new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            };

            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "docWithSecuritySchemeReference.yaml"), settings);
            var securityScheme = result.Document.Components.SecuritySchemes["OAuth2"];

            // Assert
            Assert.False(securityScheme.UnresolvedReference);
            Assert.NotNull(securityScheme.Flows);
        }

        [Fact]
        public void ParseDocumentWithJsonSchemaReferencesWorks()
        {
            // Arrange
            using var stream = Resources.GetStream(System.IO.Path.Combine(SampleFolderPath, "docWithJsonSchema.yaml"));

            // Act
            var settings = new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences
            };
            var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml, settings);

            var actualSchema = result.Document.Paths["/users/{userId}"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new OpenApiSchemaReference("User", result.Document);
            // Assert
            actualSchema.Should().BeEquivalentTo(expectedSchema);
        }

        [Fact]
        public void ValidateExampleShouldNotHaveDataTypeMismatch()
        {
            // Act
            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "documentWithDateExampleInSchema.yaml"), new OpenApiReaderSettings
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences

            });

            // Assert
            var warnings = result.Diagnostic.Warnings;
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
                                        Schema = new()
                                        { 
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32",
                                            Default = 10
                                        },
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
                            Schema = new()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32",
                                Default = 10
                            },
                        }
                    }
                }
            };

            var expectedSerializedDoc = @"openapi: 3.0.4
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

            using var stream = Resources.GetStream(System.IO.Path.Combine(SampleFolderPath, "minifiedPetStore.yaml"));

            // Act
            var doc = OpenApiDocument.Load(stream, "yaml").Document;
            var actualParam = doc.Paths["/pets"].Operations[OperationType.Get].Parameters[0];
            var outputDoc = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0).MakeLineBreaksEnvironmentNeutral();
            var expectedParam = expected.Paths["/pets"].Operations[OperationType.Get].Parameters[0];

            // Assert
            actualParam.Should().BeEquivalentTo(expectedParam, options => options
                .Excluding(x => x.Reference.HostDocument)
                .Excluding(x => x.Schema.Default.Parent)
                .Excluding(x => x.Schema.Default.Options)
                .IgnoringCyclicReferences());
            outputDoc.Should().BeEquivalentTo(expectedSerializedDoc.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void ParseBasicDocumentWithServerVariableShouldSucceed()
        {
            var result = OpenApiDocument.Parse("""
                                                            openapi : 3.0.0
                                                            info:
                                                                title: The API
                                                                version: 0.9.1
                                                            servers:
                                                              - url: http://www.example.org/api/{version}
                                                                description: The http endpoint
                                                                variables:
                                                                  version:
                                                                    default: v2
                                                                    enum: [v1, v2]
                                                            paths: {}
                                                            """, "yaml");

            var expected = new OpenApiDocument
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
                            Url = "http://www.example.org/api/{version}",
                            Description = "The http endpoint",
                            Variables = new Dictionary<string, OpenApiServerVariable>
                            {
                                {"version", new OpenApiServerVariable {Default = "v2", Enum = ["v1", "v2"]}}
                            }
                        }
                    },
                Paths = new()
            };

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic 
                { 
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                });

            result.Document.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.BaseUri));
        }

        [Fact]
        public void ParseBasicDocumentWithServerVariableAndNoDefaultShouldFail()
        {
            var result = OpenApiDocument.Parse("""
                                                            openapi : 3.0.0
                                                            info:
                                                                title: The API
                                                                version: 0.9.1
                                                            servers:
                                                              - url: http://www.example.org/api/{version}
                                                                description: The http endpoint
                                                                variables:
                                                                  version:
                                                                    enum: [v1, v2]
                                                            paths: {}
                                                            """, "yaml");

            result.Diagnostic.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void ParseDocumentWithEmptyPathsSucceeds()
        {
            var result = OpenApiDocument.Load(System.IO.Path.Combine(SampleFolderPath, "docWithEmptyPaths.yaml"));
            result.Diagnostic.Errors.Should().BeEmpty();
        }
    }
}
