// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Reference = new OpenApiReference()
                {
                    Id = "test"
                },
                UnresolvedReference = false
            };

            OpenApiDocument document = new OpenApiDocument();
            document.Components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, OpenApiSchema>()
                {
                    [sharedSchema.Reference.Id] = sharedSchema
                }
            };

            document.Paths = new OpenApiPaths()
            {
                ["/"] = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType,OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation()
                        {
                            Responses = new OpenApiResponses()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Content = new Dictionary<string,OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType()
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
            var errors = document.Validate(new ValidationRuleSet() { new AlwaysFailRule<OpenApiSchema>()});


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
                Reference = new OpenApiReference()
                {
                    Id = "test"
                },
                UnresolvedReference = true
            };

            OpenApiDocument document = new OpenApiDocument();
            document.Components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, OpenApiSchema>()
                {
                    [sharedSchema.Reference.Id] = sharedSchema
                }
            };

            // Act
            var errors = document.Validate(new ValidationRuleSet() { new AlwaysFailRule<OpenApiSchema>() });

            // Assert
            Assert.True(errors.Count() == 0);
        }

        [Fact]
        public void UnresolvedSchemaReferencedShouldNotBeValidated()
        {
            // Arrange

            var sharedSchema = new OpenApiSchema
            {
                Reference = new OpenApiReference()
                {
                    Id = "test"
                },
                UnresolvedReference = true
            };

            OpenApiDocument document = new OpenApiDocument();

            document.Paths = new OpenApiPaths()
            {
                ["/"] = new OpenApiPathItem()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation()
                        {
                            Responses = new OpenApiResponses()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Content = new Dictionary<string, OpenApiMediaType>()
                                    {
                                        ["application/json"] = new OpenApiMediaType()
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
            var errors = document.Validate(new ValidationRuleSet() { new AlwaysFailRule<OpenApiSchema>() });

            // Assert
            Assert.True(errors.Count() == 0);
        }
    }

    public class AlwaysFailRule<T> : ValidationRule<T> where T: IOpenApiElement
    {
        public AlwaysFailRule() : base( (c,t) =>  c.CreateError("x","y"))
        {
            
        }
    }
}
