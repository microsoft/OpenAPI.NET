// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        public OpenApiDocumentTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        public static readonly OpenApiComponents TopLevelReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchemaReference("schema2", null),
                ["schema2"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String,
                            Annotations = new Dictionary<string, object> { { "key1", "value" } }
                        }
                    }
                },
            }
        };

        public static readonly OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String,
                            Annotations = new Dictionary<string, object> { { "key1", "value" } }
                        }
                    },
                    Annotations = new Dictionary<string, object> { { "key1", "value" } },
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
                ["schema2"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                },
            }

        };

        public static readonly OpenApiComponents TopLevelSelfReferencingComponents = new OpenApiComponents()
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

        public static readonly OpenApiDocument SimpleDocumentWithTopLevelReferencingComponents = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = TopLevelReferencingComponents
        };

        public static readonly OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = TopLevelSelfReferencingComponentsWithOtherProperties
        };

        public static readonly OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponents = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = TopLevelSelfReferencingComponents
        };

        public static readonly OpenApiComponents AdvancedComponentsWithReference = new OpenApiComponents
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["pet"] = new()
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

        public static OpenApiSchema PetSchemaWithReference = AdvancedComponentsWithReference.Schemas["pet"];

        public static OpenApiSchema NewPetSchemaWithReference = AdvancedComponentsWithReference.Schemas["newPet"];

        public static OpenApiSchema ErrorModelSchemaWithReference =
            AdvancedComponentsWithReference.Schemas["errorModel"];

        public static readonly OpenApiDocument AdvancedDocumentWithReference = new OpenApiDocument
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
                                                Items = PetSchemaWithReference
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                        Schema = NewPetSchemaWithReference
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
                                            Schema = PetSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = PetSchemaWithReference
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = PetSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
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
                                            Schema = ErrorModelSchemaWithReference
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = AdvancedComponentsWithReference
        };

        public static readonly OpenApiComponents AdvancedComponents = new OpenApiComponents
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["pet"] = new()
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

        public static readonly OpenApiSchema PetSchema = AdvancedComponents.Schemas["pet"];

        public static readonly OpenApiSchema NewPetSchema = AdvancedComponents.Schemas["newPet"];

        public static readonly OpenApiSchema ErrorModelSchema = AdvancedComponents.Schemas["errorModel"];

        public OpenApiDocument AdvancedDocument = new OpenApiDocument
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
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                        Schema = NewPetSchema
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
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = PetSchema
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = PetSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
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
                                            Schema = ErrorModelSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = AdvancedComponents
        };

        public static readonly OpenApiDocument DocumentWithWebhooks = new OpenApiDocument()
        {
            Info = new OpenApiInfo
            {
                Title = "Webhook Example",
                Version = "1.0.0"
            },
            Webhooks = new Dictionary<string, OpenApiPathItem>
            {
                ["newPet"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Information about a new pet in the system",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("Pet", null)
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Return a 200 status to indicate that the data was received successfully"
                                }
                            }
                        }
                    }
                }
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema()
                    {
                        Required = new HashSet<string>
                        {
                           "id", "name"
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
                        },
                    }
                }
            }
        };

        public static readonly OpenApiDocument DuplicateExtensions = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Version = "1.0.0",
                Title = "Swagger Petstore (Simple)",
                Description = "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
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
                ["/add/{operand1}/{operand2}"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            OperationId = "addByOperand1AndByOperand2",
                            Parameters = new List<OpenApiParameter>
                            {
                                new OpenApiParameter
                                {
                                    Name = "operand1",
                                    In = ParameterLocation.Path,
                                    Description = "The first operand",
                                    Required = true,
                                    Schema = new()
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Extensions = new Dictionary<string, IOpenApiExtension>
                                        {
                                            ["my-extension"] = new OpenApiAny(4)
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["my-extension"] = new OpenApiAny(4),
                                    }
                                },
                                new OpenApiParameter
                                {
                                    Name = "operand2",
                                    In = ParameterLocation.Path,
                                    Description = "The second operand",
                                    Required = true,
                                    Schema = new()
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Extensions = new Dictionary<string, IOpenApiExtension>
                                        {
                                            ["my-extension"] = new OpenApiAny(4)
                                        }
                                    },
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["my-extension"] = new OpenApiAny(4),
                                    }
                                },
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
                                                Items = PetSchema
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = AdvancedComponents
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
                                        Type = JsonSchemaType.Array,
                                        Items = new()
                                        {
                                            Type = JsonSchemaType.String
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
                                        Type = JsonSchemaType.Integer,
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
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = new()
                                            {
                                                Type = JsonSchemaType.Array,
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
                                        Type = JsonSchemaType.Integer,
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
                                        Type = JsonSchemaType.Integer,
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
            Annotations = new Dictionary<string, object> { { "key1", "value" } },
            Components = AdvancedComponents
        };

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SerializeAdvancedDocumentAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedDocument.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithReferenceAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedDocumentWithReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithServerVariableAsV2JsonWorksAsync(bool produceTerseOutput)
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
        public async Task SerializeAdvancedDocumentAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedDocument.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeDuplicateExtensionsAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            DuplicateExtensions.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeDuplicateExtensionsAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            DuplicateExtensions.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedDocumentWithReferenceAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

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
            var expected = @"swagger: '2.0'
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
        type: string";

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
            var expected = @"swagger: '2.0'
info:
  version: 1.0.0
paths: { }
definitions:
  schema1: { }";

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
            var expected = @"swagger: '2.0'
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
        type: string";

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
            var document = new OpenApiDocument()
            {
                Info = new OpenApiInfo
                {
                    Title = "Test",
                    Version = "1.0.0"
                },
                Paths = new OpenApiPaths
                {
                    ["/"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType
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
                @"swagger: '2.0'
info:
  version: 1.0.0
basePath: /server1
paths: { }";
            var doc = new OpenApiDocument()
            {
                Info = new OpenApiInfo() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>() {
                    new OpenApiServer()
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
                @"swagger: '2.0'
info:
  version: 1.0.0
host: //example.org
basePath: /server1
paths: { }";
            var doc = new OpenApiDocument()
            {
                Info = new OpenApiInfo() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>() {
                    new OpenApiServer()
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
                @"swagger: '2.0'
info:
  version: 1.0.0
host: //example.org
paths: { }";
            var doc = new OpenApiDocument()
            {
                Info = new OpenApiInfo() { Version = "1.0.0" },
                Servers = new List<OpenApiServer>() {
                    new OpenApiServer()
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
            using FileStream stream = File.OpenRead(filePath);
            var format = OpenApiModelFactory.GetFormat(filePath);
            var openApiDoc = OpenApiDocument.Load(stream, format).Document;

            return openApiDoc;
        }

        [Fact]
        public void SerializeV2DocumentWithNonArraySchemaTypeDoesNotWriteOutCollectionFormat()
        {
            // Arrange
            var expected = @"swagger: '2.0'
info: { }
paths:
  /foo:
    get:
      parameters:
        - in: query
          type: string
      responses: { }";

            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo(),
                Paths = new OpenApiPaths
                {
                    ["/foo"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Parameters = new List<OpenApiParameter>
                                {
                                    new OpenApiParameter
                                    {
                                        In = ParameterLocation.Query,
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.String
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses()
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
            var expected = @"openapi: 3.0.4
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
                type: string";

            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "magic style",
                    Version = "1.0.0"
                },
                Paths = new OpenApiPaths
                {
                    ["/foo"] = new OpenApiPathItem
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>
                        {
                            [OperationType.Get] = new OpenApiOperation
                            {
                                Parameters = new List<OpenApiParameter>
                                {
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Query,
                                        Schema = new()
                                        {
                                            Type = JsonSchemaType.Object,
                                            AdditionalProperties = new()
                                            {
                                                Type = JsonSchemaType.Integer
                                            }
                                        }
                                    }
                                },
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "foo",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/plain"] = new OpenApiMediaType
                                            {
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.String
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

        [Fact]
        public void OpenApiDocumentCopyConstructorWithAnnotationsSucceeds()
        {
            var baseDocument = new OpenApiDocument
            {
                Annotations = new Dictionary<string, object>
                {
                    ["key1"] = "value1",
                    ["key2"] = 2
                }
            };

            var actualDocument = new OpenApiDocument(baseDocument);

            Assert.Equal(baseDocument.Annotations["key1"], actualDocument.Annotations["key1"]);

            baseDocument.Annotations["key1"] = "value2";

            Assert.NotEqual(baseDocument.Annotations["key1"], actualDocument.Annotations["key1"]);
        }

        [Fact]
        public void SerializeExamplesDoesNotThrowNullReferenceException()
        {
            OpenApiDocument doc = new OpenApiDocument
            {
                Paths = new OpenApiPaths
                {
                    ["test"] = new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            [OperationType.Post] = new OpenApiOperation
                            {
                                RequestBody = new OpenApiRequestBody()
                                {
                                    Content =
                                    {
                                        ["application/json"] = new OpenApiMediaType()
                                        {
                                            Examples = null,
                                        },
                                    }
                                }
                            },
                        }
                    },
                }
            };

            OpenApiJsonWriter apiWriter = new OpenApiJsonWriter(new StringWriter());
            doc.Invoking(d => d.SerializeAsV3(apiWriter)).Should().NotThrow();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeDocumentWithWebhooksAsV3JsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            DocumentWithWebhooks.SerializeAsV31(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeDocumentWithWebhooksAsV3YamlWorks()
        {
            // Arrange
            var expected = @"openapi: '3.1.1'
info:
  title: Webhook Example
  version: 1.0.0
paths: { }
components:
  schemas:
    Pet:
      required:
        - id
        - name
      properties:
        id:
          type: integer
          format: int64
        name:
          type: string
        tag:
          type: string
webhooks:
  newPet:
    post:
      requestBody:
        description: Information about a new pet in the system
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/Pet'
      responses:
        '200':
          description: Return a 200 status to indicate that the data was received successfully";

            // Act
            var actual = DocumentWithWebhooks.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SerializeDocumentWithRootJsonSchemaDialectPropertyWorks()
        {
            // Arrange
            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "JsonSchemaDialectTest",
                    Version = "1.0.0"
                },
                JsonSchemaDialect = "http://json-schema.org/draft-07/schema#"
            };

            var expected = @"openapi: '3.1.1'
jsonSchemaDialect: http://json-schema.org/draft-07/schema#
info:
  title: JsonSchemaDialectTest
  version: 1.0.0
paths: { }";

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeV31DocumentWithRefsInWebhooksWorks()
        {
            var expected = @"description: Returns all pets from the system that the user has access to
operationId: findPets
responses:
  '200':
    description: pet response
    content:
      application/json:
        schema:
          type: array
          items:
            type: object";

            var doc = OpenApiDocument.Load("Models/Samples/docWithReusableWebhooks.yaml").Document;

            var stringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(stringWriter, new OpenApiWriterSettings { InlineLocalReferences = true });
            var webhooks = doc.Webhooks["pets"].Operations;

            webhooks[OperationType.Get].SerializeAsV31(writer);
            var actual = stringWriter.ToString();
            actual.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeDocWithDollarIdInDollarRefSucceeds()
        {
            var expected = @"openapi: '3.1.1'
info:
  title: Simple API
  version: 1.0.0
paths:
  /box:
    get:
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                $ref: https://foo.bar/Box
  /circle:
    get:
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                $ref: https://foo.bar/Circle
components:
  schemas:
    Box:
      $id: https://foo.bar/Box
      type: object
      properties:
        width:
          type: number
        height:
          type: number
    Circle:
      $id: https://foo.bar/Circle
      type: object
      properties:
        radius:
          type: number
";
            var doc = OpenApiDocument.Load("Models/Samples/docWithDollarId.yaml").Document;

            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);
            actual.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeDocumentTagsWithMultipleExtensionsWorks()
        {
            var expected = @"{
  ""openapi"": ""3.0.4"",
  ""info"": {
    ""title"": ""Test"",
    ""version"": ""1.0.0""
  },
  ""paths"": { },
  ""tags"": [
    {
      ""name"": ""tag1"",
      ""x-tag1"": ""tag1""
    },
    {
      ""name"": ""tag2"",
      ""x-tag2"": ""tag2""
    }
  ]
}";
            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Test",
                    Version = "1.0.0"
                },
                Paths = new OpenApiPaths(),
                Tags = new List<OpenApiTag>
                {
                    new OpenApiTag
                    {
                        Name = "tag1",
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["x-tag1"] = new OpenApiAny("tag1")
                        }
                    },
                    new OpenApiTag
                    {
                        Name = "tag2",
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["x-tag2"] = new OpenApiAny("tag2")
                        }
                    }
                }
            };

            var actual = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            actual.MakeLineBreaksEnvironmentNeutral().Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
