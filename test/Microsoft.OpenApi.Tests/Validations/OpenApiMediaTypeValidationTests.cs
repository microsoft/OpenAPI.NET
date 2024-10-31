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
    public class OpenApiMediaTypeValidationTests
    {
        [Fact]
        public void ValidateExampleShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var mediaType = new OpenApiMediaType
            {
                Example = 55,
                Schema = new()
                {
                    Type = JsonSchemaType.String,
                }
            };

            // Act
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(mediaType);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;

            var mediaType = new OpenApiMediaType
            {
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
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(mediaType);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeTrue();
        }
    }
}
