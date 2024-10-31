// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiHeaderValidationTests
    {
        [Fact]
        public void ValidateExampleShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var header = new OpenApiHeader
            {
                Required = true,
                Example = 55,
                Schema = new OpenApiSchema
                { 
                    Type = JsonSchemaType.String
                }
            };

            // Act
            var defaultRuleSet = ValidationRuleSet.GetDefaultRuleSet();
            defaultRuleSet.Add(typeof(OpenApiHeader), OpenApiNonDefaultRules.HeaderMismatchedDataType);
            var validator = new OpenApiValidator(defaultRuleSet);

            var walker = new OpenApiWalker(validator);
            walker.Walk(header);

            errors = validator.Errors;
            var warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;

            var header = new OpenApiHeader
            {
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer
                    }
                },
                Examples =
                {
                    ["example0"] = new()
                    {
                        Value = "1",
                    },
                    ["example1"] = new()
                    {
                        Value = new JsonObject()
                        {
                            ["x"] = 2,
                            ["y"] = "20",
                            ["z"] = "200"
                        }
                    },
                    ["example2"] = new()
                    {
                        Value = new JsonArray(){3}
                    },
                    ["example3"] = new()
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
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(header);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeTrue();
        }
    }
}
