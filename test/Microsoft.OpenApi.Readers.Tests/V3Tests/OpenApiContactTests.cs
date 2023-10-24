// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using FluentAssertions;
using Microsoft.OpenApi.Models;
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
            var reader = new OpenApiStringReader();

            // Act
            var contact = reader.ReadFragment<OpenApiContact>(input, OpenApiSpecVersion.OpenApi3_0, out var diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            contact.Should().BeEquivalentTo(
                new OpenApiContact
                {
                    Email = "support@swagger.io",
                    Name = "API Support",
                    Url = new("http://www.swagger.io/support")
                });
        }
    }
}
