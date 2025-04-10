// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaValidationTests
    {
        [Fact]
        public void ValidateDefaultShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Default = 55,
                Type = JsonSchemaType.String,
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateExampleAndDefaultShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Example = 55,
                Default = "1234",
                Type = JsonSchemaType.String,
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            bool result = !warnings.Any();
            var expectedWarnings = warnings.Select(e => e.Message).ToList();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateEnumShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema()
            {
                Enum =
                [
                    1,
                    new JsonObject()
                    {
                        ["x"] = 2,
                        ["y"] = "20",
                        ["z"] = "200"
                    },
                    new JsonArray() { 3 },
                    new JsonObject()
                    {
                        ["x"] = 4,
                        ["y"] = 40,
                    }
                ],
                Type = JsonSchemaType.Object,
                AdditionalProperties = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Integer
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateDefaultShouldNotHaveDataTypeMismatchForComplexSchema()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["property1"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int64"
                        }
                    },
                    ["property2"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Object,
                            AdditionalProperties = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Boolean
                            }
                        }
                    },
                    ["property3"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String,
                        Format = "password"
                    },
                    ["property4"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                },
                Default = new JsonObject()
                {
                    ["property1"] = new JsonArray()
                    {
                        12,
                        13,
                        "1",
                    },
                    ["property2"] = new JsonArray()
                    {
                        2,
                        new JsonObject()
                        {
                            ["x"] = true,
                            ["y"] = false,
                            ["z"] = "1234",
                        }
                    },
                    ["property3"] = "123",
                    ["property4"] = DateTime.UtcNow
                }
            };

            // Act
            var defaultRuleSet = ValidationRuleSet.GetDefaultRuleSet();
            defaultRuleSet.Add(typeof(IOpenApiSchema), OpenApiNonDefaultRules.SchemaMismatchedDataType);
            var validator = new OpenApiValidator(defaultRuleSet);
            var walker = new OpenApiWalker(validator);
            walker.Walk((IOpenApiSchema)schema);

            // Assert
            Assert.NotEmpty(validator.Warnings);
        }

        [Fact]
        public void ValidateSchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator()
        {
            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    {
                        "schema1",
                        new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Discriminator = new() { PropertyName = "property1" },
                        }
                    }
                }
            };
            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(components);

            // Assert
            Assert.NotEmpty(validator.Errors);
            Assert.Equivalent(new List<OpenApiValidatorError>
            {
                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateSchemaDiscriminator),"#/schemas/schema1/discriminator",
                        string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                    string.Empty, "property1"))
            }, validator.Errors);
        }

        [Fact]
        public void ValidateOneOfSchemaPropertyNameContainsPropertySpecifiedInTheDiscriminator()
        {
            // Arrange
            var components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    {
                        "Person",
                        new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            Discriminator = new()
                            {
                                PropertyName = "type"
                            },
                            OneOf =
                            [
                                new OpenApiSchema()
                                {
                                    Properties = new Dictionary<string, IOpenApiSchema>
                                    {
                                        {
                                            "type",
                                            new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Array
                                            }
                                        }
                                    },
                                }
                            ],
                        }
                    }
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(components);

            var errors = validator.Errors;

            //Assert
            Assert.Empty(errors);
        }
    }
}
