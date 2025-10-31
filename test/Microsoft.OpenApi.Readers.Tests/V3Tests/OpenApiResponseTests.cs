// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiResponse/";

        [Fact]
        public async Task ResponseWithReferencedHeaderShouldReferenceComponent()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "responseWithHeaderReference.yaml"), SettingsFixture.ReaderSettings);

            var response = result.Document.Components.Responses["Test"];
            var expected = response.Headers.First().Value;
            var actual = result.Document.Components.Headers.First().Value;

            Assert.Equal(expected.Description, actual.Description);
        }

        [Fact]
        public async Task ResponseWithSummaryV32ShouldDeserializeCorrectly()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "responseWithSummary.yaml"), SettingsFixture.ReaderSettings);

            var response = result.Document.Components.Responses["SuccessResponse"] as OpenApiResponse;
            
            Assert.NotNull(response);
            Assert.Equal("Successful response", response.Summary);
            Assert.Equal("A successful response with summary", response.Description);
        }

        [Fact]
        public async Task ResponseWithSummaryExtensionV31ShouldDeserializeCorrectly()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "responseWithSummaryExtension.yaml"), SettingsFixture.ReaderSettings);

            var response = result.Document.Components.Responses["SuccessResponse"] as OpenApiResponse;
            
            Assert.NotNull(response);
            Assert.Equal("Successful response", response.Summary);
            Assert.Equal("A successful response with summary extension", response.Description);
        }
    }
}
