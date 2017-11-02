// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.YamlReaders;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class InfoTests
    {
        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.petstore30.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var info = openApiDoc.Info;
            Assert.Equal("Swagger Petstore (Simple)", openApiDoc.Info.Title);
            Assert.Equal(
                "A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification",
                info.Description);
            Assert.Equal("1.0.0", info.Version.ToString());
        }

        [Fact]
        public void ParseCompleteHeaderOpenApi()
        {
            // Arrange
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.CompleteHeader.yaml");

            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            // Assert
            Assert.Equal("1.0.0", openApiDoc.SpecVersion.ToString());

            Assert.Empty(openApiDoc.Paths);

            // verify info
            var info = openApiDoc.Info;
            Assert.NotNull(info);
            Assert.Equal("The Api", info.Title);
            Assert.Equal("0.9.1", info.Version.ToString());
            Assert.Equal("This is an api", info.Description);
            Assert.Equal("http://example.org/Dowhatyouwant", info.TermsOfService.OriginalString);
            Assert.Equal("Darrel Miller", info.Contact.Name);

            // verify info's extensions
            Assert.NotNull(info.Extensions);
            Assert.Equal(3, info.Extensions.Count);

            OpenApiString stringValue = Assert.IsType<OpenApiString>(info.Extensions["x-something"]);
            Assert.Equal("Why does it start with x-, sigh", stringValue.Value);

            OpenApiObject objValue = Assert.IsType<OpenApiObject>(info.Extensions["x-contact"]);
            Assert.Equal(3, objValue.Count);
            Assert.Equal(new[] { "name", "url", "email" }, objValue.Keys);

            OpenApiArray arrayValue = Assert.IsType<OpenApiArray>(info.Extensions["x-list"]);
            Assert.Equal(2, arrayValue.Count);
            Assert.Equal(new[] { "1", "2" }, arrayValue.Select(e => ((OpenApiString)e).Value));
        }
    }
}