// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiInfoValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInInfo()
        {
            // Arrange
            var titleError = String.Format(SRResource.Validation_FieldIsRequired, "title", "info");
            var versionError = String.Format(SRResource.Validation_FieldIsRequired, "version", "info");
            var info = new OpenApiInfo();

            // Act
            var errors = info.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            var result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);

            Assert.Equal(new[] { titleError, versionError }, errors.Select(e => e.Message));
        }
    }
}
