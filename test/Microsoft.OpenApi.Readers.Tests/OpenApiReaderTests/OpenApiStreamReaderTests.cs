// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    public class OpenApiStreamReaderTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        [Fact]
        public void StreamShouldCloseIfLeaveStreamOpenSettingEqualsFalse()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var reader = new OpenApiStreamReader(new() { LeaveStreamOpen = false });
            reader.Read(stream, out _);
            Assert.False(stream.CanRead);
        }

        [Fact]
        public void StreamShouldNotCloseIfLeaveStreamOpenSettingEqualsTrue()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var reader = new OpenApiStreamReader(new() { LeaveStreamOpen = true});
            reader.Read(stream, out _);
            Assert.True(stream.CanRead);
        }

        [Fact]
        public async Task StreamShouldNotBeDisposedIfLeaveStreamOpenSettingIsTrue()
        {
            var memoryStream = new MemoryStream();
            using var fileStream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));

            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var stream = memoryStream;

            var reader = new OpenApiStreamReader(new() { LeaveStreamOpen = true });
            _ = await reader.ReadAsync(stream);
            stream.Seek(0, SeekOrigin.Begin); // does not throw an object disposed exception
            Assert.True(stream.CanRead);
        }
    }
}
