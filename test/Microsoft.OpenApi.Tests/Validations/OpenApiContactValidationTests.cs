// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiContactValidationTests
    {
        [Fact]
        public void ValidateEmailFieldIsEmailAddressInContact()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiContact Contact = new OpenApiContact()
            {
                Email = "support/example.com",
            };

            // Act
            bool result = Contact.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal("The string 'support/example.com' MUST be in the format of an email address.", error.ErrorMessage);
        }
    }
}
