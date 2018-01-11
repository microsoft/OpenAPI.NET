// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

        [Fact]
        public void DetectedSpecificationVersionShouldBeV2_0()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basic.v2.yaml" )))
            {
                new OpenApiStreamReader().Read(stream, out var diagnostic);

                diagnostic.Should().NotBeNull();
                diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi2_0);
            }
        }
    }
}