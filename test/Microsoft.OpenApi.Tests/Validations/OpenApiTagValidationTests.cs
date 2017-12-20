// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
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
            IEnumerable<ValidationError> errors;
            OpenApiTag tag = new OpenApiTag();

            // Act
            bool result = tag.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "name", "tag"), error.ErrorMessage);
        }

        [Fact]
        public void ValidateExtensionNameStartsWithXDashInTag()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiTag tag = new OpenApiTag
            {
                Name = "tag"
            };
            tag.Extensions.Add("tagExt", new OpenApiString("value"));

            // Act
            bool result = tag.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, "tagExt", "#/extensions"), error.ErrorMessage);
        }
    }
}
