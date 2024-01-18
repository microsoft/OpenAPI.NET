// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        public static OpenApiComponents TopLevelReferencingComponents = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema2"
                    }
                },
                ["schema2"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
                ["schema2"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                }
            }
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelReferencingComponents = new()
        {
            Info = new()
            {
                Version = "1.0.0"
            },
            Components = TopLevelReferencingComponents
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponentsWithOtherProperties = new()
        {
            Info = new()
            {
                Version = "1.0.0"
            },
            Components = TopLevelSelfReferencingComponentsWithOtherProperties
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponents = new()
        {
            Info = new()
            {
                Version = "1.0.0"
            },
            Components = TopLevelSelfReferencingComponents
        };

        public static OpenApiComponents AdvancedComponentsWithReference = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["pet"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "id",
                        "name"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["id"] = new()
                        {
                            Type = "integer",
                            Format = "int64"
                        },
                        ["name"] = new()
                        {
                            Type = "string"
                        },
                        ["tag"] = new()
                        {
                            Type = "string"
                        },
                    },
                    Reference = new()
                    {
                        Id = "pet",
                        Type = ReferenceType.Schema
                    }
                },
                ["newPet"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "name"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["id"] = new()
                        {
                            Type = "integer",
                            Format = "int64"
                        },
                        ["name"] = new()
                        {
                            Type = "string"
                        },
                        ["tag"] = new()
                        {
                            Type = "string"
                        },
                    },
                    Reference = new()
                    {
                        Id = "newPet",
                        Type = ReferenceType.Schema
                    }
                },
                ["errorModel"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "code",
                        "message"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["code"] = new()
                        {
                            Type = "integer",
                            Format = "int32"
                        },
                        ["message"] = new()
                        {
                            Type = "string"
                        }
                    },
                    Reference = new()
                    {
                        Id = "errorModel",
                        Type = ReferenceType.Schema
                    }
                },
            }
        };

        public static OpenApiSchema PetSchemaWithReference = AdvancedComponentsWithReference.Schemas["pet"];

        public static OpenApiSchema NewPetSchemaWithReference = AdvancedComponentsWithReference.Schemas["newPet"];

        public static OpenApiSchema ErrorModelSchemaWithReference =
            AdvancedComponentsWithReference.Schemas["errorModel"];

        public static OpenApiDocument AdvancedDocumentWithReference = new()
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
                                    Schema = new()
                                    {
                                        Type = "array",
                                        Items = new()
                                        {
                                            Type = "string"
                                        }
                                    }
                                },
                                new()
                                {
                                    Name = "limit",
                                    In = ParameterLocation.Query,
                                    Description = "maximum number of results to return",
                                    Required = false,
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int32"
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
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchemaWithReference
                                            }
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchemaWithReference
                                            }
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                        Schema = NewPetSchemaWithReference
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
                                            Schema = PetSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
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
                                            Schema = PetSchemaWithReference
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = PetSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
                                    }
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Components = AdvancedComponentsWithReference
        };

        public static OpenApiComponents AdvancedComponents = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["pet"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "id",
                        "name"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["id"] = new()
                        {
                            Type = "integer",
                            Format = "int64"
                        },
                        ["name"] = new()
                        {
                            Type = "string"
                        },
                        ["tag"] = new()
                        {
                            Type = "string"
                        },
                    }
                },
                ["newPet"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "name"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["id"] = new()
                        {
                            Type = "integer",
                            Format = "int64"
                        },
                        ["name"] = new()
                        {
                            Type = "string"
                        },
                        ["tag"] = new()
                        {
                            Type = "string"
                        },
                    }
                },
                ["errorModel"] = new()
                {
                    Type = "object",
                    Required = new HashSet<string>
                    {
                        "code",
                        "message"
                    },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["code"] = new()
                        {
                            Type = "integer",
                            Format = "int32"
                        },
                        ["message"] = new()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiSchema PetSchema = AdvancedComponents.Schemas["pet"];

        public static OpenApiSchema NewPetSchema = AdvancedComponents.Schemas["newPet"];

        public static OpenApiSchema ErrorModelSchema = AdvancedComponents.Schemas["errorModel"];

        public OpenApiDocument AdvancedDocument = new()
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
                                    Schema = new()
                                    {
                                        Type = "array",
                                        Items = new()
                                        {
                                            Type = "string"
                                        }
                                    }
                                },
                                new()
                                {
                                    Name = "limit",
                                    In = ParameterLocation.Query,
                                    Description = "maximum number of results to return",
                                    Required = false,
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int32"
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
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchema
                                            }
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                        Schema = NewPetSchema
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
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
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
                                            Schema = PetSchema
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
                                    }
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Components = AdvancedComponents
        };

        public static OpenApiDocument DuplicateExtensions = new()
        {
            Info = new()
            {
                Version = "1.0.0",
                Title = "Swagger Petstore (Simple)",
                Description = "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
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
                ["/add/{operand1}/{operand2}"] = new()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            OperationId = "addByOperand1AndByOperand2",
                            Parameters = new List<OpenApiParameter>
                            {
                                new()
                                {
                                    Name = "operand1",
                                    In = ParameterLocation.Path,
                                    Description = "The first operand",
                                    Required = true,
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Extensions = new Dictionary<string, IOpenApiExtension>
                                        {
                                            ["my-extension"] = new Any.OpenApiInteger(4),
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["my-extension"] = new Any.OpenApiInteger(4),
                                    }
                                },
                                new()
                                {
                                    Name = "operand2",
                                    In = ParameterLocation.Path,
                                    Description = "The second operand",
                                    Required = true,
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Extensions = new Dictionary<string, IOpenApiExtension>
                                        {
                                            ["my-extension"] = new Any.OpenApiInteger(4),
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["my-extension"] = new Any.OpenApiInteger(4),
                                    }
                                },
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
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchema
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        public OpenApiDocument AdvancedDocumentWithServerVariable = new()
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
                    Url = "https://{endpoint}/openai",
                    Variables = new Dictionary<string, OpenApiServerVariable>
                    {
                        ["endpoint"] = new()
                        {
                            Default = "your-resource-name.openai.azure.com"
                        }
                    }
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
                                    Schema = new()
                                    {
                                        Type = "array",
                                        Items = new()
                                        {
                                            Type = "string"
                                        }
                                    }
                                },
                                new()
                                {
                                    Name = "limit",
                                    In = ParameterLocation.Query,
                                    Description = "maximum number of results to return",
                                    Required = false,
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int32"
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
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = new()
                                            {
                                                Type = "array",
                                                Items = PetSchema
                                            }
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                        Schema = NewPetSchema
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
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
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
                                            Schema = PetSchema
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                    Schema = new()
                                    {
                                        Type = "integer",
                                        Format = "int64"
                                    }
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Components = AdvancedComponents
        };

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SerializeAdvancedDocumentAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedDocument.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithReferenceAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedDocumentWithReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithServerVariableAsV2JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedDocumentWithServerVariable.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentAsV2JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedDocument.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeDuplicateExtensionsAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            DuplicateExtensions.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeDuplicateExtensionsAsV2JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            DuplicateExtensions.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithReferenceAsV2JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedDocumentWithReference.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeSimpleDocumentWithTopLevelReferencingComponentsAsYamlV2Works()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                paths: { }
                definitions:
                  schema1:
                    $ref: '#/definitions/schema2'
                  schema2:
                    type: object
                    properties:
                      property1:
                        type: string
                """;

            // Act
            var actual = SimpleDocumentWithTopLevelReferencingComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSimpleDocumentWithTopLevelSelfReferencingComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                paths: { }
                definitions:
                  schema1: { }
                """;

            // Act
            var actual = SimpleDocumentWithTopLevelSelfReferencingComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSimpleDocumentWithTopLevelSelfReferencingWithOtherPropertiesComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                paths: { }
                definitions:
                  schema1:
                    type: object
                    properties:
                      property1:
                        type: string
                  schema2:
                    type: object
                    properties:
                      property1:
                        type: string
                """;

            // Act
            var actual = SimpleDocumentWithTopLevelSelfReferencingComponentsWithOtherProperties.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeDocumentWithReferenceButNoComponents()
        {
            // Arrange
            var document = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "Test",
                    Version = "1.0.0"
                },
                Paths = new()
                {
                    ["/"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new()
                            {
                                Responses = new()
                                {
                                    ["200"] = new()
                                    {
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Reference = new()
                                                    {
                                                        Id = "test",
                                                        Type = ReferenceType.Schema
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var reference = document.Paths["/"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema.Reference;

            // Act
            var actual = document.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void SerializeRelativePathAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                basePath: /server1
                paths: { }
                """;
            var doc = new OpenApiDocument
            {
                Info = new() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = "/server1"
                    }
                }
            };

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeRelativePathWithHostAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                host: //example.org
                basePath: /server1
                paths: { }
                """;
            var doc = new OpenApiDocument
            {
                Info = new() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = "//example.org/server1"
                    }
                }
            };

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeRelativeRootPathWithHostAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info:
                  version: 1.0.0
                host: //example.org
                paths: { }
                """;
            var doc = new OpenApiDocument
            {
                Info = new() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = "//example.org/"
                    }
                }
            };

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TestHashCodesForSimilarOpenApiDocuments()
        {
            // Arrange
            var sampleFolderPath = "Models/Samples/";

            var doc1 = ParseInputFile(Path.Combine(sampleFolderPath, "sampleDocument.yaml"));
            var doc2 = ParseInputFile(Path.Combine(sampleFolderPath, "sampleDocument.yaml"));
            var doc3 = ParseInputFile(Path.Combine(sampleFolderPath, "sampleDocumentWithWhiteSpaces.yaml"));

            // Act && Assert
            /*
                Test whether reading in two similar documents yield the same hash code,
                And reading in similar documents(one has a whitespace) yields the same hash code as the result is terse
            */
            Assert.True(doc1.HashCode != null && doc2.HashCode != null && doc1.HashCode.Equals(doc2.HashCode));
            Assert.Equal(doc1.HashCode, doc3.HashCode);
        }

        private static OpenApiDocument ParseInputFile(string filePath)
        {
            // Read in the input yaml file
            using var stream = File.OpenRead(filePath);
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            return openApiDoc;
        }

        //[Fact]
        //public void CopyConstructorForAdvancedDocumentWorks()
        //{
        //    // Arrange & Act
        //    var doc = new OpenApiDocument(AdvancedDocument);

        //    var docOpId = doc.Paths["/pets"].Operations[OperationType.Get].OperationId = "findAllMyPets";
        //    var advancedDocOpId = AdvancedDocument.Paths["/pets"].Operations[OperationType.Get].OperationId;
        //    var responseSchemaTypeCopy = doc.Paths["/pets"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema.Type = "object";
        //    var advancedDocResponseSchemaType = AdvancedDocument.Paths["/pets"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema.Type;

        //    // Assert
        //    Assert.NotNull(doc.Info);
        //    Assert.NotNull(doc.Servers);
        //    Assert.NotNull(doc.Paths);
        //    Assert.Equal(2, doc.Paths.Count);
        //    Assert.NotNull(doc.Components);
        //    Assert.NotEqual(docOpId, advancedDocOpId);
        //    Assert.NotEqual(responseSchemaTypeCopy, advancedDocResponseSchemaType);
        //}

        [Fact]
        public void SerializeV2DocumentWithNonArraySchemaTypeDoesNotWriteOutCollectionFormat()
        {
            // Arrange
            var expected =
                """
                swagger: '2.0'
                info: { }
                paths:
                  /foo:
                    get:
                      parameters:
                        - in: query
                          type: string
                      responses: { }
                """;

            var doc = new OpenApiDocument
            {
                Info = new(),
                Paths = new()
                {
                    ["/foo"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new()
                            {
                                Parameters = new List<OpenApiParameter>
                                {
                                    new()
                                    {
                                        In = ParameterLocation.Query,
                                        Schema = new()
                                        {
                                            Type = "string"
                                        }
                                    }
                                },
                                Responses = new()
                            }
                        }
                    }
                }
            };

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeV2DocumentWithStyleAsNullDoesNotWriteOutStyleValue()
        {
            // Arrange
            var expected =
                """
                openapi: 3.0.1
                info:
                  title: magic style
                  version: 1.0.0
                paths:
                  /foo:
                    get:
                      parameters:
                        - name: id
                          in: query
                          schema:
                            type: object
                            additionalProperties:
                              type: integer
                      responses:
                        '200':
                          description: foo
                          content:
                            text/plain:
                              schema:
                                type: string
                """;

            var doc = new OpenApiDocument
            {
                Info = new()
                {
                    Title = "magic style",
                    Version = "1.0.0"
                },
                Paths = new()
                {
                    ["/foo"] = new()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new()
                            {
                                Parameters = new List<OpenApiParameter>
                                {
                                    new()
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Query,
                                        Schema = new()
                                        {
                                            Type = "object",
                                            AdditionalProperties = new()
                                            {
                                                Type = "integer"
                                            }
                                        }
                                    }
                                },
                                Responses = new()
                                {
                                    ["200"] = new()
                                    {
                                        Description = "foo",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/plain"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Type = "string"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
