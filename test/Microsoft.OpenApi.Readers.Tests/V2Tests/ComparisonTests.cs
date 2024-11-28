// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
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
        public async Task EquivalentV2AndV3DocumentsShouldProduceEquivalentObjects(string fileName)
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            using var streamV2 = Resources.GetStream(Path.Combine(SampleFolderPath, $"{fileName}.v2.yaml"));
            using var streamV3 = Resources.GetStream(Path.Combine(SampleFolderPath, $"{fileName}.v3.yaml"));
            var result1 = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, $"{fileName}.v2.yaml"));
            var result2 = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, $"{fileName}.v3.yaml"));

            result2.OpenApiDocument.Should().BeEquivalentTo(result1.OpenApiDocument,
                options => options.Excluding(x => x.Workspace).Excluding(y => y.BaseUri));

            result1.OpenApiDiagnostic.Errors.Should().BeEquivalentTo(result2.OpenApiDiagnostic.Errors);
        }
    }
}
