﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class FullDocumentReaderTests
    {
        private readonly ITestOutputHelper _output;

        public FullDocumentReaderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReadingStandardPetStoreDocumentShouldSucceed()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(
                    typeof(FullDocumentReaderTests),
                    "Samples.PetStore30.yaml");

            var actual = new OpenApiStreamReader().Read(stream, out var context);


            var components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, OpenApiSchema>()
                {
                    ["pet"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "id",
                            "name"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                        }
                    },
                    ["newPet"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "name"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                        }
                    },
                    ["errorModel"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "code",
                            "message"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["code"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int32"
                            },
                            ["message"] = new OpenApiSchema()
                            {
                                Type = "string"
                            }
                        }
                    },
                }
            };

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(JsonConvert.SerializeObject(components.Schemas["pet"]));
            petSchema.Reference = new OpenApiReference()
            {
                Id = "pet",
                Type = ReferenceType.Schema
            };

            var newPetSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(JsonConvert.SerializeObject(components.Schemas["newPet"]));
            newPetSchema.Reference = new OpenApiReference()
            {
                Id = "newPet",
                Type = ReferenceType.Schema
            };

            var errorModelSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(
                    JsonConvert.SerializeObject(components.Schemas["errorModel"]));
            errorModelSchema.Reference = new OpenApiReference()
            {
                Id = "errorModel",
                Type = ReferenceType.Schema
            };

            var expected = new OpenApiDocument()
            {
                SpecVersion = new Version(3, 0, 0),
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                    Description =
                        "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                    TermsOfService = new Uri("http://helloreverb.com/terms/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Swagger API team",
                        Email = "foo@example.com",
                        Url = new Uri("http://swagger.io")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT")
                    }
                },
                Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer()
                    {
                        Url = "http://petstore.swagger.io/api"
                    }
                },
                Paths = new OpenApiPaths()
                {
                    ["/pets"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation()
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "tags",
                                        In = ParameterLocation.Query,
                                        Description = "tags to filter by",
                                        Required = false,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema()
                                            {
                                                Type = "string"
                                            }
                                        }
                                    },
                                    new OpenApiParameter()
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "maximum number of results to return",
                                        Required = false,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation()
                            {
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody()
                                {
                                    Description = "Pet to add to the store",
                                    Required = true,
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType()
                                        {
                                            Schema = newPetSchema
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            },
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/pets/{id}"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation()
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to fetch",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int64"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            },
                                            ["application/xml"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Delete] = new OpenApiOperation()
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to delete",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int64"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["204"] = new OpenApiResponse()
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
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
            
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ReadingModifiedPetStoreDocumentWithTagAndSecurityShouldSucceed()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(
                    typeof(FullDocumentReaderTests),
                    "Samples.PetStoreWithTagAndSecurity30.yaml");

            var actual = new OpenApiStreamReader().Read(stream, out var context);

            var components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, OpenApiSchema>()
                {
                    ["pet"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "id",
                            "name"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                        }
                    },
                    ["newPet"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "name"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["id"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int64"
                            },
                            ["name"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                        }
                    },
                    ["errorModel"] = new OpenApiSchema()
                    {
                        Type = "object",
                        Required = new List<string>()
                        {
                            "code",
                            "message"
                        },
                        Properties = new Dictionary<string, OpenApiSchema>()
                        {
                            ["code"] = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int32"
                            },
                            ["message"] = new OpenApiSchema()
                            {
                                Type = "string"
                            }
                        }
                    },
                },
                SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
                {
                    ["securitySchemeName1"] = new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "apiKeyName1",
                        In = ParameterLocation.Header
                    },
                    ["securitySchemeName2"] = new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.OpenIdConnect,
                        OpenIdConnectUrl = new Uri("http://example.com")
                    }
                }
            };

            // Create a clone of the schema to avoid modifying things in components.
            var petSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(JsonConvert.SerializeObject(components.Schemas["pet"]));
            petSchema.Reference = new OpenApiReference()
            {
                Id = "pet",
                Type = ReferenceType.Schema
            };

            var newPetSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(JsonConvert.SerializeObject(components.Schemas["newPet"]));
            newPetSchema.Reference = new OpenApiReference()
            {
                Id = "newPet",
                Type = ReferenceType.Schema
            };

            var errorModelSchema =
                JsonConvert.DeserializeObject<OpenApiSchema>(
                    JsonConvert.SerializeObject(components.Schemas["errorModel"]));
            errorModelSchema.Reference = new OpenApiReference()
            {
                Id = "errorModel",
                Type = ReferenceType.Schema
            };


            var tag1 = new OpenApiTag()
            {
                Name = "tagName1",
                Description = "tagDescription1"
            };

            tag1.Reference = new OpenApiReference()
            {
                Id = "tagName1",
                Type = ReferenceType.Tag
            };

            var tag2 = new OpenApiTag()
            {
                Name = "tagName2"
            };

            var securityScheme1 = JsonConvert.DeserializeObject<OpenApiSecurityScheme>(
                JsonConvert.SerializeObject(components.SecuritySchemes["securitySchemeName1"]));
            securityScheme1.Reference = new OpenApiReference()
            {
                Id = "securitySchemeName1",
                Type = ReferenceType.SecurityScheme
            };

            var securityScheme2 = JsonConvert.DeserializeObject<OpenApiSecurityScheme>(
                JsonConvert.SerializeObject(components.SecuritySchemes["securitySchemeName2"]));
            securityScheme2.Reference = new OpenApiReference()
            {
                Id = "securitySchemeName2",
                Type = ReferenceType.SecurityScheme
            };

            var expected = new OpenApiDocument()
            {
                SpecVersion = new Version(3, 0, 0),
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)",
                    Description =
                        "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                    TermsOfService = new Uri("http://helloreverb.com/terms/"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Swagger API team",
                        Email = "foo@example.com",
                        Url = new Uri("http://swagger.io")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT")
                    }
                },
                Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer()
                    {
                        Url = "http://petstore.swagger.io/api"
                    }
                },
                Paths = new OpenApiPaths()
                {
                    ["/pets"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation()
                            {
                                Tags = new List<OpenApiTag>()
                                {
                                    tag1,
                                    tag2
                                },
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "tags",
                                        In = ParameterLocation.Query,
                                        Description = "tags to filter by",
                                        Required = false,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema()
                                            {
                                                Type = "string"
                                            }
                                        }
                                    },
                                    new OpenApiParameter()
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "maximum number of results to return",
                                        Required = false,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            },
                                            ["application/xml"] = new OpenApiMediaType()
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = "array",
                                                    Items = petSchema
                                                }
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Post] = new OpenApiOperation()
                            {
                                Tags = new List<OpenApiTag>()
                                {
                                    tag1,
                                    tag2
                                },
                                Description = "Creates a new pet in the store.  Duplicates are allowed",
                                OperationId = "addPet",
                                RequestBody = new OpenApiRequestBody()
                                {
                                    Description = "Pet to add to the store",
                                    Required = true,
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType()
                                        {
                                            Schema = newPetSchema
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            },
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                },
                                Security = new List<OpenApiSecurityRequirement>()
                                {
                                    new OpenApiSecurityRequirement()
                                    {
                                        [securityScheme1] = new List<string>(),
                                        [securityScheme2] = new List<string>()
                                        {
                                            "scope1",
                                            "scope2"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    ["/pets/{id}"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Get] = new OpenApiOperation()
                            {
                                Description =
                                    "Returns a user based on a single ID, if the user does not have access to the pet",
                                OperationId = "findPetById",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to fetch",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int64"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["200"] = new OpenApiResponse()
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            },
                                            ["application/xml"] = new OpenApiMediaType()
                                            {
                                                Schema = petSchema
                                            }
                                        }
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    }
                                }
                            },
                            [OperationType.Delete] = new OpenApiOperation()
                            {
                                Description = "deletes a single pet based on the ID supplied",
                                OperationId = "deletePet",
                                Parameters = new List<OpenApiParameter>()
                                {
                                    new OpenApiParameter()
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Path,
                                        Description = "ID of pet to delete",
                                        Required = true,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int64"
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
                                {
                                    ["204"] = new OpenApiResponse()
                                    {
                                        Description = "pet deleted"
                                    },
                                    ["4XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected client error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
                                            {
                                                Schema = errorModelSchema
                                            }
                                        }
                                    },
                                    ["5XX"] = new OpenApiResponse()
                                    {
                                        Description = "unexpected server error",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["text/html"] = new OpenApiMediaType()
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
                Tags = new List<OpenApiTag>()
                {
                    new OpenApiTag()
                    {
                        Name = "tagName1",
                        Description = "tagDescription1"
                    }
                },
                SecurityRequirements = new List<OpenApiSecurityRequirement>()
                {
                    new OpenApiSecurityRequirement()
                    {
                        [securityScheme1] = new List<string>(),
                        [securityScheme2] = new List<string>()
                        {
                            "scope1",
                            "scope2",
                            "scope3"
                        }
                    }
                }
            };
            
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}