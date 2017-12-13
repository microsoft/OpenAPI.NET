// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiInfoValidationTests
    {
        [Fact]
        public void ValidateFieldIsRequiredInInfo()
        {
            // Arrange
            IEnumerable<ValidationError> errors;
            OpenApiInfo info = new OpenApiInfo();

            // Act
            bool result = info.Validate(out errors);

            // Assert
            Assert.False(result);
            Assert.NotNull(errors);

            Assert.Equal(new[] { "The field 'url' in 'info' object is REQUIRED.",
                "The field 'version' in 'info' object is REQUIRED."}, errors.Select(e => e.ErrorMessage));
        }
    }
}
