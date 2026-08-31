// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
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
            var settings = new OpenApiReaderSettings();
            settings.AddYamlReader();
            using var streamV2 = Resources.GetStream(Path.Join(SampleFolderPath, $"{fileName}.v2.yaml"));
            using var streamV3 = Resources.GetStream(Path.Join(SampleFolderPath, $"{fileName}.v3.yaml"));
            var result1 = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, $"{fileName}.v2.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var result2 = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, $"{fileName}.v3.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            OpenApiTestAssert.Equivalent(result1.Document, result2.Document, "Workspace", "BaseUri");

            Assert.Equivalent(result2.Diagnostic.Errors, result1.Diagnostic.Errors);
        }
    }
}
