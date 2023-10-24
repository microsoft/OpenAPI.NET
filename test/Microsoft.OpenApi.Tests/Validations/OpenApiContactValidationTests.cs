﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.OpenApi.Extensions;
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

            var contact = new OpenApiContact
            {
                Email = testEmail
            };

            // Act
            var errors = contact.Validate(ValidationRuleSet.GetDefaultRuleSet());
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_StringMustBeEmailAddress, testEmail), error.Message);
        }
    }
}
