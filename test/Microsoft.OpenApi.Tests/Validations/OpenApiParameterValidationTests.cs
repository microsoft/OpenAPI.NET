// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiParameterValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInParameter()
        {
            // Arrange
            var nameError = string.Format(SRResource.Validation_FieldIsRequired, "name", "parameter");
            var inError = string.Format(SRResource.Validation_FieldIsRequired, "in", "parameter");
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
            var parameter = new OpenApiParameter
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
            IEnumerable<OpenApiError> warnings;
            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Example = new OpenApiAny(55),
                Schema = new()
                {
                    Type = "string",
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("{parameter1}");
            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                "type : Value is \"integer\" but should be \"string\" at "
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                "#/{parameter1}/example",
            });
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;

            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new()
                {
                    Type = "object",
                    AdditionalProperties = new()
                    {
                        Type = "integer",
                    }
                },
                Examples =
                    {
                        ["example0"] = new()
                        {
                            Value = new OpenApiAny("1"),
                        },
                        ["example1"] = new()
                        {
                           Value = new OpenApiAny(new JsonObject()
                            {
                                ["x"] = 2,
                                ["y"] = "20",
                                ["z"] = "200"
                            })
                        },
                        ["example2"] = new()
                        {
                            Value = new OpenApiAny(new JsonArray(){3})
                        },
                        ["example3"] = new()
                        {
                            Value = new OpenApiAny(new JsonObject()
                            {
                                ["x"] = 4,
                                ["y"] = 40
                            })
                        },
                    }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Enter("{parameter1}");
            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                "type : Value is \"string\" but should be \"object\" at ",
                "type : Value is \"string\" but should be \"integer\" at /y",
                "type : Value is \"string\" but should be \"integer\" at /z",
                "type : Value is \"array\" but should be \"object\" at "
            });
            warnings.Select(e => e.Pointer).Should().BeEquivalentTo(new[]
            {
                // #enum/0 is not an error since the spec allows
                // representing an object using a string.
               "#/{parameter1}/examples/example0/value",
               "#/{parameter1}/examples/example1/value",
               "#/{parameter1}/examples/example1/value",
               "#/{parameter1}/examples/example2/value"
            });
        }

        [Fact]
        public void PathParameterNotInThePathShouldReturnAnError()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;

            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new()
                {
                    Type = "string",
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());

            var walker = new OpenApiWalker(validator);
            walker.Walk(parameter);

            errors = validator.Errors;
            var result = errors.Any();

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
        public void PathParameterInThePathShouldBeOk()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;

            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new()
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
            var result = errors.Any();

            // Assert
            result.Should().BeFalse();
        }
    }
}
