// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
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
            OpenApiExternalDocs externalDocs = new OpenApiExternalDocs();

            // Act
            var errors = externalDocs.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
          
            bool result = !errors.Any();

            Assert.False(result);
            Assert.NotNull(errors);
            OpenApiError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"), error.Message);
        }
    }
}
