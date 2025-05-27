// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Moq;
using Moq.Protected;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";
        private static async Task<T> CloneAsync<T>(T element) where T : class, IOpenApiSerializable
        {
            using var stream = new MemoryStream();
            var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(streamWriter, new OpenApiJsonWriterSettings()
            {
                InlineLocalReferences = true
            });
            element.SerializeAsV3(writer);
            await writer.FlushAsync();
            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            var result = await streamReader.ReadToEndAsync();
            return OpenApiModelFactory.Parse<T>(result, OpenApiSpecVersion.OpenApi3_0, new(), out var _);
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
                OpenApiConstants.Yaml, SettingsFixture.ReaderSettings);

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

            Assert.Equivalent(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);
        }

        [Fact]
        public void ParseInlineStringWithoutProvidingFormatSucceeds()
        {
            var stringOpenApiDoc = """
openapi: 3.1.0
info:
  title: Sample API
  version: 1.0.0
paths: {}
""";

            var readResult = OpenApiDocument.Parse(stringOpenApiDoc, settings: SettingsFixture.ReaderSettings);
            Assert.Equal("Sample API", readResult.Document.Info.Title);
        }

        [Fact]
        public async Task ParseBasicDocumentWithMultipleServersShouldSucceed()
        {
            var path = Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml");
            var result = await OpenApiDocument.LoadAsync(path, SettingsFixture.ReaderSettings);

            Assert.Empty(result.Diagnostic.Errors);
            result.Document.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "The API",
                        Version = "0.9.1",
                    },
                    Servers =
                    [
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
                    ],
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));
        }
        [Fact]
        public async Task ParseBrokenMinimalDocumentShouldYieldExpectedDiagnostic()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "brokenMinimalDocument.yaml"));
            // Copy stream to MemoryStream
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var result = await OpenApiDocument.LoadAsync(memoryStream, settings: SettingsFixture.ReaderSettings);

            result.Document.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Version = "0.9"
                    },
                    Paths = new OpenApiPaths()
                }, options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            Assert.Equivalent(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                            new OpenApiValidatorError(nameof(OpenApiInfoRules.InfoRequiredFields),"#/info/title", "The field 'title' in 'info' object is REQUIRED.")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);
        }

        [Fact]
        public async Task ParseMinimalDocumentShouldSucceed()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "minimalDocument.yaml"), SettingsFixture.ReaderSettings);

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

            Assert.Equivalent(
                new OpenApiDiagnostic()
                {
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);
        }

        [Fact]
        public async Task ParseStandardPetStoreDocumentShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var actual = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings);

            var components = new OpenApiComponents
            {
                Schemas = new()
                {
                    ["pet1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
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
                        }
                    },
                    ["newPet"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "name"
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
                        }
                    },
                    ["errorModel"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                        Properties = new()
                        {
                            ["code"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32"
                            },
                            ["message"] = new OpenApiSchema()
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
                Servers =
                [
                    new OpenApiServer
                    {
                        Url = "http://petstore.swagger.io/api"
                    }
                ],
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
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
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
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
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Post] = new OpenApiOperation
                            {
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Pet to add to the store",
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
                                        Description = "pet response",
                                        Content = new()
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
                                        Content = new()
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
                                        Content = new()
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
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to fetch",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int64"
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
                                        Content = new()
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
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Delete] = new OpenApiOperation
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to delete",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int64"
                                        }
                                    }
                                ],
                                Responses = new OpenApiResponses
                                {
                                    ["204"] = new OpenApiResponse
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
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
                                        Content = new()
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

            Assert.Equivalent(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 }, actual.Diagnostic);
        }

        [Fact]
        public async Task ParseModifiedPetStoreDocumentWithTagAndSecurityShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStoreWithTagAndSecurity.yaml"));
            var actual = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings);

            var components = new OpenApiComponents
            {
                Schemas = new()
                {
                    ["pet1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "id",
                                "name"
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
                        }
                    },
                    ["newPet"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "name"
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
                        }
                    },
                    ["errorModel"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                        Properties = new()
                        {
                            ["code"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32"
                            },
                            ["message"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    },
                },
                SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
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
            var petSchemaSource = Assert.IsType<OpenApiSchema>(components.Schemas["pet1"]);
            var petSchema = await CloneAsync(petSchemaSource);
            Assert.IsType<OpenApiSchema>(petSchema);
            var petSchemaReference = new OpenApiSchemaReference("pet1");

            var newPetSchemaSource = Assert.IsType<OpenApiSchema>(components.Schemas["newPet"]);
            var newPetSchema = await CloneAsync(newPetSchemaSource);
            Assert.IsType<OpenApiSchema>(newPetSchema);
            var newPetSchemaReference = new OpenApiSchemaReference("newPet");

            var errorModelSchemaSource = Assert.IsType<OpenApiSchema>(components.Schemas["errorModel"]);
            var errorModelSchema = await CloneAsync(errorModelSchemaSource);
            Assert.IsType<OpenApiSchema>(errorModelSchema);
            var errorModelSchemaReference = new OpenApiSchemaReference("errorModel");

            var tagReference1 = new OpenApiTagReference("tagName1");

            var tagReference2 = new OpenApiTagReference("tagName2");

            Assert.IsType<OpenApiSecurityScheme>(components.SecuritySchemes["securitySchemeName1"]);

            Assert.IsType<OpenApiSecurityScheme>(components.SecuritySchemes["securitySchemeName2"]);

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
                Servers =
                    [
                        new OpenApiServer
                        {
                            Url = "http://petstore.swagger.io/api"
                        }
                    ],
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Tags = new HashSet<OpenApiTagReference>
                                    {
                                        tagReference1,
                                        tagReference2
                                    },
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
                                                    Items = petSchemaReference
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = petSchemaReference
                                                }
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Post] = new OpenApiOperation
                            {
                                Tags = new HashSet<OpenApiTagReference>
                                    {
                                        tagReference1,
                                        tagReference2
                                    },
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody
                                {
                                    Description = "Pet to add to the store",
                                    Required = true,
                                    Content = new()
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = newPetSchemaReference
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new()
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = petSchemaReference
                                            },
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    }
                                },
                                Security =
                                    [
                                        new OpenApiSecurityRequirement
                                        {
                                            [new OpenApiSecuritySchemeReference("securitySchemeName1")] = [],
                                            [new OpenApiSecuritySchemeReference("securitySchemeName2")] =
                                            [
                                                "scope1",
                                                "scope2"
                                            ]
                                        }
                                    ]
                            }
                        }
                    },
                    ["/pets/{id}"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters =
                                    [
                                        new OpenApiParameter
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to fetch",
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Integer,
                                                Format = "int64"
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
                                                Schema = petSchemaReference
                                            },
                                            ["application/xml"] = new OpenApiMediaType
                                            {
                                                Schema = petSchemaReference
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    }
                                }
                            },
                            [HttpMethod.Delete] = new OpenApiOperation
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters =
                                    [
                                        new OpenApiParameter
                                        {
                                            Name = "id",
                                            In = ParameterLocation.Path,
                                            Description = "ID of pet to delete",
                                            Required = true,
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Integer,
                                                Format = "int64"
                                            }
                                        }
                                    ],
                                Responses = new OpenApiResponses
                                {
                                    ["204"] = new OpenApiResponse
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected client error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse
                                    {
                                        Description = "unexpected server error",
                                        Content = new()
                                        {
                                            ["text/html"] = new OpenApiMediaType
                                            {
                                                Schema = errorModelSchemaReference
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Components = components,
                Tags = new HashSet<OpenApiTag>
                    {
                        new OpenApiTag
                        {
                            Name = "tagName1",
                            Description = "tagDescription1"                            
                        },
                        new OpenApiTag
                        {
                            Name = "tagName2",
                            Description = "tagDescription2"
                        }
                    },
                Security =
                    [
                        new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecuritySchemeReference("securitySchemeName1")] = [],
                            [new OpenApiSecuritySchemeReference("securitySchemeName2")] =
                            [
                                "scope1",
                                "scope2",
                                "scope3"
                            ]
                        }
                    ]
            };
            expected.RegisterComponents();
            expected.SetReferenceHostDocument();

            actual.Document.Should().BeEquivalentTo(expected, options => options
            .IgnoringCyclicReferences()
            .Excluding(ctx => ctx.Path.Contains("Paths[\"/pets\"].Operations[HttpMethod.Get].Tags"))
            .Excluding(ctx => ctx.Path.Contains("Paths[\"/pets\"].Operations[HttpMethod.Post].Tags"))
            .Excluding(x => x.Workspace)
            .Excluding(y => y.BaseUri));

            Assert.Equivalent(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 }, actual.Diagnostic);
        }

        [Fact]
        public async Task ParsePetStoreExpandedShouldSucceed()
        {
            var actual = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "petStoreExpanded.yaml"), SettingsFixture.ReaderSettings);

            // TODO: Create the object in memory and compare with the one read from YAML file.

            Assert.Equivalent(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 }, actual.Diagnostic);
        }

        [Fact]
        public async Task GlobalSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "securedApi.yaml"), SettingsFixture.ReaderSettings);

            var securityRequirement = result.Document.Security[0];

            Assert.Equivalent(result.Document.Components.SecuritySchemes.First().Value, securityRequirement.Keys.First());
        }

        [Fact]
        public async Task HeaderParameterShouldAllowExample()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "apiWithFullHeaderComponent.yaml"), SettingsFixture.ReaderSettings);

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
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String,
                        Format = "uuid"
                    },
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Example.Parent));

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
                    Examples = new Dictionary<string, IOpenApiExample>
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
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String,
                        Format = "uuid"
                    },
                }, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Examples["uuid1"].Value.Parent)
                .Excluding(e => e.Examples["uuid2"].Value.Parent));
        }

        [Fact]
        public async Task ParseDocumentWithReferencedSecuritySchemeWorks()
        {
            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithSecuritySchemeReference.yaml"), SettingsFixture.ReaderSettings);
            var securityScheme = result.Document.Components.SecuritySchemes["OAuth2"];

            // Assert
            Assert.NotNull(securityScheme.Flows);
        }

        [Fact]
        public async Task ParseDocumentWithJsonSchemaReferencesWorks()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "docWithJsonSchema.yaml"));

            // Act
            var result = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings);

            var actualSchema = result.Document.Paths["/users/{userId}"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;

            var expectedSchema = new OpenApiSchemaReference("User", result.Document);
            // Assert
            Assert.Equivalent(expectedSchema, actualSchema);
        }

        [Fact]
        public async Task ValidateExampleShouldNotHaveDataTypeMismatch()
        {
            // Act
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "documentWithDateExampleInSchema.yaml"), SettingsFixture.ReaderSettings);

            // Assert
            var warnings = result.Diagnostic.Warnings;
            Assert.False(warnings.Any());
        }
        const string DoubleHopReferenceSerializedDoc =
