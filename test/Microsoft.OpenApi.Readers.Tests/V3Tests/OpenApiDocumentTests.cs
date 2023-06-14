﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Rules;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        private readonly ITestOutputHelper _output;

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
                    return new OpenApiStringReader().ReadFragment<T>(result, OpenApiSpecVersion.OpenApi3_0, out OpenApiDiagnostic diagnostic4);
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
                    return new OpenApiStringReader().ReadFragment<OpenApiSecurityScheme>(result, OpenApiSpecVersion.OpenApi3_0, out OpenApiDiagnostic diagnostic4);
                }
            }
        }


        public OpenApiDocumentTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseDocumentFromInlineStringShouldSucceed()
        {
            var openApiDoc = new OpenApiStringReader().Read(
                @"
openapi : 3.0.0
info:
    title: Simple Document
    version: 0.9.1
paths: {}",
                out var context);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Paths = new OpenApiPaths()
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

        [Theory]
        [InlineData("en-US")]
        [InlineData("hi-IN")]
        // The equivalent of English 1,000.36 in French and Danish is 1.000,36
        [InlineData("fr-FR")]
        [InlineData("da-DK")]
        public void ParseDocumentWithDifferentCultureShouldSucceed(string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            var openApiDoc = new OpenApiStringReader().Read(
                @"
openapi : 3.0.0
info:
    title: Simple Document
    version: 0.9.1
components:
  schemas:
    sampleSchema:
      type: object
      properties:
        sampleProperty:
          type: double
          minimum: 100.54
          maximum: 60000000.35
          exclusiveMaximum: true
          exclusiveMinimum: false
paths: {}",
                out var context);

            openApiDoc.Should().BeEquivalentTo(
                new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Title = "Simple Document",
                        Version = "0.9.1"
                    },
                    Components = new OpenApiComponents()
                    {
                        Schemas =
                        {
                            ["sampleSchema"] = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["sampleProperty"] = new OpenApiSchema()
                                    {
                                        Type = "double",
                                        Minimum = (decimal)100.54,
                                        Maximum = (decimal)60000000.35,
                                        ExclusiveMaximum = true,
                                        ExclusiveMinimum = false
                                    }
                                },
                                Reference = new OpenApiReference()
                                {
                                    Id = "sampleSchema",
                                    Type = ReferenceType.Schema
                                }
                            }
                        }
                    },
                    Paths = new OpenApiPaths()
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
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicDocumentWithMultipleServers.yaml")))
            {
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
                    });
            }
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
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minimalDocument.yaml")))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

                openApiDoc.Should().BeEquivalentTo(
                    new OpenApiDocument
                    {
                        Info = new OpenApiInfo
                        {
                            Title = "Simple Document",
                            Version = "0.9.1"
                        },
                        Paths = new OpenApiPaths()
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
                        },
                        ["errorModel"] = new OpenApiSchema
                        {
                            Type = "object",
                            Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["code"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Format = "int32"
                                },
                                ["message"] = new OpenApiSchema
                                {
                                    Type = "string"
                                }
                            },
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "errorModel",
                                HostDocument = actual
                            }
                        },
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

                var errorModelSchema = Clone(components.Schemas["errorModel"]);

                errorModelSchema.Reference = new OpenApiReference
                {
                    Id = "errorModel",
                    Type = ReferenceType.Schema,
                    HostDocument = actual
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
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer",
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
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer",
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

                actual.Should().BeEquivalentTo(expected);
            }

            context.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
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
                        },
                        ["errorModel"] = new OpenApiSchema
                        {
                            Type = "object",
                            Required = new HashSet<string>
                            {
                                "code",
                                "message"
                            },
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["code"] = new OpenApiSchema
                                {
                                    Type = "integer",
                                    Format = "int32"
                                },
                                ["message"] = new OpenApiSchema
                                {
                                    Type = "string"
                                }
                            },
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.Schema,
                                Id = "errorModel"
                            }
                        },
                    },
                    SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["securitySchemeName1"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.ApiKey,
                            Name = "apiKeyName1",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "securitySchemeName1",
                                Type = ReferenceType.SecurityScheme,
                                HostDocument = actual
                            }

                        },
                        ["securitySchemeName2"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OpenIdConnect,
                            OpenIdConnectUrl = new Uri("http://example.com"),
                            Reference = new OpenApiReference
                            {
                                Id = "securitySchemeName2",
                                Type = ReferenceType.SecurityScheme,
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
                    Type = ReferenceType.Schema
                };

                var newPetSchema = Clone(components.Schemas["newPet"]);

                newPetSchema.Reference = new OpenApiReference
                {
                    Id = "newPet",
                    Type = ReferenceType.Schema
                };

                var errorModelSchema = Clone(components.Schemas["errorModel"]);

                errorModelSchema.Reference = new OpenApiReference
                {
                    Id = "errorModel",
                    Type = ReferenceType.Schema
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
                    Name = "tagName2"
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
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer",
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
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "integer",
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
                            Description = "tagDescription1",
                            Reference = new OpenApiReference()
                            {
                                Id = "tagName1",
                                Type = ReferenceType.Tag
                            }
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

                actual.Should().BeEquivalentTo(expected, options => options.Excluding(m => m.Name == "HostDocument"));
            }

            context.Should().BeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
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
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });
        }

        [Fact]
        public void GlobalSecurityRequirementShouldReferenceSecurityScheme()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "securedApi.yaml")))
            {
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

                var securityRequirement = openApiDoc.SecurityRequirements.First();

                Assert.Same(securityRequirement.Keys.First(), openApiDoc.Components.SecuritySchemes.First().Value);
            }
        }
        
        [Fact]
        public void HeaderParameterShouldAllowExample()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "apiWithFullHeaderComponent.yaml")))
            {
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
                        Schema = new OpenApiSchema()
                        {
                            Type = "string",
                            Format = "uuid"
                        },
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
                        Schema = new OpenApiSchema()
                        {
                            Type = "string",
                            Format = "uuid"
                        },
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.Header,
                            Id = "examples-header"
                        }
                    }, options => options.IgnoringCyclicReferences()
                    .Excluding(e => e.Examples["uuid1"].Value.Node.Parent)
                    .Excluding(e => e.Examples["uuid2"].Value.Node.Parent));
            }
        }

        [Fact]
        public void DoesNotChangeExternalReferences()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "documentWithExternalRefs.yaml"));

            // Act
            var doc = new OpenApiStreamReader(
                new OpenApiReaderSettings { ReferenceResolution = ReferenceResolutionSetting.DoNotResolveReferences })
                .Read(stream, out var diagnostic);

            var externalRef = doc.Components.Schemas["Nested"].Properties["AnyOf"].AnyOf.First().Reference.ReferenceV3;
            var externalRef2 = doc.Components.Schemas["Nested"].Properties["AnyOf"].AnyOf.Last().Reference.ReferenceV3;

            // Assert
            Assert.Equal("file:///C:/MySchemas.json#/definitions/ArrayObject", externalRef);
            Assert.Equal("../foo/schemas.yaml#/components/schemas/Number", externalRef2);
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
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1 });
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ParseDocumentsWithReusablePathItemInWebhooksSucceeds()
        {
            // Arrange && Act
            using var stream = Resources.GetStream("V3Tests/Samples/OpenApiDocument/documentWithReusablePaths.yaml");
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
    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_1});

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
