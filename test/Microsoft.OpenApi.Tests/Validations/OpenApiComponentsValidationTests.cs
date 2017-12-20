// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Validations.Rules;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiComponentsValidationTests
    {
        [Fact]
        public void ValidateKeyMustMatchRegularExpressionInComponents()
        {
            // Arrange
            const string key = "%@abc";
            IEnumerable<ValidationError> errors;
            OpenApiComponents components = new OpenApiComponents()
            {
                Responses = new Dictionary<string, OpenApiResponse>
                {
                    { key, new OpenApiResponse { Description = "any" } }
                }
            };

            // Act
            bool result = components.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            ValidationError error = Assert.Single(errors);
            Assert.Equal(String.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, "responses", OpenApiComponentsRules.KeyRegex.ToString()),
                error.ErrorMessage);
        }
    }
}
