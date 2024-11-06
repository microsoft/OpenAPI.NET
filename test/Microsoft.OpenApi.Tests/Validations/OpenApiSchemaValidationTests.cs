// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;
using Microsoft.OpenApi.Extensions;

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
            result.Should().BeTrue();
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
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateEnumShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema()
            {
                Enum = 
                {
                    new OpenApiAny("1").Node,
                    new OpenApiAny(new JsonObject()
                    {
                        ["x"] = 2,
                        ["y"] = "20",
                        ["z"] = "200"
                    }).Node,
                    new OpenApiAny(new JsonArray() { 3 }).Node,
                    new OpenApiAny(new JsonObject()
                    {
                        ["x"] = 4,
                        ["y"] = 40,
                    }).Node
                },
                Type = JsonSchemaType.Object,
                AdditionalProperties = new()
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
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateDefaultShouldNotHaveDataTypeMismatchForComplexSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties =
                {
                    ["property1"] = new()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new()
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int64"
                        }
                    },
                    ["property2"] = new()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new()
                        {
                            Type = JsonSchemaType.Object,
                            AdditionalProperties = new()
                            {
                                Type = JsonSchemaType.Boolean
                            }
                        }
                    },
                    ["property3"] = new()
                    {
                        Type = JsonSchemaType.String,
                        Format = "password"
                    },
                    ["property4"] = new()
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
            defaultRuleSet.Add(typeof(OpenApiSchema), OpenApiNonDefaultRules.SchemaMismatchedDataType);
            var validator = new OpenApiValidator(defaultRuleSet);
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            bool result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateSchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator()
        {
            IEnumerable<OpenApiError> errors;
            var components = new OpenApiComponents
            {
                Schemas = {
                    {
                        "schema1",
                        new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Discriminator = new() { PropertyName = "property1" },
                            Reference = new() { Id = "schema1" }
                        }
                    }
                }
            };
            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(components);

            errors = validator.Errors;
            var result = !errors.Any();

            // Assert
            result.Should().BeFalse();
            errors.Should().BeEquivalentTo(new List<OpenApiValidatorError>
            {
                    new OpenApiValidatorError(nameof(OpenApiSchemaRules.ValidateSchemaDiscriminator),"#/schemas/schema1/discriminator",
                        string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator,
                                    "schema1", "property1"))
            });
        }

        [Fact]
        public void ValidateOneOfSchemaPropertyNameContainsPropertySpecifiedInTheDiscriminator()
        {
            // Arrange
            var components = new OpenApiComponents
            {
                Schemas =
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
                            OneOf = new List<OpenApiSchema>
                            {
                                new()
                                {
                                    Properties =
                                    {
                                        {
                                            "type",
                                            new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Array
                                            }
                                        }
                                    },
                                    Reference = new()
                                    {
                                        Type = ReferenceType.Schema,
                                        Id = "Person"
                                    }
                                }
                            },
                            Reference = new() { Id = "Person" }
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
            errors.Should().BeEmpty();
        }
    }
}
