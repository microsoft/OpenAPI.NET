// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
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
            Assert.Equal("The field 'url' in 'External Documentation' object is REQUIRED.", error.ErrorMessage);
        }
    }
}
