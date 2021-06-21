// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    public class OpenApiStreamReaderTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        [Fact]
        public void StreamShouldCloseIfLeaveStreamOpenSettingEqualsFalse()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml")))
            {
                var reader = new OpenApiStreamReader(new OpenApiReaderSettings { LeaveStreamOpen = false });
                reader.Read(stream, out _);
                Assert.False(stream.CanRead);
            }
        }

        [Fact]
        public void StreamShouldNotCloseIfLeaveStreamOpenSettingEqualsTrue()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml")))
            {
                var reader = new OpenApiStreamReader(new OpenApiReaderSettings { LeaveStreamOpen = true});
                reader.Read(stream, out _);
                Assert.True(stream.CanRead);
            }
        }
    }
}
