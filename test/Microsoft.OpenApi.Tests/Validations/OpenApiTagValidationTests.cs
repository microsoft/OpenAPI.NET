// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiTagValidationTests
    {
        [Fact]
        public void ValidateNameIsRequiredInTag()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var tag = new OpenApiTag();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Visit(tag);
            errors = validator.Errors;
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "name", "tag"), error.Message);
        }

        [Fact]
        public void ValidateExtensionNameStartsWithXDashInTag()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var tag = new OpenApiTag
            {
                Name = "tag"
            };
            tag.Extensions.Add("tagExt", new OpenApiAny("value"));

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Visit(tag as IOpenApiExtensible);
            errors = validator.Errors;
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, "tagExt", "#/extensions"), error.Message);
        }
    }
}
