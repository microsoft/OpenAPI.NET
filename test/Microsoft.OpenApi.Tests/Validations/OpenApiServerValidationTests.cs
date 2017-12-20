// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiServerValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInServer()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiServer server = new OpenApiServer();

            // Act
            bool result = server.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_FieldIsRequired, "url", "server"), error.ErrorMessage);
        }
    }
}
