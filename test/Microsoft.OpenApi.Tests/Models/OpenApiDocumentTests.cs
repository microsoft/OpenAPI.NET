// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using VerifyXunit;
using Xunit;

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
                ["schema2"] = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
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
                ["schema1"] = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String,
                            Annotations = new Dictionary<string, object> { { "key1", "value" } }
                        }
                    },
                    Annotations = new Dictionary<string, object> { { "key1", "value" } },
                },
                ["schema2"] = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
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
                ["schema1"] = new OpenApiSchema()
                {
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
            Schemas = new Dictionary<string, IOpenApiSchema>
            {
                ["pet"] = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    Required = new HashSet<string>
                    {
                        "id",
                        "name"
                    },
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
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

        public static OpenApiSchema PetSchemaWithReference = AdvancedComponentsWithReference.Schemas["pet"] as OpenApiSchema;

        public static OpenApiSchema NewPetSchemaWithReference = AdvancedComponentsWithReference.Schemas["newPet"] as OpenApiSchema;

        public static OpenApiSchema ErrorModelSchemaWithReference =
            AdvancedComponentsWithReference.Schemas["errorModel"] as OpenApiSchema;

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
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchemaWithReference
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema()
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
            Schemas = new Dictionary<string, IOpenApiSchema>
            {
                ["pet"] = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    Required = new HashSet<string>
                    {
                        "id",
                        "name"
                    },
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
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
                    Properties = new Dictionary<string, IOpenApiSchema>
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

        public static readonly OpenApiSchema PetSchema = AdvancedComponents.Schemas["pet"] as OpenApiSchema;

        public static readonly OpenApiSchema NewPetSchema = AdvancedComponents.Schemas["newPet"] as OpenApiSchema;

        public static readonly OpenApiSchema ErrorModelSchema = AdvancedComponents.Schemas["errorModel"] as OpenApiSchema;

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
                            Parameters = new List<IOpenApiParameter>
                            {
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
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema()
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
                            Parameters = new List<IOpenApiParameter>
                            {
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
                            Parameters = new List<IOpenApiParameter>
                            {
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
            Webhooks = new Dictionary<string, IOpenApiPathItem>
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
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Pet"] = new OpenApiSchema()
                    {
                        Required = new HashSet<string>
                        {
                           "id", "name"
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>
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
                            Parameters = new List<IOpenApiParameter>
                            {
                                new OpenApiParameter
                                {
                                    Name = "operand1",
                                    In = ParameterLocation.Path,
                                    Description = "The first operand",
                                    Required = true,
                                    Schema = new OpenApiSchema()
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
                                    Schema = new OpenApiSchema()
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
                                            Schema = new OpenApiSchema()
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
                ["/pets"] = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Description = "Returns all pets from the system that the user has access to",
                            OperationId = "findPets",
                            Parameters = new List<IOpenApiParameter>
                            {
                                new OpenApiParameter()
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
                                new OpenApiParameter()
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
                            },
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Description = "pet response",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new()
                                        {
                                            Schema = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = PetSchema
                                            }
                                        }
                                    }
                                },
                                ["4XX"] = new OpenApiResponse()
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
                                ["5XX"] = new OpenApiResponse()
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
                            RequestBody = new OpenApiRequestBody()
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
                                ["200"] = new OpenApiResponse()
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
                                ["4XX"] = new OpenApiResponse()
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
                                ["5XX"] = new OpenApiResponse()
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
                ["/pets/{id}"] = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Description =
                                "Returns a user based on a single ID, if the user does not have access to the pet",
                            OperationId = "findPetById",
                            Parameters = new List<IOpenApiParameter>
                            {
                                new OpenApiParameter()
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
                            },
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
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
                                ["4XX"] = new OpenApiResponse()
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
                                ["5XX"] = new OpenApiResponse()
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
                            Parameters = new List<IOpenApiParameter>
                            {
                                new OpenApiParameter()
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
                            },
                            Responses = new()
                            {
                                ["204"] = new OpenApiResponse()
                                {
                                    Description = "pet deleted"
                                },
                                ["4XX"] = new OpenApiResponse()
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
                                ["5XX"] = new OpenApiResponse()
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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeSimpleDocumentWithTopLevelReferencingComponentsAsYamlV2Works()
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
            var actual = await SimpleDocumentWithTopLevelReferencingComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeSimpleDocumentWithTopLevelSelfReferencingComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"swagger: '2.0'
info:
  version: 1.0.0
paths: { }
definitions:
  schema1: { }";

            // Act
            var actual = await SimpleDocumentWithTopLevelSelfReferencingComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeSimpleDocumentWithTopLevelSelfReferencingWithOtherPropertiesComponentsAsYamlV3Works()
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
            var actual = await SimpleDocumentWithTopLevelSelfReferencingComponentsWithOtherProperties.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeDocumentWithReferenceButNoComponents()
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
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            document.Paths["/"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema = new OpenApiSchemaReference("test", document);

            // Act
            var actual = await document.SerializeAsync(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task SerializeRelativePathAsV2JsonWorks()
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
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeRelativePathWithHostAsV2JsonWorks()
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
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeRelativeRootPathWithHostAsV2JsonWorks()
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
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task TestHashCodesForSimilarOpenApiDocuments()
        {
            // Arrange
            var sampleFolderPath = "Models/Samples/";

            var doc1 = await ParseInputFileAsync(Path.Combine(sampleFolderPath, "sampleDocument.yaml"));
            var doc2 = await ParseInputFileAsync(Path.Combine(sampleFolderPath, "sampleDocument.yaml"));
            var doc3 = await ParseInputFileAsync(Path.Combine(sampleFolderPath, "sampleDocumentWithWhiteSpaces.yaml"));

            // Act && Assert
            /*
                Test whether reading in two similar documents yield the same hash code,
                And reading in similar documents(one has a whitespace) yields the same hash code as the result is terse
            */
            var doc1HashCode = await doc1.GetHashCodeAsync();
            var doc2HashCode = await doc2.GetHashCodeAsync();
            var doc3HashCode = await doc3.GetHashCodeAsync();
            Assert.NotNull(doc1HashCode);
            Assert.NotNull(doc2HashCode);
            Assert.NotNull(doc3HashCode);
            Assert.NotEmpty(doc1HashCode);
            Assert.NotEmpty(doc2HashCode);
            Assert.NotEmpty(doc3HashCode);
            Assert.Equal(doc1HashCode, doc2HashCode);
            Assert.Equal(doc1HashCode, doc3HashCode);
        }

        private static async Task<OpenApiDocument> ParseInputFileAsync(string filePath)
        {
            var openApiDoc = (await OpenApiDocument.LoadAsync(filePath)).Document;
            return openApiDoc;
        }

        [Fact]
        public async Task SerializeV2DocumentWithNonArraySchemaTypeDoesNotWriteOutCollectionFormat()
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
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        In = ParameterLocation.Query,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.String
                                        }
                                    }
                                ],
                                Responses = new OpenApiResponses()
                            }
                        }
                    }
                }
            };

            // Act
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeV2DocumentWithStyleAsNullDoesNotWriteOutStyleValue()
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
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        Name = "id",
                                        In = ParameterLocation.Query,
                                        Schema = new OpenApiSchema()
                                        {
                                            Type = JsonSchemaType.Object,
                                            AdditionalProperties = new OpenApiSchema()
                                            {
                                                Type = JsonSchemaType.Integer
                                            }
                                        }
                                    }
                                ],
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "foo",
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["text/plain"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema()
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
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
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
            doc.SerializeAsV3(apiWriter);
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
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            await Verifier.Verify(actual).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeDocumentWithWebhooksAsV3YamlWorks()
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
            var actual = await DocumentWithWebhooks.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeDocumentWithRootJsonSchemaDialectPropertyWorks()
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
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeV31DocumentWithRefsInWebhooksWorks()
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

            var doc = (await OpenApiDocument.LoadAsync("Models/Samples/docWithReusableWebhooks.yaml")).Document;
          
            var stringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(stringWriter, new OpenApiWriterSettings { InlineLocalReferences = true });
            var webhooks = doc.Webhooks["pets"].Operations;

            webhooks[OperationType.Get].SerializeAsV31(writer);
            var actual = stringWriter.ToString();
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeDocWithDollarIdInDollarRefSucceeds()
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
            var doc = (await OpenApiDocument.LoadAsync("Models/Samples/docWithDollarId.yaml")).Document;
            var actual = await doc.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeDocumentTagsWithMultipleExtensionsWorks()
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
                Tags = new HashSet<OpenApiTag>
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

            var actual = await doc.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }
        [Fact]
        public void DeduplicatesTags()
        {
            var document = new OpenApiDocument
            {
                Tags = new HashSet<OpenApiTag>
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
                    },
                    new OpenApiTag
                    {
                        Name = "tag1",
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["x-tag1"] = new OpenApiAny("tag1")
                        }
                    }
                }
            };
            Assert.Equal(2, document.Tags.Count);
            Assert.Contains(document.Tags, t => t.Name == "tag1");
            Assert.Contains(document.Tags, t => t.Name == "tag2");
        }
    }
}
