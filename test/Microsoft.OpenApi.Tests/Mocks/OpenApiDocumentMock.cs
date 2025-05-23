using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;

namespace Microsoft.OpenApi.Tests.Mocks
{
    public static class OpenApiDocumentMock
    {
        public static OpenApiDocument CreateCompleteOpenApiDocument()
        {
            return new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "Sample API",
                    Version = "1.0.0"
                },
                Servers = new List<OpenApiServer>
                {
                    new() 
                    {
                        Url = "https://api.example.com",
                        Description = "Production server"
                    }
                },
                Webhooks = new Dictionary<string, IOpenApiPathItem>
                {
                    ["pets"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "pet response",
                                        Content = new Dictionary<string, OpenApiMediaType>()
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Schema = new OpenApiSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Servers =
                        [
                            new()
                            {
                                Url = "https://api.example.com",
                                Description = "Production server"
                            }
                        ],
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "limit",
                                In = ParameterLocation.Query,
                                Description = "How many items to return at one time (max 100)",
                                Required = false,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.Integer,
                                    Format = "int32"
                                },
                                Examples = new Dictionary<string, IOpenApiExample>
                                {
                                    ["cat"] = new OpenApiExample
                                    {
                                        Summary = "An example cat",
                                        Value = JsonValue.Create("Fluffy")
                                    }
                                },
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("Pet")
                                    }
                                }
                            }
                        ],
                        Operations = new Dictionary<HttpMethod, OpenApiOperation>
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Summary = "List all pets",
                                Callbacks = new Dictionary<string, IOpenApiCallback>
                                {
                                    ["onData"] = new OpenApiCallback
                                    {
                                        PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
                                        {
                                            [RuntimeExpression.Build("{$request.body#/callbackUrl}")] = new OpenApiPathItem
                                            {
                                                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                                                {
                                                    [HttpMethod.Post] = new OpenApiOperation
                                                    {
                                                        Responses = new OpenApiResponses
                                                        {
                                                            ["200"] = new OpenApiResponse { Description = "ok" }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                Parameters =
                                [
                                    new OpenApiParameter
                                    {
                                        Name = "limit",
                                        In = ParameterLocation.Query,
                                        Description = "How many items to return at one time (max 100)",
                                        Required = false,
                                        Schema = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        }
                                    }
                                ],
                                RequestBody =  new OpenApiRequestBody
                                {
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchemaReference("Pet")
                                        },
                                        ["application/xml"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.String,
                                                Xml = new OpenApiXml
                                                {
                                                    Name = "name1",
                                                    Namespace = new Uri("http://example.com/schema/namespaceSample"),
                                                }
                                            },
                                        }
                                    }
                                },                                
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse
                                    {
                                        Description = "A list of pets.",
                                        Headers = new Dictionary<string, IOpenApiHeader>
                                        {
                                            ["x-rate-limit"] = new OpenApiHeader
                                            {
                                                Description = "The number of allowed requests in the current period",
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = JsonSchemaType.Integer
                                                },
                                                Examples = new Dictionary<string, IOpenApiExample>
                                                {
                                                    ["cat"] = new OpenApiExample
                                                    {
                                                        Summary = "An example cat",
                                                        Value = JsonValue.Create("Fluffy")
                                                    }
                                                },
                                                Content = new Dictionary<string, OpenApiMediaType>
                                                {
                                                    ["application/json"] = new OpenApiMediaType
                                                    {
                                                        Schema = new OpenApiSchema
                                                        {
                                                            Type = JsonSchemaType.Array,
                                                            Items = new OpenApiSchemaReference("Pet")
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        Links = new Dictionary<string, IOpenApiLink>
                                        {
                                            ["UserRepositories"] = new OpenApiLink
                                            {
                                                OperationId = "getRepositoriesByUsername",
                                                Parameters = new Dictionary<string, RuntimeExpressionAnyWrapper>
                                                {
                                                    ["username"] = new RuntimeExpressionAnyWrapper
                                                    {
                                                        Expression = RuntimeExpression.Build("$request.path.id")
                                                    }
                                                }
                                            }
                                        },
                                        Content = new Dictionary<string, OpenApiMediaType>
                                        {
                                            ["application/json"] = new OpenApiMediaType
                                            {
                                                Encoding = new Dictionary<string, OpenApiEncoding>
                                                {
                                                    ["x-rate-limit"] = new OpenApiEncoding
                                                    {
                                                        Headers = new Dictionary<string, IOpenApiHeader>
                                                        {
                                                            ["x-rate-limit"] = new OpenApiHeader
                                                            {
                                                                Description = "The number of allowed requests in the current period",
                                                                Schema = new OpenApiSchema
                                                                {
                                                                    Type = JsonSchemaType.Integer
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                Schema = new OpenApiSchema
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Items = new OpenApiSchemaReference("Pet")
                                                },
                                                Examples = new Dictionary<string, IOpenApiExample>
                                                {
                                                    ["cat"] = new OpenApiExample
                                                    {
                                                        Summary = "An example cat",
                                                        Value = JsonValue.Create("Fluffy")
                                                    }
                                                }
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
                                ],
                                Tags = new HashSet<OpenApiTagReference>
                                {
                                    new OpenApiTagReference("tagId1", new OpenApiDocument{ Tags = new HashSet<OpenApiTag>() { new OpenApiTag{Name = "tagId1"}} })
                                },
                                                }
                        }
                    }
                },
                Components = new OpenApiComponents
                {
                    Schemas = new Dictionary<string, IOpenApiSchema>
                    {
                        ["pet"] = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = new Dictionary<string, IOpenApiSchema>
                            {
                                ["id"] = new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" },
                                ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                                ["tag"] = new OpenApiSchema { Type = JsonSchemaType.String }
                            },
                            Required = new HashSet<string> { "id", "name" }
                        }
                    },
                    Parameters = new Dictionary<string, IOpenApiParameter>
                    {
                        ["limit"] = new OpenApiParameter
                        {
                            Name = "limit",
                            In = ParameterLocation.Query,
                            Description = "How many items to return at one time (max 100)",
                            Required = false,
                            Schema = new OpenApiSchema
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
                            Description = "A list of pets.",                            
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Array,
                                        Items = new OpenApiSchemaReference("Pet")
                                    }
                                }
                            }
                        }
                    },
                    RequestBodies = new Dictionary<string, IOpenApiRequestBody>
                    {
                        ["pet"] = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Pet")
                                }
                            }
                        }
                    },
                    Links = new Dictionary<string, IOpenApiLink>
                    {
                        ["UserRepositories"] = new OpenApiLink
                        {
                            OperationId = "getRepositoriesByUsername",
                            Parameters = new Dictionary<string, RuntimeExpressionAnyWrapper>
                            {
                                ["username"] = new RuntimeExpressionAnyWrapper
                                {
                                    Expression = RuntimeExpression.Build("$request.path.id")
                                }
                            }
                        }
                    },
                    Headers = new Dictionary<string, IOpenApiHeader>
                    {
                        ["x-rate-limit"] = new OpenApiHeader
                        {
                            Description = "The number of allowed requests in the current period",
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Integer
                            }
                        }
                    },
                    Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["cat"] = new OpenApiExample
                        {
                            Summary = "An example cat",
                            Value = JsonValue.Create("Fluffy")
                        }
                    },
                    SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                    {
                        ["api_key"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.ApiKey,
                            Name = "api_key",
                            In = ParameterLocation.Header
                        }
                    },
                    Callbacks = new Dictionary<string, IOpenApiCallback>
                    {
                        ["onData"] = new OpenApiCallback
                        {
                            PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>
                            {
                                [RuntimeExpression.Build("{$request.body#/callbackUrl}")] = new OpenApiPathItem
                                {
                                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                                    {
                                        [HttpMethod.Post] = new OpenApiOperation
                                        {
                                            Responses = new OpenApiResponses
                                            {
                                                ["200"] = new OpenApiResponse { Description = "ok" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    PathItems = new Dictionary<string, IOpenApiPathItem>
                    {
                        ["/pets"] = new OpenApiPathItem
                        {
                            Operations = new Dictionary<HttpMethod, OpenApiOperation>
                            {
                                [HttpMethod.Get] = new OpenApiOperation
                                {
                                    Summary = "List all pets",
                                    Responses = new OpenApiResponses
                                    {
                                        ["200"] = new OpenApiResponse
                                        {
                                            Description = "A list of pets.",
                                            Content = new Dictionary<string, OpenApiMediaType>
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Array,
                                                        Items = new OpenApiSchemaReference("Pet")
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
                Security =
                [
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference("api_key"),
                            new List<string>()
                        }
                    }
                ],
                Tags = new HashSet<OpenApiTag>
                {
                    new() {
                        Name = "pets",
                        Description = "Operations related to pets"
                    }
                },
                ExternalDocs = new OpenApiExternalDocs
                {
                    Description = "Find out more",
                    Url = new Uri("https://example.com/docs")
                }
            };
        }
    }

}
