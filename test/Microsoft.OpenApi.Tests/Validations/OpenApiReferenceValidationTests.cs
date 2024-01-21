// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Validations
{
    public class OpenApiReferenceValidationTests
    {
        [Fact]
        public void ReferencedSchemaShouldOnlyBeValidatedOnce()
        {
            // Arrange

            var sharedSchema = new OpenApiSchema
            {
                Type = "string",
                Reference = new()
                {
                    Id = "test"
                },
                UnresolvedReference = false
            };

            var document = new OpenApiDocument();
            document.Components = new()
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    [sharedSchema.Reference.Id] = sharedSchema
                }
            };

            document.Paths = new()
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
                                            Schema = sharedSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var errors = document.Validate(new() { new AlwaysFailRule<OpenApiSchema>() });

            // Assert
            Assert.True(errors.Count() == 1);
        }

        [Fact]
        public void UnresolvedReferenceSchemaShouldNotBeValidated()
        {
            // Arrange
            var sharedSchema = new OpenApiSchema
            {
                Type = "string",
                Reference = new()
                {
                    Id = "test"
                },
                UnresolvedReference = true
            };

            var document = new OpenApiDocument();
            document.Components = new()
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    [sharedSchema.Reference.Id] = sharedSchema
                }
            };

            // Act
            var errors = document.Validate(new() { new AlwaysFailRule<OpenApiSchema>() });

            // Assert
            Assert.True(errors.Count() == 0);
        }

        [Fact]
        public void UnresolvedSchemaReferencedShouldNotBeValidated()
        {
            // Arrange

            var sharedSchema = new OpenApiSchema
            {
                Reference = new()
                {
                    Id = "test"
                },
                UnresolvedReference = true
            };

            var document = new OpenApiDocument();

            document.Paths = new()
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
                                            Schema = sharedSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var errors = document.Validate(new() { new AlwaysFailRule<OpenApiSchema>() });

            // Assert
            Assert.True(errors.Count() == 0);
        }
    }

    public class AlwaysFailRule<T> : ValidationRule<T> where T : IOpenApiElement
    {
        public AlwaysFailRule() : base("AlwaysFailRule", (c, _) => c.CreateError("x", "y"))
        {
        }
    }
}
