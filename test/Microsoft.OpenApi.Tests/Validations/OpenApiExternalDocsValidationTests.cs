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
    public class OpenApiExternalDocsValidationTests
    {
        [Fact]
        public void ValidateUrlIsRequiredInExternalDocs()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiExternalDocs externalDocs = new OpenApiExternalDocs();

            // Act
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(externalDocs);

            // Assert
            errors = validator.Errors;
            bool result = !errors.Any();

            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"), error.ErrorMessage);
        }
    }
}
