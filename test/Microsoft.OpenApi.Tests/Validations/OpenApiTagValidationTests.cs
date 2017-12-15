// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Tests.Models
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
            Assert.Equal("The field 'name' in 'Tag' object is REQUIRED.", error.ErrorMessage);
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
            Assert.Equal("The extension name 'tagExt' in '#/extensions' object MUST begin with 'x-'.", error.ErrorMessage);
        }
    }
}
