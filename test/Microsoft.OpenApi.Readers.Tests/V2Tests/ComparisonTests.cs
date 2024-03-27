// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class ComparisonTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

        [Theory]
        [InlineData("minimal")]
        [InlineData("basic")]
        //[InlineData("definitions")]  //Currently broken due to V3 references not behaving the same as V2
        public void EquivalentV2AndV3DocumentsShouldProductEquivalentObjects(string fileName)
        {
            using var streamV2 = Resources.GetStream(Path.Combine(SampleFolderPath, $"{fileName}.v2.yaml"));
            using var streamV3 = Resources.GetStream(Path.Combine(SampleFolderPath, $"{fileName}.v3.yaml"));
            var result1 = OpenApiDocument.Load(Path.Combine(SampleFolderPath, $"{fileName}.v2.yaml"));
            var result2 = OpenApiDocument.Load(Path.Combine(SampleFolderPath, $"{fileName}.v3.yaml"));

            result2.OpenApiDocument.Should().BeEquivalentTo(result1.OpenApiDocument);

            result1.OpenApiDiagnostic.Errors.Should().BeEquivalentTo(result2.OpenApiDiagnostic.Errors);
        }
    }
}
