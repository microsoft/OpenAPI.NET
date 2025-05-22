﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.OpenApi.Models;
using Xunit;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        [Fact]
        public async Task DetectedSpecificationVersionShouldBeV2_0()
        {
            var actual = await OpenApiDocument.LoadAsync("V2Tests/Samples/basic.v2.yaml", SettingsFixture.ReaderSettings);

            Assert.NotNull(actual.Diagnostic);
            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, actual.Diagnostic.SpecificationVersion);
        }

        [Fact]
        public async Task DetectedSpecificationVersionShouldBeV3_0()
        {
            var actual = await OpenApiDocument.LoadAsync("V3Tests/Samples/OpenApiDocument/minimalDocument.yaml", SettingsFixture.ReaderSettings);

            Assert.NotNull(actual.Diagnostic);
            Assert.Equal(OpenApiSpecVersion.OpenApi3_0, actual.Diagnostic.SpecificationVersion);
        }

        [Fact]
        public async Task DiagnosticReportMergedForExternalReferenceAsync()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new ResourceLoader(),
                BaseUrl = new("fie://c:\\")
            };
            settings.AddYamlReader();

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("OpenApiReaderTests/Samples/OpenApiDiagnosticReportMerged/TodoMain.yaml", settings);

            Assert.NotNull(result);
            Assert.NotNull(result.Document.Workspace);
            Assert.Empty(result.Diagnostic.Errors);
        }
    }

    public class ResourceLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            var path = new Uri(new("http://example.org/OpenApiReaderTests/Samples/OpenApiDiagnosticReportMerged/"), uri).AbsolutePath;
            path = path[1..]; // remove leading slash
            return Task.FromResult(Resources.GetStream(path));
        }
    }
}
