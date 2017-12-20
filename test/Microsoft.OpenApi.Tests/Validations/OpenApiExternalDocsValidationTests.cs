// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
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
            bool result = externalDocs.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"), error.ErrorMessage);
        }
    }
}
