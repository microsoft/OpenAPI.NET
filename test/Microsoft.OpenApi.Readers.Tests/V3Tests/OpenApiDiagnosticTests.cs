// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        [Fact]
        public void DetectedSpecificationVersionShouldBeV3_0()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minimalDocument.yaml")))
            {
                new OpenApiStreamReader().Read(stream, out var diagnostic);

                diagnostic.Should().NotBeNull();
                diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi3_0);
            }
        }
    }
}