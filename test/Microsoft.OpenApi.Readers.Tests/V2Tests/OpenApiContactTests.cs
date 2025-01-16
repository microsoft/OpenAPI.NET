// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
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
            var contact = OpenApiModelFactory.Parse<OpenApiContact>(input, OpenApiSpecVersion.OpenApi2_0, new(), out var diagnostic);

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
