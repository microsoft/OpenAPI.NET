// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiResponse/";

        public OpenApiResponseTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public async Task ResponseWithReferencedHeaderShouldReferenceComponent()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "responseWithHeaderReference.yaml"));

            var response = result.OpenApiDocument.Components.Responses["Test"];
            var expected = response.Headers.First().Value;
            var actual = result.OpenApiDocument.Components.Headers.First().Value;

            actual.Description.Should().BeEquivalentTo(expected.Description);
        }
    }
}
