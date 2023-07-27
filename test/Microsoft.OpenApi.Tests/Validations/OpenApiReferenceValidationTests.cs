// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Json.Schema;
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

            var sharedSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Ref("test");

            OpenApiDocument document = new OpenApiDocument();
            document.Components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, JsonSchema>()
                {
                    ["test"] = sharedSchema
                }
            };

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
            var rules = new Dictionary<string, IList<ValidationRule>>()
            {
                { typeof(JsonSchema).Name,
                    new List<ValidationRule>() { new AlwaysFailRule<JsonSchema>() }
                }
            };
            
            var errors = document.Validate(new ValidationRuleSet(rules));


            // Assert
            Assert.True(errors.Count() == 1);
        }

        [Fact]
        public void UnresolvedReferenceSchemaShouldNotBeValidated()
        {
            // Arrange
            var sharedSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Ref("test");

            OpenApiDocument document = new OpenApiDocument();
            document.Components = new OpenApiComponents()
            {
                Schemas = new Dictionary<string, JsonSchema>()
                {
                    ["test"] = sharedSchema
                }
            };

            // Act
            var rules = new Dictionary<string, IList<ValidationRule>>()
            {
                { typeof(JsonSchema).Name,
                    new List<ValidationRule>() { new AlwaysFailRule<JsonSchema>() }
                }
            };

            var errors = document.Validate(new ValidationRuleSet(rules));

            // Assert
            Assert.True(errors.Count() == 0);
        }

        [Fact]
        public void UnresolvedSchemaReferencedShouldNotBeValidated()
        {
            // Arrange

            var sharedSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Ref("test").Build();

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
            var rules = new Dictionary<string, IList<ValidationRule>>()
            {
                { typeof(JsonSchema).Name,
                    new List<ValidationRule>() { new AlwaysFailRule<JsonSchema>() }
                }
            };

            var errors = document.Validate(new ValidationRuleSet(rules));

            // Assert
            Assert.True(errors.Count() == 0);
        }
    }

    public class AlwaysFailRule<T> : ValidationRule<T>
    {
        public AlwaysFailRule() : base((c, t) => c.CreateError("x", "y"))
        {

        }
    }
}
