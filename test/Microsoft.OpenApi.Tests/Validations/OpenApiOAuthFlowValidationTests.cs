// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiOAuthFlowValidationTests
    {
        [Fact]
        public void ValidateFixedFieldsIsRequiredInResponse()
        {
            // Arrange
            string authorizationUrlError = String.Format(SRResource.Validation_FieldIsRequired, "authorizationUrl", "OAuth Flow");
            string tokenUrlError = String.Format(SRResource.Validation_FieldIsRequired, "tokenUrl", "OAuth Flow");
            IEnumerable<ValidationError> errors;
            OpenApiOAuthFlow oAuthFlow = new OpenApiOAuthFlow();

            // Act
            bool result = oAuthFlow.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            Assert.Equal(2, errors.Count());
            Assert.Equal(new[] { authorizationUrlError, tokenUrlError }, errors.Select(e => e.ErrorMessage));
        }
    }
}
