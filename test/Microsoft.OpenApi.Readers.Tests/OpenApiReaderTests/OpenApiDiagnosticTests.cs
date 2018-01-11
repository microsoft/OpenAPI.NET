// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using FluentAssertions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        [Fact]
        public void DetectedSpecificationVersionShouldBeV2_0()
        {
            using (var stream = Resources.GetStream("V2Tests/Samples/basic.v2.yaml"))
            {
                new OpenApiStreamReader().Read(stream, out var diagnostic);

                diagnostic.Should().NotBeNull();
                diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi2_0);
            }
        }

        [Fact]
        public void DetectedSpecificationVersionShouldBeV3_0()
        {
            using (var stream = Resources.GetStream("V3Tests/Samples/OpenApiDocument/minimalDocument.yaml"))
            {
                new OpenApiStreamReader().Read(stream, out var diagnostic);

                diagnostic.Should().NotBeNull();
                diagnostic.SpecificationVersion.Should().Be( OpenApiSpecVersion.OpenApi3_0);
            }
        }
    }
}