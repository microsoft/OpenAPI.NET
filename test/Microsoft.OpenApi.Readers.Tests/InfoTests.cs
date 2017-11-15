// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class InfoTests
    {
        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.PetStore30.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var info = openApiDoc.Info;
            openApiDoc.Info.Title.Should().Be("Swagger Petstore (Simple)");
            info.Description.Should()
                .Be(
                    "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification");
            info.Version.Should().Be("1.0.0");
        }

        [Fact]
        public void ParseCompleteHeaderOpenApi()
        {
            // Arrange
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.CompleteHeader.yaml");

            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            // Assert
            openApiDoc.SpecVersion.ToString().Should().Be("1.0.0");

            openApiDoc.Paths.Should().BeEmpty();

            // verify info
            var info = openApiDoc.Info;
            info.Should().NotBeNull();
            info.Title.Should().Be("The Api");
            info.Version.Should().Be("0.9.1");
            info.Description.Should().Be("This is an api");
            info.TermsOfService.OriginalString.Should().Be("http://example.org/Dowhatyouwant");
            info.Contact.Name.Should().Be("Darrel Miller");

            // verify info's extensions
            info.Extensions.Should().NotBeNull();
            info.Extensions.Count.Should().Be(3);

            info.Extensions["x-something"].Should().BeOfType<OpenApiString>();
            var stringValue = (OpenApiString)(info.Extensions["x-something"]);
            stringValue.Value.Should().Be("Why does it start with x-, sigh");

            info.Extensions["x-contact"].Should().BeOfType<OpenApiObject>();
            var objValue = (OpenApiObject)(info.Extensions["x-contact"]);
            objValue.Count.Should().Be(3);
            objValue.Keys.Should().Equal("name", "url", "email");

            info.Extensions["x-list"].Should().BeOfType<OpenApiArray>();
            var arrayValue = (OpenApiArray)(info.Extensions["x-list"]);
            arrayValue.Count.Should().Be(2);
            arrayValue.Select(e => ((OpenApiString)e).Value).Should().Equal("1", "2");
        }
    }
}