﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Tests.Services
{
    internal static class OpenApiComparerTestCases
    {
        public static IEnumerable<object[]> GetTestCasesForOpenApiComparerShouldSucceed()
        {
            // New and removed paths
            yield return new object[]
            {
                "New And Removed Paths",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/newPath", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1newPath",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    }
                }
            };

            // New and removed operations
            yield return new object[]
            {
                "New And Removed Operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Patch, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/patch",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/post",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    }
                }
            };

            // Empty target document paths
            yield return new object[]
            {
                "Empty target document",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument(),
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiPaths)
                    }
                }
            };

            // Empty source document
            yield return new object[]
            {
                "Empty source document",
                new OpenApiDocument(),
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/newPath", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiPaths)
                    }
                }
            };

            // Empty target operations
            yield return new object[]
            {
                "Empty target operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>()
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/post",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    }
                }
            };

            // Empty source operations
            yield return new object[]
            {
                "Empty source operations",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>()
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Patch, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/patch",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    }
                }
            };

            // Identical source and target
            yield return new object[]
            {
                "Identical source and target documents",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>()
            };

            // Differences in summary and description
            yield return new object[]
            {
                "Differences in summary and description",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "updated",
                                Description = "updated",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation()
                                    },
                                    {
                                        OperationType.Post, new OpenApiOperation()
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/summary",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string)
                    }
                }
            };

            // Differences in schema
            yield return new object[]
            {
                "Differences in schema",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation
                                        {
                                            Parameters = new List<OpenApiParameter>
                                            {
                                                new OpenApiParameter
                                                {
                                                    Name = "Test Parameter",
                                                    In = ParameterLocation.Path,
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "title1",
                                                        MultipleOf = 3,
                                                        Maximum = 42,
                                                        ExclusiveMinimum = true,
                                                        Minimum = 10,
                                                        Default = new OpenApiInteger(15),
                                                        Type = "integer",

                                                        Nullable = true,
                                                        ExternalDocs = new OpenApiExternalDocs
                                                        {
                                                            Url = new Uri("http://example.com/externalDocs")
                                                        },

                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.Schema,
                                                            Id = "schemaObject1"
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
                    Components = new OpenApiComponents
                    {
                        Schemas = new Dictionary<string, OpenApiSchema>
                        {
                            ["schemaObject1"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property7"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject2"
                                        }
                                    }
                                }
                            },
                            ["schemaObject2"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation
                                        {
                                            Parameters = new List<OpenApiParameter>
                                            {
                                                new OpenApiParameter
                                                {
                                                    Name = "Test Parameter",
                                                    In = ParameterLocation.Path,
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Title = "title1",
                                                        MultipleOf = 3,
                                                        Maximum = 42,
                                                        ExclusiveMinimum = true,
                                                        Minimum = 10,
                                                        Default = new OpenApiInteger(15),
                                                        Type = "integer",

                                                        Nullable = true,
                                                        ExternalDocs = new OpenApiExternalDocs
                                                        {
                                                            Url = new Uri("http://example.com/externalDocs")
                                                        },

                                                        Reference = new OpenApiReference
                                                        {
                                                            Type = ReferenceType.Schema,
                                                            Id = "schemaObject1"
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
                    Components = new OpenApiComponents
                    {
                        Schemas = new Dictionary<string, OpenApiSchema>
                        {
                            ["schemaObject1"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject2"
                                        }
                                    }
                                }
                            },
                            ["schemaObject2"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get/parameters/0/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get/parameters/0/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/parameters/0/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/parameters/0/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    }
                }
            };

            // Differences in request and response
            yield return new object[]
            {
                "Differences in request and response",
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation
                                        {
                                            RequestBody = new OpenApiRequestBody
                                            {
                                                Description = "description",
                                                Required = true,
                                                Content =
                                                {
                                                    ["application/xml"] = new OpenApiMediaType
                                                    {
                                                        Schema = new OpenApiSchema
                                                        {
                                                            Reference = new OpenApiReference
                                                            {
                                                                Id = "schemaObject1",
                                                                Type = ReferenceType.Schema
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            Responses = new OpenApiResponses
                                            {
                                                {
                                                    "200",
                                                    new OpenApiResponse
                                                    {
                                                        Description = "An updated complex object array response",
                                                        Content =
                                                        {
                                                            ["application/json"] = new OpenApiMediaType
                                                            {
                                                                Schema = new OpenApiSchema
                                                                {
                                                                    Type = "array",
                                                                    Items = new OpenApiSchema
                                                                    {
                                                                        Reference = new OpenApiReference
                                                                        {
                                                                            Type = ReferenceType.Schema,
                                                                            Id = "schemaObject1"
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
                                }
                            }
                        }
                    },
                    Components = new OpenApiComponents
                    {
                        Schemas = new Dictionary<string, OpenApiSchema>
                        {
                            ["schemaObject1"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property7"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject2"
                                        }
                                    }
                                }
                            },
                            ["schemaObject2"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiDocument
                {
                    Paths = new OpenApiPaths
                    {
                        {
                            "/test", new OpenApiPathItem
                            {
                                Summary = "test",
                                Description = "test",
                                Operations = new Dictionary<OperationType, OpenApiOperation>
                                {
                                    {
                                        OperationType.Get, new OpenApiOperation
                                        {
                                            RequestBody = new OpenApiRequestBody
                                            {
                                                Description = "description",
                                                Required = true,
                                                Content =
                                                {
                                                    ["application/xml"] = new OpenApiMediaType
                                                    {
                                                        Schema = new OpenApiSchema
                                                        {
                                                            Reference = new OpenApiReference
                                                            {
                                                                Id = "schemaObject1",
                                                                Type = ReferenceType.Schema
                                                            }
                                                        }
                                                    }
                                                }
                                            },
                                            Responses = new OpenApiResponses
                                            {
                                                {
                                                    "200",
                                                    new OpenApiResponse
                                                    {
                                                        Description = "An updated complex object array response",
                                                        Content =
                                                        {
                                                            ["application/json"] = new OpenApiMediaType
                                                            {
                                                                Schema = new OpenApiSchema
                                                                {
                                                                    Type = "array",
                                                                    Items = new OpenApiSchema
                                                                    {
                                                                        Reference = new OpenApiReference
                                                                        {
                                                                            Type = ReferenceType.Schema,
                                                                            Id = "schemaObject1"
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                {
                                                    "400",
                                                    new OpenApiResponse
                                                    {
                                                        Description = "An updated complex object array response",
                                                        Content =
                                                        {
                                                            ["application/json"] = new OpenApiMediaType
                                                            {
                                                                Schema = new OpenApiSchema
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
                            }
                        }
                    },
                    Components = new OpenApiComponents
                    {
                        Schemas = new Dictionary<string, OpenApiSchema>
                        {
                            ["schemaObject1"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject2"
                                        }
                                    }
                                }
                            },
                            ["schemaObject2"] = new OpenApiSchema
                            {
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["property2"] = new OpenApiSchema
                                    {
                                        Type = "integer"
                                    },
                                    ["property5"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        MaxLength = 15
                                    },
                                    ["property6"] = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "schemaObject1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get/requestBody/content/application~1xml/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get/requestBody/content/application~1xml/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/requestBody/content/application~1xml/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/requestBody/content/application~1xml/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/paths/~1test/get/responses/400",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiResponse>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/responses/200/content/application~1json/items/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/responses/200/content/application~1json/items/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/responses/200/content/application~1json/items/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/paths/~1test/get/responses/200/content/application~1json/items/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    }
                }
            };
        }
    }
}