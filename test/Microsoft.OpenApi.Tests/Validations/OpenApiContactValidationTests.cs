// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiContactValidationTests
    {
        public static OpenApiContact Contact = new OpenApiContact()
        {
            Email = "support/example.com",
        };

        [Fact]
        public void SerializeAdvanceContactAsJsonWorks()
        {
            // Arrange
            IEnumerable<ValidationError> errors;

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
