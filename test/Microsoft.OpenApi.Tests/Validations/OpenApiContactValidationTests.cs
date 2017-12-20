// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
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
            bool result = Contact.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_StringMustBeEmailAddress, testEmail), error.ErrorMessage);
        }
    }
}
