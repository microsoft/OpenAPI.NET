// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
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
            var nameError = string.Format(SRResource.Validation_FieldIsRequired, "name", "parameter");
            var inError = string.Format(SRResource.Validation_FieldIsRequired, "in", "parameter");
            var parameter = new OpenApiParameter();

            // Act
            var errors = parameter.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            Assert.NotEmpty(errors);
            Assert.Equivalent(new[]
            {
                nameError,
                inError
            }, errors.Select(e => e.Message));
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
            walker.Walk((IOpenApiParameter)parameter);
            var errors = validator.Errors;
            // Assert
            Assert.NotEmpty(errors);
            Assert.Equivalent(new[]
            {
                "\"required\" must be true when parameter location is \"path\""
            }, errors.Select(e => e.Message));
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
                Example = 55,
                Schema = new()
                {
                    Type = JsonSchemaType.String,
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
            Assert.True(result);
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new()
                {
                    Type = JsonSchemaType.Object,
                    AdditionalProperties = new()
                    {
                        Type = JsonSchemaType.Integer,
                    }
                },
                Examples =
                    {
                        ["example0"] = new OpenApiExample()
                        {
                            Value = "1",
                        },
                        ["example1"] = new OpenApiExample()
                        {
                           Value = new JsonObject()
                            {
                                ["x"] = 2,
                                ["y"] = "20",
                                ["z"] = "200"
                            }
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value = new JsonArray(){3}
                        },
                        ["example3"] = new OpenApiExample()
                        {
                            Value = new JsonObject()
                            {
                                ["x"] = 4,
                                ["y"] = 40
                            }
                        },
                    }
            };

            // Act
            var defaultRuleSet = ValidationRuleSet.GetDefaultRuleSet();
            defaultRuleSet.Add(typeof(IOpenApiParameter), OpenApiNonDefaultRules.ParameterMismatchedDataType);

            var validator = new OpenApiValidator(defaultRuleSet);
            validator.Enter("{parameter1}");
            var walker = new OpenApiWalker(validator);
            walker.Walk((IOpenApiParameter)parameter);

            // Assert
            Assert.NotEmpty(validator.Warnings);
        }

        [Fact]
        public void PathParameterNotInThePathShouldReturnAnError()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "parameter1",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new()
                {
                    Type = JsonSchemaType.String,
                }
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());

            var walker = new OpenApiWalker(validator);
            walker.Walk((IOpenApiParameter)parameter);

            // Assert
            Assert.NotEmpty(validator.Errors);
            Assert.Equivalent(new[]
            {
                "PathParameterShouldBeInThePath"
            }, validator.Errors.OfType<OpenApiValidatorError>().Select(e => e.RuleName));
            Assert.Equivalent(new[]
            {
                "#/in"
            }, validator.Errors.Select(e => e.Pointer));
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
                    Type = JsonSchemaType.String,
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
            Assert.False(result);
        }
    }
}
