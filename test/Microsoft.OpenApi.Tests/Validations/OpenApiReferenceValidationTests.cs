// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                Type = JsonSchemaType.String,
            };

            var document = new OpenApiDocument();
            document.Components = new()
            {
                Schemas = new()
                {
                    ["test"] = sharedSchema
                }
            };

            document.Paths = new()
            {
                ["/"] = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Get] = new()
                        {
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Content = new()
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchemaReference("test")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var rules = new Dictionary<Type, List<ValidationRule>>()
            {
                { typeof(IOpenApiSchema),
                    new List<ValidationRule>() { new AlwaysFailRule<IOpenApiSchema>() }
                }
            };

            var errors = document.Validate(new ValidationRuleSet(rules));


            // Assert
            Assert.Single(errors);
        }

        [Fact]
        public void UnresolvedSchemaReferencedShouldNotBeValidated()
        {
            // Arrange

            var sharedSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
            };

            var document = new OpenApiDocument();
            document.AddComponent("test", sharedSchema);

            document.Paths = new()
            {
                ["/"] = new OpenApiPathItem()
                {
                    Operations = new()
                    {
                        [HttpMethod.Get] = new()
                        {
                            Responses = new()
                            {
                                ["200"] = new OpenApiResponse()
                                {
                                    Content = new()
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchemaReference("test")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var rules = new Dictionary<Type, List<ValidationRule>>()
            {
                { typeof(OpenApiSchema),
                    new List<ValidationRule>() { new AlwaysFailRule<OpenApiSchema>() }
                }
            };

            var errors = document.Validate(new ValidationRuleSet(rules));

            // Assert
            Assert.True(!errors.Any());
        }
    }

    public class AlwaysFailRule<T> : ValidationRule<T>
    {
        public AlwaysFailRule() : base("AlwaysFailRule", (c, _) => c.CreateError("x", "y"))
        {
        }
    }
}
