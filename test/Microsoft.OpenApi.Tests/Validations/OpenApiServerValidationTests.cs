// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
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
            Assert.Equal("The field 'url' in 'Server' object is REQUIRED.", error.ErrorMessage);
        }
    }
}
