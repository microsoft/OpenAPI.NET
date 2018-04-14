// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
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
            OpenApiTag tag = new OpenApiTag();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Visit(tag);
            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            OpenApiError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "name", "tag"), error.Message);
        }

        [Fact]
        public void ValidateExtensionNameStartsWithXDashInTag()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            OpenApiTag tag = new OpenApiTag
            {
                Name = "tag"
            };
            tag.Extensions.Add("tagExt", new OpenApiString("value"));

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Visit(tag as IOpenApiExtensible);
            errors = validator.Errors;
            bool result = !errors.Any(); 

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            OpenApiError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, "tagExt", "#/extensions"), error.Message);
        }
    }
}
