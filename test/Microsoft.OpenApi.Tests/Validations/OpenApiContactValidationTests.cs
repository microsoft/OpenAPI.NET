// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiContactValidationTests
    {
        [Fact]
        public void ValidateEmailFieldIsEmailAddressInContact()
        {
            // Arrange
            const string testEmail = "support/example.com";
            IEnumerable<ValidationError> errors;
            OpenApiContact Contact = new OpenApiContact()
            {
                Email = testEmail
            };

            // Act
            var validator = new OpenApiValidator();
            validator.Visit(Contact);

            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_StringMustBeEmailAddress, testEmail), error.ErrorMessage);
        }
    }
}
