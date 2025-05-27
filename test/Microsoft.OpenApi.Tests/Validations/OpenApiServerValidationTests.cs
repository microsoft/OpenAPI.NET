// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiServerValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInServer()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var server = new OpenApiServer();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            validator.Visit(server);
            errors = validator.Errors;
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);
            var error = Assert.Single(errors);
            Assert.Equal(string.Format(SRResource.Validation_FieldIsRequired, "url", "server"), error.Message);
        }
    }
}