"""
{
  "components": {
    "schemas": {
      "Pet": {
        "description": "A pet",
        "properties": {
          "id": {
            "format": "int64",
            "type": "integer"
          },
          "name": {
            "type": "string"
          },
          "tag": {
            "type": "string"
          }
        },
        "type": "object"
      },
      "PetReference": {
        "$ref": "#/components/schemas/Pet",
        "description": "A reference to a pet"
      }
    }
  },
  "info": {
    "title": "Pet Store with double hop references",
    "version": "1.0.0"
  },
  "openapi": "3.1.1",
  "paths": {
    "/pets": {
      "get": {
        "responses": {
          "200": {
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/PetReference",
                  "description": "A reference to a pet reference"
                }
              }
            },
            "description": "A list of pets"
          }
        },
        "summary": "Returns all pets"
      }
    }
  }
}
""";
        [Fact]
        public async Task ParsesDoubleHopReferences()
        {
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(DoubleHopReferenceSerializedDoc));
            var (document, _) = await OpenApiDocument.LoadAsync(stream);
            Assert.NotNull(document);

            var petReferenceInResponse = Assert.IsType<OpenApiSchemaReference>(document.Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema);
            Assert.Equal("A reference to a pet reference", petReferenceInResponse.Description, StringComparer.OrdinalIgnoreCase);
            var petReference = Assert.IsType<OpenApiSchemaReference>(petReferenceInResponse.Target);
            Assert.Equal("A reference to a pet", petReference.Description, StringComparer.OrdinalIgnoreCase);
            var petReferenceTarget = Assert.IsType<OpenApiSchema>(petReference.Target);
            Assert.Equal("A pet", petReferenceTarget.Description, StringComparer.OrdinalIgnoreCase);
            Assert.Equal(petReferenceTarget, petReferenceInResponse.RecursiveTarget);
        }

        [Fact]
        public async Task SerializesDoubleHopeReferences()
        {
            var document = new OpenApiDocument()
            {
                Info = new OpenApiInfo
                {
                    Title = "Pet Store with double hop references",
                    Version = "1.0.0"
                }
            };
            var petSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Description = "A pet",
                Properties = new()
                {
                    ["id"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer,
                        Format = "int64"
                    },
                    ["name"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["tag"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };
            document.AddComponent("Pet", petSchema);
            var petSchemaReference = new OpenApiSchemaReference("Pet")
            {
                Description = "A reference to a pet"
            };
            document.AddComponent("PetReference", petSchemaReference);
            document.AddComponent("Pet", petSchema); // does not add duplicate keys
            document.Paths.Add("/pets", new OpenApiPathItem
            {
                Operations = new()
                {
                    [HttpMethod.Get] = new OpenApiOperation
                    {
                        Summary = "Returns all pets",
                        Responses =
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "A list of pets",
                                Content = new()
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("PetReference")
                                        {
                                            Description = "A reference to a pet reference"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            using var stringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(stringWriter);
            document.SerializeAsV31(writer);
            await writer.FlushAsync();

            var serializedDoc = stringWriter.ToString();

            Assert.True(JsonNode.DeepEquals(
                JsonNode.Parse(serializedDoc),
                JsonNode.Parse(DoubleHopReferenceSerializedDoc)));
        }

        [Fact]
        public async Task ParseDocWithRefsUsingProxyReferencesSucceeds()
        {
            var parameter = new OpenApiParameter
            {
                Name = "limit",
                In = ParameterLocation.Query,
                Description = "Limit the number of pets returned",
                Required = false,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Integer,
                    Format = "int32",
                    Default = 10
                },
            };
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
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Summary = "Returns all pets",
                                Parameters =
                                [
                                    new OpenApiParameterReference("LimitParameter"),
                                ],
                                Responses = new OpenApiResponses()
                            }
                        }
                    }
                },
                Components = new OpenApiComponents
                {
                    Parameters = new Dictionary<string, IOpenApiParameter>
                    {
                        ["LimitParameter"] = parameter
                    }
                }
            };
            expected.RegisterComponents();
            expected.SetReferenceHostDocument();

            var expectedSerializedDoc = 
"""
openapi: 3.0.4
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
        default: 10
""";

            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minifiedPetStore.yaml"));

            // Act
            var doc = (await OpenApiDocument.LoadAsync(stream, settings: SettingsFixture.ReaderSettings)).Document;
            var actualParam = doc.Paths["/pets"].Operations[HttpMethod.Get].Parameters[0];
            var outputDoc = (await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0)).MakeLineBreaksEnvironmentNeutral();
            var expectedParam = expected.Paths["/pets"].Operations[HttpMethod.Get].Parameters[0];
            var expectedParamReference = Assert.IsType<OpenApiParameterReference>(expectedParam);

            var actualParamReference = Assert.IsType<OpenApiParameterReference>(actualParam);

            // Assert
            actualParamReference.Should().BeEquivalentTo(expectedParamReference, options => options
                .Excluding(x => x.Reference)
                .Excluding(x => x.Target)
                .Excluding(x => x.RecursiveTarget)
                .Excluding(x => x.Schema.Default.Parent)
                .Excluding(x => x.Schema.Default.Options)
                .IgnoringCyclicReferences());
            Assert.Equal(expectedSerializedDoc.MakeLineBreaksEnvironmentNeutral(), outputDoc);
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
                                                            """, "yaml", SettingsFixture.ReaderSettings);

            var expected = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "The API",
                    Version = "0.9.1",
                },
                Servers =
                [
                    new OpenApiServer
                    {
                        Url = "http://www.example.org/api/{version}",
                        Description = "The http endpoint",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            {"version", new OpenApiServerVariable {Default = "v2", Enum = ["v1", "v2"]}}
                        }
                    }
                ],
                Paths = new()
            };

            Assert.Equivalent(
                new OpenApiDiagnostic 
                { 
                    SpecificationVersion = OpenApiSpecVersion.OpenApi3_0
                }, result.Diagnostic);

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
                                                            """, "yaml", SettingsFixture.ReaderSettings);

            Assert.NotEmpty(result.Diagnostic.Errors);
        }

        [Fact]
        public async Task ParseDocumentWithEmptyPathsSucceeds()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithEmptyPaths.yaml"), SettingsFixture.ReaderSettings);
            Assert.Empty(result.Diagnostic.Errors);
        }

        [Fact]
        public async Task ParseDocumentWithExampleReferencesPasses()
        {
            // Act & Assert: Ensure no NullReferenceException is thrown
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "docWithExampleReferences.yaml"), SettingsFixture.ReaderSettings);
            Assert.Empty(result.Diagnostic.Errors);
        }

        [Fact]
        public async Task ParseDocumentWithNonStandardMIMETypePasses()
        {
            var path = Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml");
            using var stream = Resources.GetStream(path);
            using var streamReader = new StreamReader(stream);
            var contentAsString = await streamReader.ReadToEndAsync();
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(contentAsString, new MediaTypeHeaderValue("text/x-yaml"))
                });
            var settings = new OpenApiReaderSettings
            {
                HttpClient = new HttpClient(mockMessageHandler.Object)
            };
            settings.AddYamlReader();
            // Act & Assert: Ensure NotSupportedException is not thrown for non-standard MIME type: text/x-yaml
            var result = await OpenApiDocument.LoadAsync("https://localhost/doesntmatter/foo.bar", settings);
            Assert.NotNull(result.Document); 
        }
    }
}
