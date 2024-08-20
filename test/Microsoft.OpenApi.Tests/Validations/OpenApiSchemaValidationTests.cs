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
                Default = new OpenApiAny(55),
                Type = "string",
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/default",
            });
        }

        [Fact]
        public void ValidateExampleAndDefaultShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Example = new OpenApiAny(55),
                Default = new OpenApiAny("1234"),
                Type = "string",
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            bool result = !warnings.Any();
            var expectedWarnings = warnings.Select(e => e.Message).ToList();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/example",
            });
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
                Type = "object",
                AdditionalProperties = new()
                {
                    Type = "integer"
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                // #enum/0 is not an error since the spec allows
                // representing an object using a string.
                "#/enum/1/y",
                "#/enum/1/z",
                "#/enum/2"
            });
        }

        [Fact]
        public void ValidateDefaultShouldNotHaveDataTypeMismatchForComplexSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    ["property1"] = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Type = "integer",
                            Format = "int64"
                        }
                    },
                    ["property2"] = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Type = "object",
                            AdditionalProperties = new()
                            {
                                Type = "boolean"
                            }
                        }
                    },
                    ["property3"] = new()
                    {
                        Type = "string",
                        Format = "password"
                    },
                    ["property4"] = new()
                    {
                        Type = "string"
                    }
                },
                Default = new OpenApiAny(new JsonObject()
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
                })
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            bool result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/default/property1/2",
                "#/default/property2/0",
                "#/default/property2/1/z"
            });
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
                            Type = "object",
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
                            Type = "array",
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
                                                Type = "array"
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
