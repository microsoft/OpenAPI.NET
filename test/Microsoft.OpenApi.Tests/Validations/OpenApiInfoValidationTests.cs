// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Validations.Tests
{
    public class OpenApiInfoValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInInfo()
        {
            // Arrange
            string urlError = String.Format(SRResource.Validation_FieldIsRequired, "title", "info");
            string versionError = String.Format(SRResource.Validation_FieldIsRequired, "version", "info");
            OpenApiInfo info = new OpenApiInfo();

            // Act
            var errors = info.Validate(ValidationRuleSet.GetDefaultRuleSet());

            // Assert
            bool result = !errors.Any();

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);

            Assert.Equal(new[] { urlError, versionError }, errors.Select(e => e.Message));
        }
    }
}
