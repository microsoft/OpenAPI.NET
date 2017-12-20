// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiResponseValidationTests
    {
        [Fact]
        public void ValidateDescriptionIsRequiredInResponse()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiResponse response = new OpenApiResponse();

            // Act
            bool result = response.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "description", "response"), error.ErrorMessage);
            Assert.Equal(ErrorReason.Required, error.ErrorCode);
            Assert.Equal("#/description", error.ErrorPath);
        }
    }
}
