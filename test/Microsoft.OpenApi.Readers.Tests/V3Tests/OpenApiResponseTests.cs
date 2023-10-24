// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiResponseTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiResponse/";

        [Fact]
        public void ResponseWithReferencedHeaderShouldReferenceComponent()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "responseWithHeaderReference.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            var response = openApiDoc.Components.Responses["Test"];

            Assert.Same(response.Headers.First().Value, openApiDoc.Components.Headers.First().Value);
        }
    }
}
