// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiOAuthFlowValidationTests
    {
        [Fact]
        public void ValidateFixedFieldsIsRequiredInResponse()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiOAuthFlow oAuthFlow = new OpenApiOAuthFlow();

            // Act
            bool result = oAuthFlow.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            Assert.Equal(2, errors.Count());
            Assert.Equal(new[] { "The field 'authorizationUrl' in 'OAuth Flow' object is REQUIRED.",
                "The field 'tokenUrl' in 'OAuth Flow' object is REQUIRED."}, errors.Select(e => e.ErrorMessage));
        }
    }
}
