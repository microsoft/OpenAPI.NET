// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiContactTests
    {
        [Fact]
        public async Task ParseStringContactFragmentShouldSucceed()
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
            var contact = await OpenApiModelFactory.ParseAsync<OpenApiContact>(input, OpenApiSpecVersion.OpenApi2_0);

            // Assert
            contact.OpenApiDiagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            contact.Element.Should().BeEquivalentTo(
                new OpenApiContact
                {
                    Email = "support@swagger.io",
                    Name = "API Support",
                    Url = new("http://www.swagger.io/support")
                });
        }
    }
}
