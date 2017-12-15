// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiComponentsValidationTests
    {
        [Fact]
        public void ValidateKeyMustMatchRegularExpressionInComponents()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiComponents components = new OpenApiComponents()
            {
                Responses = new Dictionary<string, OpenApiResponse>
                {
                    { "%@abc", new OpenApiResponse { Description = "any" } }
                }
            };

            // Act
            bool result = components.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(@"The key '%@abc' in 'responses' of components MUST match the regular expression '^[a-zA-Z0-9\.\-_]+$'.",
                error.ErrorMessage);
        }
    }
}
