// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests;

public static class OpenApiDocumentValidationTests
{
    [Fact]
    public static void ValidateSchemaReferencesAreValid()
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

        // Act
        var errors = document.Validate(ValidationRuleSet.GetDefaultRuleSet());
        var result = !errors.Any();

        // Assert
        Assert.True(result);
        Assert.NotNull(errors);
        Assert.Empty(errors);
    }

    [Fact]
    public static void ValidateSchemaReferencesAreInvalid()
    {
        // Arrange
        var document = new OpenApiDocument
        {
            Components = new OpenApiComponents(),
            Info = new OpenApiInfo
            {
                Title = "Pets Document",
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

        document.Paths.Add("/pets", new OpenApiPathItem
        {
            Operations = new Dictionary<HttpMethod, OpenApiOperation>()
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Responses = new()
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, OpenApiMediaType>()
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("Pet", document),
                                }
                            }
                        }
                    }
                }
            }
        });

        // Act
        var errors = document.Validate(ValidationRuleSet.GetDefaultRuleSet());
        var result = !errors.Any();

        // Assert
        Assert.False(result);
        Assert.NotNull(errors);
        var error = Assert.Single(errors);
        Assert.Equal("The schema reference '#/components/schemas/Pet' does not point to an existing schema.", error.Message);
        Assert.Equal("#/paths/~1pets/get/responses/200/content/application~1json/schema/$ref", error.Pointer);
    }
}
