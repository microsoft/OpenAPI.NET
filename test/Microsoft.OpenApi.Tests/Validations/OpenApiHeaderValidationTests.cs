// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Json.Schema;
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
                Example = new OpenApiAny(55),
                Schema = new JsonSchemaBuilder().Type(SchemaValueType.String)
            };

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(header);

            errors = validator.Errors;
            var warnings = validator.Warnings;
            var result = !warnings.Any();

            // Assert
            result.Should().BeFalse();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                "type : Value is \"integer\" but should be \"string\" at "
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

            var header = new OpenApiHeader
            {
                Required = true,
                Schema = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .AdditionalProperties(
                    new JsonSchemaBuilder()
                    .Type(SchemaValueType.Integer)
                    .Build())
                .Build(),
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
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(header);

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
                "#/examples/example0/value",
                "#/examples/example1/value",
                "#/examples/example1/value",
                "#/examples/example2/value"
            });
        }
    }
}
