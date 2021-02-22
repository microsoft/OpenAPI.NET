using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiWorkspaceTests
{
    public class OpenApiWorkspaceStreamTests
    {


        // Use OpenApiWorkspace to load a document and a referenced document

        [Fact]
        public async Task LoadDocumentIntoWorkspace()
        {
            // Create a reader that will resolve all references
            var reader = new OpenApiStreamReader(new OpenApiReaderSettings()
            {
                ReferenceResolution = ReferenceResolutionSetting.ResolveAllReferences,
                CustomExternalLoader = new MockLoader()
            });

            // Todo: this should be ReadAsync
            var stream = new MemoryStream();
            var doc = @"openapi: 3.0.0
info:
  title: foo
  version: 1.0.0
paths: {}";
            var wr = new StreamWriter(stream);
            wr.Write(doc);
            wr.Flush();
            stream.Position = 0;

            var result = await reader.ReadAsync(stream);

            Assert.NotNull(result.OpenApiDocument.Workspace);

        }
    }

    public class MockLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public async Task<Stream> LoadAsync(Uri uri)
        {
            return null;
        }
    }
}
