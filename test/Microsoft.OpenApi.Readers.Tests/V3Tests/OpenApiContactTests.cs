// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    public class OpenApiContactTests
    {
        [Fact]
        public void ParseStringContactFragmentShouldSucceed()
        {
            var input =
                """
                {
                  "name": "API Support",
                  "url": "http://www.swagger.io/support",
                  "email": "support@swagger.io"
                }
                """;

            // Act
            var contact = OpenApiModelFactory.Parse<OpenApiContact>(input, OpenApiSpecVersion.OpenApi3_0, new(), out var diagnostic, OpenApiConstants.Json);

            // Assert
            Assert.Equivalent(new OpenApiDiagnostic(), diagnostic);

            Assert.Equivalent(
                new OpenApiContact
                {
                    Email = "support@swagger.io",
                    Name = "API Support",
                    Url = new("http://www.swagger.io/support")
                }, contact);
        }
    }
}
