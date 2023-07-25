// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualBasic;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Microsoft.OpenApi.Readers.Extensions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiDocumentTests
    {
        public static OpenApiComponents TopLevelReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder().Ref("#/definitions/schema2"),
                ["schema2"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Properties(("property1", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                                .Build()
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Properties(("property1", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                                .Ref("schema1"),
                ["schema2"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Properties(("property1", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
            }

        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder().Ref("schema1")
            }
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelReferencingComponents = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Components = TopLevelReferencingComponents
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Components = TopLevelSelfReferencingComponentsWithOtherProperties
        };

        public static OpenApiDocument SimpleDocumentWithTopLevelSelfReferencingComponents = new OpenApiDocument()
        {
            Info = new OpenApiInfo()
            {
                Version = "1.0.0"
            },
            Components = TopLevelSelfReferencingComponents
        };

        public static OpenApiComponents AdvancedComponentsWithReference = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["pet"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Required("id", "name")
                            .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()),
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                            .Ref("pet").Build(),
                ["newPet"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("name")
                                .Properties(
                                    ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()),
                                    ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()),
                                    ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                                .Ref("newPet").Build(),
                ["errorModel"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("code", "message")
                                .Properties(
                                    ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build()),
                                    ("message", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                                .Ref("errorModel").Build()
            }
        };

        public static JsonSchema PetSchemaWithReference = AdvancedComponentsWithReference.Schemas["pet"];

        public static JsonSchema NewPetSchemaWithReference = AdvancedComponentsWithReference.Schemas["newPet"];

        public static JsonSchema ErrorModelSchemaWithReference =
            AdvancedComponentsWithReference.Schemas["errorModel"];

        public static OpenApiDocument AdvancedDocumentWithReference = new OpenApiDocument
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
                                    .Items(new JsonSchemaBuilder().Type(SchemaValueType.String).Build()).Build()
                                },
                                new OpenApiParameter
                                {
                                    Name = "limit",
                                    In = ParameterLocation.Query,
                                    Description = "maximum number of results to return",
                                    Required = false,
                                    Schema = new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Integer)
                                    .Format("int32").Build()
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
                                                            .Items(PetSchemaWithReference).Build()
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new JsonSchemaBuilder()
                                                            .Type(SchemaValueType.Array)
                                                            .Items(PetSchemaWithReference).Build()
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
                                    Schema = new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.Integer)
                                                    .Format("int64")
                                                    .Build()
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
                                    Schema = new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.Integer)
                                                    .Format("int64")
                                                    .Build()
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
            Components = AdvancedComponentsWithReference
        };

        public static OpenApiComponents AdvancedComponents = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["pet"] = new JsonSchemaBuilder()
                            .Type(SchemaValueType.Object)
                            .Required("id", "name")
                            .Properties(("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()),
                                ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()),
                                ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String).Build())),
                ["newPet"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("name")
                                .Properties(
                                    ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()),
                                    ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()),
                                    ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String).Build())),
                ["errorModel"] = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("code", "message")
                                .Properties(
                                    ("code", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build()),
                                    ("message", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
            }
        };

        public static JsonSchema PetSchema = AdvancedComponents.Schemas["pet"];

        public static JsonSchema NewPetSchema = AdvancedComponents.Schemas["newPet"];

        public static JsonSchema ErrorModelSchema = AdvancedComponents.Schemas["errorModel"];

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
                                    Schema = new JsonSchemaBuilder()
                                                .Type(SchemaValueType.Array)
                                                .Items(new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.String)
                                                    .Build())
                                                .Build()
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
                                                .Build()
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
                                                .Items(PetSchema)
                                                .Build()
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new JsonSchemaBuilder()
                                                .Type(SchemaValueType.Array)
                                                .Items(PetSchema)
                                                .Build()
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
                                    Schema = new JsonSchemaBuilder()
                                                .Type(SchemaValueType.Integer)
                                                .Format("int64")
                                                .Build()
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
                                    Schema = new JsonSchemaBuilder()
                                                .Type(SchemaValueType.Integer)
                                                .Format("int64")
                                                .Build()
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
            Components = AdvancedComponents
        };

        public static OpenApiDocument DocumentWithWebhooks = new OpenApiDocument()
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
                                        Schema = new JsonSchemaBuilder()
                                                       .Ref("#/components/schemas/Pet").Build()
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
                Schemas = new Dictionary<string, JsonSchema>
                {
                    ["Pet"] = new JsonSchemaBuilder()
                                .Required("id", "name")
                                .Properties(
                                    ("id", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build()),
                                    ("name", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()),
                                    ("tag", new JsonSchemaBuilder().Type(SchemaValueType.String).Build()))
                                .Build()
                }
            }
        };

        public static OpenApiDocument DuplicateExtensions = new OpenApiDocument
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
                                    Schema = new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Integer),                                  
                                        //.Extensions(new Dictionary<string, IOpenApiExtension>
                                        //{
                                        //    ["my-extension"] = new OpenApiAny(4),
                                        //})
                                        //.Build(),
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
                                    Schema = new JsonSchemaBuilder()
                                    .Type(SchemaValueType.Integer),
                                    //.Extensions(new Dictionary<string, IOpenApiExtension>
                                    //    {
                                    //        ["my-extension"] = new OpenApiAny(4),
                                    //    })
                                    //.Build(),
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
                                            Schema = new JsonSchemaBuilder()
                                                        .Type(SchemaValueType.Array)
                                                        .Items(PetSchema)
                                                        .Build()
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiDocumentTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SerializeAdvancedDocumentAsV3JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeAdvancedDocumentWithReferenceAsV3JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeAdvancedDocumentAsV2JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeDuplicateExtensionsAsV3JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeDuplicateExtensionsAsV2JsonWorks(bool produceTerseOutput)
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
        public async Task SerializeAdvancedDocumentWithReferenceAsV2JsonWorks(bool produceTerseOutput)
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
                                                Schema = new JsonSchemaBuilder().Ref("test")
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var reference = document.Paths["/"].Operations[OperationType.Get].Responses["200"].Content["application/json"].Schema.GetRef();           
            
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
                                        Schema = new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
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
            var expected = @"openapi: 3.0.1
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
                                        Schema = new JsonSchemaBuilder()
                                                    .Type(SchemaValueType.Object)
                                                    .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build())
                                                    .Build()
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
                                                Schema = new JsonSchemaBuilder()
                                                            .Type(SchemaValueType.String)
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void SerializeDocumentWithWebhooksAsV3JsonWorks(bool produceTerseOutput)
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
            var expected = @"openapi: '3.1.0'
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

            var expected = @"openapi: '3.1.0'
jsonSchemaDialect: http://json-schema.org/draft-07/schema#
info:
  title: JsonSchemaDialectTest
  version: 1.0.0
paths: { }";

            // Act
            var actual = doc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
