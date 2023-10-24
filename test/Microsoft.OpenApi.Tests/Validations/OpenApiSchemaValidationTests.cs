// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
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
                Default = new OpenApiInteger(55),
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
                Example = new OpenApiLong(55),
                Default = new OpenApiPassword("1234"),
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
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/default",
                "#/example",
            });
        }

        [Fact]
        public void ValidateEnumShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new OpenApiSchema
            {
                Enum =
                {
                    new OpenApiString("1"),
                    new OpenApiObject
                    {
                        ["x"] = new OpenApiInteger(2),
                        ["y"] = new OpenApiString("20"),
                        ["z"] = new OpenApiString("200")
                    },
                    new OpenApiArray
                    {
                        new OpenApiInteger(3)
                    },
                    new OpenApiObject
                    {
                        ["x"] = new OpenApiInteger(4),
                        ["y"] = new OpenApiInteger(40),
                    },
                },
                Type = "object",
                AdditionalProperties = new()
                {
                    Type = "integer",
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
                Default = new OpenApiObject
                {
                    ["property1"] = new OpenApiArray
                    {
                        new OpenApiInteger(12),
                        new OpenApiLong(13),
                        new OpenApiString("1"),
                    },
                    ["property2"] = new OpenApiArray
                    {
                        new OpenApiInteger(2),
                        new OpenApiObject
                        {
                            ["x"] = new OpenApiBoolean(true),
                            ["y"] = new OpenApiBoolean(false),
                            ["z"] = new OpenApiString("1234"),
                        }
                    },
                    ["property3"] = new OpenApiPassword("123"),
                    ["property4"] = new OpenApiDateTime(DateTime.UtcNow)
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
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/default/property1/0",
                "#/default/property1/2",
                "#/default/property2/0",
                "#/default/property2/1/z",
                "#/default/property4",
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
                    new(nameof(OpenApiSchemaRules.ValidateSchemaDiscriminator),"#/schemas/schema1/discriminator",
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
