﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    public class OpenApiDocumentLoadTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDocument/";

        public OpenApiDocumentLoadTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public async Task StreamShouldCloseIfLeaveStreamOpenSettingEqualsFalse()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var settings = new OpenApiReaderSettings { LeaveStreamOpen = false };
            _ = await OpenApiDocument.LoadAsync(stream, settings);
            Assert.False(stream.CanRead);
        }

        [Fact]
        public async Task StreamShouldNotCloseIfLeaveStreamOpenSettingEqualsTrue()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "petStore.yaml"));
            var settings = new OpenApiReaderSettings { LeaveStreamOpen = true };
            _ = await OpenApiDocument.LoadAsync(stream, settings);
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

            var result = await OpenApiDocument.LoadAsync(stream, new OpenApiReaderSettings { LeaveStreamOpen = true });
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
            var result = await OpenApiDocument.LoadAsync(stream);
            Assert.NotNull(result.OpenApiDocument);
        }
    }
}
