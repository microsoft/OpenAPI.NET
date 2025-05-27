// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiResponseValidationTests
    {
        [Fact]
        public void ValidateDescriptionIsRequiredInResponse()
        {
            // Arrange
            IEnumerable<OpenApiError> errors;
            var response = new OpenApiResponse();

            // Act
            var validator = new OpenApiValidator(ValidationRuleSet.GetDefaultRuleSet());
            var walker = new OpenApiWalker(validator);
            walker.Walk((IOpenApiResponse)response);

            errors = validator.Errors;

            // Assert
            Assert.NotEmpty(errors);
            Assert.NotNull(errors);
            Assert.Single(errors);
            var error = Assert.IsType<OpenApiValidatorError>(errors.First());
            Assert.Equal(string.Format(SRResource.Validation_FieldIsRequired, "description", "response"), error.Message);
            Assert.Equal("#/description", error.Pointer);
        }
    }
}
