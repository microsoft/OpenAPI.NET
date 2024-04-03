// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
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
        public void ResponseWithReferencedHeaderShouldReferenceComponent()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "responseWithHeaderReference.yaml"));

            var response = result.OpenApiDocument.Components.Responses["Test"];

            Assert.Same(response.Headers.First().Value, result.OpenApiDocument.Components.Headers.First().Value);
        }
    }
}
