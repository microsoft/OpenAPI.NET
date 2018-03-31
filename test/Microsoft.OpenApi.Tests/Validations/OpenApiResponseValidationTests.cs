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
    public class OpenApiResponseValidationTests
    {
        [Fact]
        public void ValidateDescriptionIsRequiredInResponse()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            OpenApiResponse response = new OpenApiResponse();

            // Act
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(response);

            errors = validator.Errors;
            bool result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            OpenApiError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "description", "response"), error.Message);
            Assert.Equal(ErrorReason.Required, error.ReasonClass);
            Assert.Equal("#/description", error.Pointer);
        }
    }
}
