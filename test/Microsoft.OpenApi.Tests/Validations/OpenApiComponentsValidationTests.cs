// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
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

            var components = new OpenApiComponents
            {
                Responses = new Dictionary<string, IOpenApiResponse>
                {
                    { key, new OpenApiResponse { Description = "any" } }
                }
            };

            var errors = components.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Act
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, "responses", OpenApiComponentsRules.KeyRegex.ToString()),
                error.Message);
        }
    }
}
