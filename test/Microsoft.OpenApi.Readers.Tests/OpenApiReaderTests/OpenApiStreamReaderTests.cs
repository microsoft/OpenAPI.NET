// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    public class OpenApiStreamReaderTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        public OpenApiStreamReaderTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void StreamShouldCloseIfLeaveStreamOpenSettingEqualsFalse()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var settings = new OpenApiReaderSettings { LeaveStreamOpen = false };
            _ = OpenApiDocument.Load(stream, "yaml", settings);
            Assert.False(stream.CanRead);
        }

        [Fact]
        public void StreamShouldNotCloseIfLeaveStreamOpenSettingEqualsTrue()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var settings = new OpenApiReaderSettings { LeaveStreamOpen = true };
            _ = OpenApiDocument.Load(stream, "yaml", settings);
            Assert.True(stream.CanRead);
        }

        [Fact]
        public async void StreamShouldNotBeDisposedIfLeaveStreamOpenSettingIsTrue()
        {
            var memoryStream = new MemoryStream();
            using var fileStream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));

            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var stream = memoryStream;

            var result = OpenApiDocument.Load(stream, "yaml", new OpenApiReaderSettings { LeaveStreamOpen = true });
            stream.Seek(0, SeekOrigin.Begin); // does not throw an object disposed exception
            Assert.True(stream.CanRead);
        }
    }
}
