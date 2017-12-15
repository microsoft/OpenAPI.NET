// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiLicenseValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInLicense()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiLicense license = new OpenApiLicense();

            // Act
            bool result = license.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal("The field 'name' in 'license' object is REQUIRED.", error.ErrorMessage);
        }
    }
}
