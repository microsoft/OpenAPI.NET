// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests;

public static class OpenApiRecommendedRulesTests
{
    [Fact]
    public static void GetOperationWithoutRequestBodyIsValid()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents(),
            Info = new OpenApiInfo
            {
                Title = "People Document",
                Version = "1.0.0"
            },
            Paths = [],
            Workspace = new()
        };

        document.AddComponent("Person", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["email"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "email" }
            }
        });

        document.Paths.Add("/people", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>()
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    RequestBody = null,
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                },
                [HttpMethod.Post] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("Person", document),
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                }
            }
        });

        var ruleSet = new ValidationRuleSet();
        ruleSet.Add(typeof(OpenApiPaths), OpenApiRecommendedRules.GetOperationShouldNotHaveRequestBody);

        // Act
        var warnings = document.Validate(ruleSet);
        var result = !warnings.Any();

        // Assert
        Assert.True(result);
        Assert.NotNull(warnings);
        Assert.Empty(warnings);
    }

    [Fact]
    public static void GetOperationWithRequestBodyIsInvalid()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents(),
            Info = new OpenApiInfo
            {
                Title = "People Document",
                Version = "1.0.0"
            },
            Paths = [],
            Workspace = new()
        };

        document.AddComponent("Person", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["email"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "email" }
            }
        });

        document.Paths.Add("/people", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>()
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("Person", document),
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                },
                [HttpMethod.Post] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("Person", document),
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                }
            }
        });

        var ruleSet = new ValidationRuleSet();
        ruleSet.Add(typeof(OpenApiPaths), OpenApiRecommendedRules.GetOperationShouldNotHaveRequestBody);

        // Act
        var warnings = document.Validate(ruleSet);
        var result = !warnings.Any();

        // Assert
        Assert.False(result);
        Assert.NotNull(warnings);
        var warning = Assert.Single(warnings);
        Assert.Equal("GET operations should not have a request body.", warning.Message);
        Assert.Equal("#/paths//people/get/requestBody", warning.Pointer);
    }

    [Fact]
    public static void GetOperationWithRequestBodyIsValidUsingDefaultRuleSet()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents(),
            Info = new OpenApiInfo
            {
                Title = "People Document",
                Version = "1.0.0"
            },
            Paths = [],
            Workspace = new()
        };

        document.AddComponent("Person", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>()
            {
                ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["email"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "email" }
            }
        });

        document.Paths.Add("/people", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>()
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("Person", document),
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                },
                [HttpMethod.Post] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("Person", document),
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Person", document),
                                }
                            }
                        }
                    }
                }
            }
        });

        var ruleSet = ValidationRuleSet.GetDefaultRuleSet();

        // Act
        var warnings = document.Validate(ruleSet);
        var result = !warnings.Any();

        // Assert
        Assert.True(result);
        Assert.NotNull(warnings);
        Assert.Empty(warnings);
    }
}
