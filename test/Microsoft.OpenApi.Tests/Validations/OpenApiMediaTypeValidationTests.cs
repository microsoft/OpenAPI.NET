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
                Example = new OpenApiAny(55),
                Schema = new()
                {
                    Type = "string",
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
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;

            var mediaType = new OpenApiMediaType
            {
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
                            Value =new OpenApiAny(
                            new JsonArray(){3})
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
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(mediaType);

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
                "#/examples/example1/value/y",
                "#/examples/example1/value/z",
                "#/examples/example2/value"
            });
        }
    }
}
