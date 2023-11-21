// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Json.Schema;
using Json.Schema.OpenApi;
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
            var schema = new JsonSchemaBuilder().Default(new OpenApiAny(55).Node).Type(SchemaValueType.String);

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
            var schema = new JsonSchemaBuilder()
                .Default(new OpenApiAny("1234").Node)
                .Type(SchemaValueType.String)
                .Example(new OpenApiAny(55).Node)
                .Build();
            
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
                "#/example"
            });
        }

        [Fact]
        public void ValidateEnumShouldNotHaveDataTypeMismatchForSimpleSchema()
        {
            // Arrange
            IEnumerable<OpenApiError> warnings;
            var schema = new JsonSchemaBuilder()
                .Enum(
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
                    }).Node)
                .Type(SchemaValueType.Object)
                .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build())
                .Build();

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
            var schema = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .Properties(
                    ("property1",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(new JsonSchemaBuilder()
                                .Type(SchemaValueType.Integer).Format("int64").Build()).Build()),
                    ("property2",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Boolean).Build())
                                .Build())
                        .Build()),
                    ("property3",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                        .Format("password")
                        .Build()),
                    ("property4",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                        .Build()))
                .Default(new OpenApiAny(new JsonObject()
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
                }).Node).Build();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(schema);

            warnings = validator.Warnings;
            bool result = warnings.Any();

            // Assert
            result.Should().BeTrue();
            warnings.Select(e => e.Message).Should().BeEquivalentTo(new[]
            {
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
                RuleHelpers.DataTypeMismatchedErrorMessage,
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
                        new JsonSchemaBuilder()
                        .Type(SchemaValueType.Object)
                        .Discriminator(new OpenApiDiscriminator() { PropertyName = "property1" })
                        .Ref("schema1")
                        .Build()
                    }
                }
            };
            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk(components);

            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            result.Should().BeFalse();
            errors.Should().BeEquivalentTo(new List<OpenApiValidatorError>
            {
                    new OpenApiValidatorError(nameof(JsonSchemaRules.ValidateSchemaDiscriminator),"#/schemas/schema1/discriminator",
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
                        new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Discriminator(new OpenApiDiscriminator
                         {
                            PropertyName = "type"
                         })
                        .OneOf(new JsonSchemaBuilder()
                            .Properties(("type", new JsonSchemaBuilder().Type(SchemaValueType.Array).Ref("Person").Build()))
                            .Build())
                        .Ref("Person")
                        .Build()
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
