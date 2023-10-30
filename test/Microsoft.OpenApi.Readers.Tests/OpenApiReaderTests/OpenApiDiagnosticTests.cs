// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;
using Microsoft.OpenApi.Readers.Interface;
using System.IO;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        [Fact]
        public void DetectedSpecificationVersionShouldBeV2_0()
        {
            using var stream = Resources.GetStream("V2Tests/Samples/basic.v2.yaml");
            new OpenApiStreamReader().Read(stream, out var diagnostic);

            diagnostic.Should().NotBeNull();
            diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi2_0);
        }

        [Fact]
        public void DetectedSpecificationVersionShouldBeV3_0()
        {
            using var stream = Resources.GetStream("V3Tests/Samples/OpenApiDocument/minimalDocument.yaml");
            new OpenApiStreamReader().Read(stream, out var diagnostic);

            diagnostic.Should().NotBeNull();
            diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi3_0);
        }

        [Fact]
        public async Task DiagnosticReportMergedForExternalReference()
        {
            // Create a reader that will resolve all references
            var reader = new OpenApiStreamReader(new()
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new ResourceLoader(),
                BaseUrl = new("fie://c:\\")
            });

            ReadResult result;
            using (var stream = Resources.GetStream("OpenApiReaderTests/Samples/OpenApiDiagnosticReportMerged/TodoMain.yaml"))
            {
                result = await reader.ReadAsync(stream);
            }

            Assert.NotNull(result);
            Assert.NotNull(result.OpenApiDocument.Workspace);
            Assert.True(result.OpenApiDocument.Workspace.Contains("TodoReference.yaml"));
            result.OpenApiDiagnostic.Errors.Should().BeEquivalentTo(new List<OpenApiError> {
                new( new OpenApiException("[File: ./TodoReference.yaml] Invalid Reference identifier 'object-not-existing'.")) });
        }
    }

    public class ResourceLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri uri)
        {
            var path = new Uri(new("http://example.org/OpenApiReaderTests/Samples/OpenApiDiagnosticReportMerged/"), uri).AbsolutePath;
            path = path[1..]; // remove leading slash
            return Task.FromResult(Resources.GetStream(path));
        }
    }
}
