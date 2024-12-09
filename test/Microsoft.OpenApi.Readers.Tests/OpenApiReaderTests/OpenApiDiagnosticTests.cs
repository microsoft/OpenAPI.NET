// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class OpenApiDiagnosticTests
    {
        public OpenApiDiagnosticTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        [Fact]
        public void DetectedSpecificationVersionShouldBeV2_0()
        {
            var actual = OpenApiDocument.Load("V2Tests/Samples/basic.v2.yaml");

            actual.Diagnostic.Should().NotBeNull();
            actual.Diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi2_0);
        }

        [Fact]
        public void DetectedSpecificationVersionShouldBeV3_0()
        {
            var actual = OpenApiDocument.Load("V3Tests/Samples/OpenApiDocument/minimalDocument.yaml");

            actual.Diagnostic.Should().NotBeNull();
            actual.Diagnostic.SpecificationVersion.Should().Be(OpenApiSpecVersion.OpenApi3_0);
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

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("OpenApiReaderTests/Samples/OpenApiDiagnosticReportMerged/TodoMain.yaml", settings);

            Assert.NotNull(result);
            Assert.NotNull(result.Document.Workspace);
            result.Diagnostic.Errors.Should().BeEmpty();
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
