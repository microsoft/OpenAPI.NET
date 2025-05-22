// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Services;
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
                Schema = new OpenApiSchema()
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
            Assert.True(result);
        }

        [Fact]
        public void ValidateExamplesShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;

            var mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Object,
                    AdditionalProperties = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Integer,
                    }
                },
                Examples = new Dictionary<string, IOpenApiExample>
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
            var ruleset = ValidationRuleSet.GetDefaultRuleSet();
            var validator = new OpenApiValidator(ruleset);
            var walker = new OpenApiWalker(validator);
            walker.Walk(mediaType);

            warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            Assert.True(result);
        }
    }
}
