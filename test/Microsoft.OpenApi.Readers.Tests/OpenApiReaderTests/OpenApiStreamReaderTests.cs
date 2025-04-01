﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
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
        public async Task StreamShouldNotBeDisposedIfLeaveStreamOpenSettingIsTrueAsync()
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

        [Fact]
        public async Task StreamShouldReadWhenInitializedAsync()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://raw.githubusercontent.com/OAI/OpenAPI-Specification/")
            };

            var stream = await httpClient.GetStreamAsync("20fe7a7b720a0e48e5842d002ac418b12a8201df/tests/v3.0/pass/petstore.yaml");

            // Read V3 as YAML
            var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);
            Assert.NotNull(openApiDocument);
        }
    }
}
