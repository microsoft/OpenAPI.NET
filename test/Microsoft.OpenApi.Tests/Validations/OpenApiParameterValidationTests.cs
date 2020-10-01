// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiParameterValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInParameter()
        {
            // Arrange
            string nameError = String.Format(SRResource.Validation_FieldIsRequired, "name", "parameter");
            string inError = String.Format(SRResource.Validation_FieldIsRequired, "in", "parameter");
            var parameter = new OpenApiParameter();

            // Act
            var errors = parameter.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            errors.Should().NotBeEmpty();
            errors.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                nameError,
                inError
            });
        }

        [Fact]
        public void ValidateRequiredIsTrueWhenInIsPathInParameter()
        {
            // Arrange
            var parameter = new OpenApiParameter()
            {
                Name = "name",
                In = ParameterLocation.Path
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("{name}");
            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);
            var errors = validator.Errors;
            // Assert
            errors.Should().NotBeEmpty();
            errors.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                "\"required\" must be true when parameter location is \"path\""
            });
        }

        [Fact]
        public void ValidateExampleShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var parameter = new OpenApiParameter()
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Example = new OpenApiInteger(55),
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("{parameter1}");
            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            result.Should().BeFalse();
            errors.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage
            });
            errors.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/{parameter1}/example",
            });
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;

            var parameter = new OpenApiParameter()
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "object",
                    AdditionalProperties = new OpenApiSchema()
                    {
                        Type = "integer",
                    }
                },
                Examples =
                    {
                        ["example0"] = new OpenApiExample()
                        {
                            Value = new OpenApiString("1"),
                        },
                        ["example1"] = new OpenApiExample()
                        {
                           Value = new OpenApiObject()
                            {
                                ["x"] = new OpenApiInteger(2),
                                ["y"] = new OpenApiString("20"),
                                ["z"] = new OpenApiString("200")
                            }
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value =
                            new OpenApiArray()
                            {
                                new OpenApiInteger(3)
                            }
                        },
                        ["example3"] = new OpenApiExample()
                        {
                            Value = new OpenApiObject()
                            {
                                ["x"] = new OpenApiInteger(4),
                                ["y"] = new OpenApiInteger(40),
                            }
                        },
                    }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("{parameter1}");
            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            result.Should().BeFalse();
            errors.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
            });
            errors.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                // #enum/0 is not an error since the spec allows
                // representing an object using a string.
                "#/{parameter1}/examples/example1/value/y",
                "#/{parameter1}/examples/example1/value/z",
                "#/{parameter1}/examples/example2/value"
            });
        }

        [Fact]
        public void PathParameterNotInThePathShouldReturnAnError()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;

            var parameter = new OpenApiParameter()
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());

            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            errors = validator.Errors;
            bool result = errors.Any();

            // Assert
            result.Should().BeTrue();
            errors.OfType<OpenApiValidatorError>().Select(e => e.RuleName).Should().BeEquivalentTo(new[]
            {
                "PathParameterShouldBeInThePath"
            });
            errors.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/in"
            });
        }

        [Fact]
        public void PathParameterInThePastShouldBeOk()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;

            var parameter = new OpenApiParameter()
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema()
                {
                    Type = "string",
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("paths");
            validator.Enter("/{parameter1}");
            validator.Enter("get");
            validator.Enter("parameters");
            validator.Enter("1");

            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            errors = validator.Errors;
            bool result = errors.Any();

            // Assert
            result.Should().BeFalse();
        }
    }
}
