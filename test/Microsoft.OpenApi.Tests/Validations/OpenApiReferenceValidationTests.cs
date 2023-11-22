// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
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

            var document = new OpenApiDocument();
            document.Components = new()
            {
                Schemas = new Dictionary<string, JsonSchema>()
                {
                    ["test"] = sharedSchema
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
        public void UnresolvedSchemaReferencedShouldNotBeValidated()
        {
            // Arrange

            var sharedSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Ref("test").Build();

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
            var rules = new Dictionary<string, IList<ValidationRule>>()
            {
                { typeof(JsonSchema).Name,
                    new List<ValidationRule>() { new AlwaysFailRule<JsonSchema>() }
                }
            };

            var errors = document.Validate(new ValidationRuleSet(rules));

            // Assert
            Assert.True(!errors.Any());
        }
    }

    public class AlwaysFailRule<T> : ValidationRule<T>
    {
        public AlwaysFailRule() : base((c, _) => c.CreateError("x", "y"))
        {
        }
    }
}
