// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiDocumentTests
    {
        public static OpenApiComponents TopLevelReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema2"
                    }
                },
                ["schema2"] = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = "string"
                        }
                    },
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
                ["schema2"] = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchema
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                }
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
                        Id = "pet",
                        Type = ReferenceType.Schema
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
                        Id = "newPet",
                        Type = ReferenceType.Schema
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
                                                Items = PetSchemaWithReference
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
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
                    }
                },
            }
        };

        public static OpenApiSchema PetSchema = AdvancedComponents.Schemas["pet"];

        public static OpenApiSchema NewPetSchema = AdvancedComponents.Schemas["newPet"];

        public static OpenApiSchema ErrorModelSchema = AdvancedComponents.Schemas["errorModel"];

        public static OpenApiDocument AdvancedDocument = new OpenApiDocument
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
                                                Items = PetSchema
                                            }
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
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

        private readonly ITestOutputHelper _output;

        public OpenApiDocumentTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeAdvancedDocumentAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""Swagger Petstore (Simple)"",
    ""description"": ""A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification"",
    ""termsOfService"": ""http://helloreverb.com/terms/"",
    ""contact"": {
      ""name"": ""Swagger API team"",
      ""url"": ""http://swagger.io"",
      ""email"": ""foo@example.com""
    },
    ""license"": {
      ""name"": ""MIT"",
      ""url"": ""http://opensource.org/licenses/MIT""
    },
    ""version"": ""1.0.0""
  },
  ""servers"": [
    {
      ""url"": ""http://petstore.swagger.io/api""
    }
  ],
  ""paths"": {
    ""/pets"": {
      ""get"": {
        ""description"": ""Returns all pets from the system that the user has access to"",
        ""operationId"": ""findPets"",
        ""parameters"": [
          {
            ""name"": ""tags"",
            ""in"": ""query"",
            ""description"": ""tags to filter by"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            }
          },
          {
            ""name"": ""limit"",
            ""in"": ""query"",
            ""description"": ""maximum number of results to return"",
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int32""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""required"": [
                      ""id"",
                      ""name""
                    ],
                    ""type"": ""object"",
                    ""properties"": {
                      ""id"": {
                        ""type"": ""integer"",
                        ""format"": ""int64""
                      },
                      ""name"": {
                        ""type"": ""string""
                      },
                      ""tag"": {
                        ""type"": ""string""
                      }
                    }
                  }
                }
              },
              ""application/xml"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""required"": [
                      ""id"",
                      ""name""
                    ],
                    ""type"": ""object"",
                    ""properties"": {
                      ""id"": {
                        ""type"": ""integer"",
                        ""format"": ""int64""
                      },
                      ""name"": {
                        ""type"": ""string""
                      },
                      ""tag"": {
                        ""type"": ""string""
                      }
                    }
                  }
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          }
        }
      },
      ""post"": {
        ""description"": ""Creates a new pet in the store.  Duplicates are allowed"",
        ""operationId"": ""addPet"",
        ""requestBody"": {
          ""description"": ""Pet to add to the store"",
          ""content"": {
            ""application/json"": {
              ""schema"": {
                ""required"": [
                  ""name""
                ],
                ""type"": ""object"",
                ""properties"": {
                  ""id"": {
                    ""type"": ""integer"",
                    ""format"": ""int64""
                  },
                  ""name"": {
                    ""type"": ""string""
                  },
                  ""tag"": {
                    ""type"": ""string""
                  }
                }
              }
            }
          },
          ""required"": true
        },
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""required"": [
                    ""id"",
                    ""name""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""id"": {
                      ""type"": ""integer"",
                      ""format"": ""int64""
                    },
                    ""name"": {
                      ""type"": ""string""
                    },
                    ""tag"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          }
        }
      }
    },
    ""/pets/{id}"": {
      ""get"": {
        ""description"": ""Returns a user based on a single ID, if the user does not have access to the pet"",
        ""operationId"": ""findPetById"",
        ""parameters"": [
          {
            ""name"": ""id"",
            ""in"": ""path"",
            ""description"": ""ID of pet to fetch"",
            ""required"": true,
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int64""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""required"": [
                    ""id"",
                    ""name""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""id"": {
                      ""type"": ""integer"",
                      ""format"": ""int64""
                    },
                    ""name"": {
                      ""type"": ""string""
                    },
                    ""tag"": {
                      ""type"": ""string""
                    }
                  }
                }
              },
              ""application/xml"": {
                ""schema"": {
                  ""required"": [
                    ""id"",
                    ""name""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""id"": {
                      ""type"": ""integer"",
                      ""format"": ""int64""
                    },
                    ""name"": {
                      ""type"": ""string""
                    },
                    ""tag"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          }
        }
      },
      ""delete"": {
        ""description"": ""deletes a single pet based on the ID supplied"",
        ""operationId"": ""deletePet"",
        ""parameters"": [
          {
            ""name"": ""id"",
            ""in"": ""path"",
            ""description"": ""ID of pet to delete"",
            ""required"": true,
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int64""
            }
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""pet deleted""
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""required"": [
                    ""code"",
                    ""message""
                  ],
                  ""type"": ""object"",
                  ""properties"": {
                    ""code"": {
                      ""type"": ""integer"",
                      ""format"": ""int32""
                    },
                    ""message"": {
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""pet"": {
        ""required"": [
          ""id"",
          ""name""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""id"": {
            ""type"": ""integer"",
            ""format"": ""int64""
          },
          ""name"": {
            ""type"": ""string""
          },
          ""tag"": {
            ""type"": ""string""
          }
        }
      },
      ""newPet"": {
        ""required"": [
          ""name""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""id"": {
            ""type"": ""integer"",
            ""format"": ""int64""
          },
          ""name"": {
            ""type"": ""string""
          },
          ""tag"": {
            ""type"": ""string""
          }
        }
      },
      ""errorModel"": {
        ""required"": [
          ""code"",
          ""message""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""code"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          },
          ""message"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}";

            // Act
            AdvancedDocument.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedDocumentWithReferenceAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""Swagger Petstore (Simple)"",
    ""description"": ""A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification"",
    ""termsOfService"": ""http://helloreverb.com/terms/"",
    ""contact"": {
      ""name"": ""Swagger API team"",
      ""url"": ""http://swagger.io"",
      ""email"": ""foo@example.com""
    },
    ""license"": {
      ""name"": ""MIT"",
      ""url"": ""http://opensource.org/licenses/MIT""
    },
    ""version"": ""1.0.0""
  },
  ""servers"": [
    {
      ""url"": ""http://petstore.swagger.io/api""
    }
  ],
  ""paths"": {
    ""/pets"": {
      ""get"": {
        ""description"": ""Returns all pets from the system that the user has access to"",
        ""operationId"": ""findPets"",
        ""parameters"": [
          {
            ""name"": ""tags"",
            ""in"": ""query"",
            ""description"": ""tags to filter by"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            }
          },
          {
            ""name"": ""limit"",
            ""in"": ""query"",
            ""description"": ""maximum number of results to return"",
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int32""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""$ref"": ""#/components/schemas/pet""
                  }
                }
              },
              ""application/xml"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""$ref"": ""#/components/schemas/pet""
                  }
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          }
        }
      },
      ""post"": {
        ""description"": ""Creates a new pet in the store.  Duplicates are allowed"",
        ""operationId"": ""addPet"",
        ""requestBody"": {
          ""description"": ""Pet to add to the store"",
          ""content"": {
            ""application/json"": {
              ""schema"": {
                ""$ref"": ""#/components/schemas/newPet""
              }
            }
          },
          ""required"": true
        },
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/pet""
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          }
        }
      }
    },
    ""/pets/{id}"": {
      ""get"": {
        ""description"": ""Returns a user based on a single ID, if the user does not have access to the pet"",
        ""operationId"": ""findPetById"",
        ""parameters"": [
          {
            ""name"": ""id"",
            ""in"": ""path"",
            ""description"": ""ID of pet to fetch"",
            ""required"": true,
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int64""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/pet""
                }
              },
              ""application/xml"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/pet""
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          }
        }
      },
      ""delete"": {
        ""description"": ""deletes a single pet based on the ID supplied"",
        ""operationId"": ""deletePet"",
        ""parameters"": [
          {
            ""name"": ""id"",
            ""in"": ""path"",
            ""description"": ""ID of pet to delete"",
            ""required"": true,
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int64""
            }
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""pet deleted""
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""content"": {
              ""text/html"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/errorModel""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""pet"": {
        ""required"": [
          ""id"",
          ""name""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""id"": {
            ""type"": ""integer"",
            ""format"": ""int64""
          },
          ""name"": {
            ""type"": ""string""
          },
          ""tag"": {
            ""type"": ""string""
          }
        }
      },
      ""newPet"": {
        ""required"": [
          ""name""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""id"": {
            ""type"": ""integer"",
            ""format"": ""int64""
          },
          ""name"": {
            ""type"": ""string""
          },
          ""tag"": {
            ""type"": ""string""
          }
        }
      },
      ""errorModel"": {
        ""required"": [
          ""code"",
          ""message""
        ],
        ""type"": ""object"",
        ""properties"": {
          ""code"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          },
          ""message"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}";

            // Act
            AdvancedDocumentWithReference.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedDocumentAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected = @"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": ""Swagger Petstore (Simple)"",
    ""description"": ""A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification"",
    ""termsOfService"": ""http://helloreverb.com/terms/"",
    ""contact"": {
      ""name"": ""Swagger API team"",
      ""url"": ""http://swagger.io"",
      ""email"": ""foo@example.com""
    },
    ""license"": {
      ""name"": ""MIT"",
      ""url"": ""http://opensource.org/licenses/MIT""
    },
    ""version"": ""1.0.0""
  },
  ""host"": ""petstore.swagger.io"",
  ""basePath"": ""/api"",
  ""schemes"": [
    ""http""
  ],
  ""paths"": {
    ""/pets"": {
      ""get"": {
        ""description"": ""Returns all pets from the system that the user has access to"",
        ""operationId"": ""findPets"",
        ""produces"": [
          ""application/json"",
          ""application/xml"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""query"",
            ""name"": ""tags"",
            ""description"": ""tags to filter by"",
            ""type"": ""array"",
            ""items"": {
              ""type"": ""string""
            }
          },
          {
            ""in"": ""query"",
            ""name"": ""limit"",
            ""description"": ""maximum number of results to return"",
            ""type"": ""integer"",
            ""format"": ""int32""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""required"": [
                  ""id"",
                  ""name""
                ],
                ""type"": ""object"",
                ""properties"": {
                  ""id"": {
                    ""format"": ""int64"",
                    ""type"": ""integer""
                  },
                  ""name"": {
                    ""type"": ""string""
                  },
                  ""tag"": {
                    ""type"": ""string""
                  }
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      },
      ""post"": {
        ""description"": ""Creates a new pet in the store.  Duplicates are allowed"",
        ""operationId"": ""addPet"",
        ""consumes"": [
          ""application/json""
        ],
        ""produces"": [
          ""application/json"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""body"",
            ""name"": ""body"",
            ""description"": ""Pet to add to the store"",
            ""required"": true,
            ""schema"": {
              ""required"": [
                ""name""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""id"": {
                  ""format"": ""int64"",
                  ""type"": ""integer""
                },
                ""name"": {
                  ""type"": ""string""
                },
                ""tag"": {
                  ""type"": ""string""
                }
              }
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""required"": [
                ""id"",
                ""name""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""id"": {
                  ""format"": ""int64"",
                  ""type"": ""integer""
                },
                ""name"": {
                  ""type"": ""string""
                },
                ""tag"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/pets/{id}"": {
      ""get"": {
        ""description"": ""Returns a user based on a single ID, if the user does not have access to the pet"",
        ""operationId"": ""findPetById"",
        ""produces"": [
          ""application/json"",
          ""application/xml"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""path"",
            ""name"": ""id"",
            ""description"": ""ID of pet to fetch"",
            ""required"": true,
            ""type"": ""integer"",
            ""format"": ""int64""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""required"": [
                ""id"",
                ""name""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""id"": {
                  ""format"": ""int64"",
                  ""type"": ""integer""
                },
                ""name"": {
                  ""type"": ""string""
                },
                ""tag"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      },
      ""delete"": {
        ""description"": ""deletes a single pet based on the ID supplied"",
        ""operationId"": ""deletePet"",
        ""produces"": [
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""path"",
            ""name"": ""id"",
            ""description"": ""ID of pet to delete"",
            ""required"": true,
            ""type"": ""integer"",
            ""format"": ""int64""
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""pet deleted""
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""required"": [
                ""code"",
                ""message""
              ],
              ""type"": ""object"",
              ""properties"": {
                ""code"": {
                  ""format"": ""int32"",
                  ""type"": ""integer""
                },
                ""message"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    }
  },
  ""definitions"": {
    ""pet"": {
      ""required"": [
        ""id"",
        ""name""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""format"": ""int64"",
          ""type"": ""integer""
        },
        ""name"": {
          ""type"": ""string""
        },
        ""tag"": {
          ""type"": ""string""
        }
      }
    },
    ""newPet"": {
      ""required"": [
        ""name""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""format"": ""int64"",
          ""type"": ""integer""
        },
        ""name"": {
          ""type"": ""string""
        },
        ""tag"": {
          ""type"": ""string""
        }
      }
    },
    ""errorModel"": {
      ""required"": [
        ""code"",
        ""message""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""code"": {
          ""format"": ""int32"",
          ""type"": ""integer""
        },
        ""message"": {
          ""type"": ""string""
        }
      }
    }
  }
}";

            // Act
            AdvancedDocument.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();
            
            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedDocumentWithReferenceAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": ""Swagger Petstore (Simple)"",
    ""description"": ""A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification"",
    ""termsOfService"": ""http://helloreverb.com/terms/"",
    ""contact"": {
      ""name"": ""Swagger API team"",
      ""url"": ""http://swagger.io"",
      ""email"": ""foo@example.com""
    },
    ""license"": {
      ""name"": ""MIT"",
      ""url"": ""http://opensource.org/licenses/MIT""
    },
    ""version"": ""1.0.0""
  },
  ""host"": ""petstore.swagger.io"",
  ""basePath"": ""/api"",
  ""schemes"": [
    ""http""
  ],
  ""paths"": {
    ""/pets"": {
      ""get"": {
        ""description"": ""Returns all pets from the system that the user has access to"",
        ""operationId"": ""findPets"",
        ""produces"": [
          ""application/json"",
          ""application/xml"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""query"",
            ""name"": ""tags"",
            ""description"": ""tags to filter by"",
            ""type"": ""array"",
            ""items"": {
              ""type"": ""string""
            }
          },
          {
            ""in"": ""query"",
            ""name"": ""limit"",
            ""description"": ""maximum number of results to return"",
            ""type"": ""integer"",
            ""format"": ""int32""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/pet""
              }
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          }
        }
      },
      ""post"": {
        ""description"": ""Creates a new pet in the store.  Duplicates are allowed"",
        ""operationId"": ""addPet"",
        ""consumes"": [
          ""application/json""
        ],
        ""produces"": [
          ""application/json"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""body"",
            ""name"": ""body"",
            ""description"": ""Pet to add to the store"",
            ""required"": true,
            ""schema"": {
              ""$ref"": ""#/definitions/newPet""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""$ref"": ""#/definitions/pet""
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          }
        }
      }
    },
    ""/pets/{id}"": {
      ""get"": {
        ""description"": ""Returns a user based on a single ID, if the user does not have access to the pet"",
        ""operationId"": ""findPetById"",
        ""produces"": [
          ""application/json"",
          ""application/xml"",
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""path"",
            ""name"": ""id"",
            ""description"": ""ID of pet to fetch"",
            ""required"": true,
            ""type"": ""integer"",
            ""format"": ""int64""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""pet response"",
            ""schema"": {
              ""$ref"": ""#/definitions/pet""
            }
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          }
        }
      },
      ""delete"": {
        ""description"": ""deletes a single pet based on the ID supplied"",
        ""operationId"": ""deletePet"",
        ""produces"": [
          ""text/html""
        ],
        ""parameters"": [
          {
            ""in"": ""path"",
            ""name"": ""id"",
            ""description"": ""ID of pet to delete"",
            ""required"": true,
            ""type"": ""integer"",
            ""format"": ""int64""
          }
        ],
        ""responses"": {
          ""204"": {
            ""description"": ""pet deleted""
          },
          ""4XX"": {
            ""description"": ""unexpected client error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          },
          ""5XX"": {
            ""description"": ""unexpected server error"",
            ""schema"": {
              ""$ref"": ""#/definitions/errorModel""
            }
          }
        }
      }
    }
  },
  ""definitions"": {
    ""pet"": {
      ""required"": [
        ""id"",
        ""name""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""format"": ""int64"",
          ""type"": ""integer""
        },
        ""name"": {
          ""type"": ""string""
        },
        ""tag"": {
          ""type"": ""string""
        }
      }
    },
    ""newPet"": {
      ""required"": [
        ""name""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""format"": ""int64"",
          ""type"": ""integer""
        },
        ""name"": {
          ""type"": ""string""
        },
        ""tag"": {
          ""type"": ""string""
        }
      }
    },
    ""errorModel"": {
      ""required"": [
        ""code"",
        ""message""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""code"": {
          ""format"": ""int32"",
          ""type"": ""integer""
        },
        ""message"": {
          ""type"": ""string""
        }
      }
    }
  }
}";

            // Act
            AdvancedDocumentWithReference.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();
            
            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
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
                                                Schema = new OpenApiSchema
                                                {
                                                    Reference = new OpenApiReference
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
    }
}