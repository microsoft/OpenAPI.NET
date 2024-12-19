using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiWorkspaceTests
{
    public class OpenApiWorkspaceStreamTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiWorkspace/";
        
        public OpenApiWorkspaceStreamTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        // Use OpenApiWorkspace to load a document and a referenced document

        [Fact]
        public async Task LoadingDocumentWithResolveAllReferencesShouldLoadDocumentIntoWorkspaceAsync()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new MockLoader(),
                BaseUrl = new("file://c:\\")
            };

            // Todo: this should be ReadAsync
            var stream = new MemoryStream();
            var doc = """
                      openapi: 3.0.0
                      info:
                        title: foo
                        version: 1.0.0
                      paths: {}
                      """;
            var wr = new StreamWriter(stream);
            await wr.WriteAsync(doc);
            await wr.FlushAsync();
            stream.Position = 0;

            var result = await OpenApiDocument.LoadAsync(stream, OpenApiConstants.Yaml, settings: settings);

            Assert.NotNull(result.Document.Workspace);
        }

        [Fact]
        public async Task LoadDocumentWithExternalReferenceShouldLoadBothDocumentsIntoWorkspaceAsync()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new ResourceLoader(),
                BaseUrl = new("file://c:\\"),
            };

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("V3Tests/Samples/OpenApiWorkspace/TodoMain.yaml", settings);

            var externalDocBaseUri = result.Document.Workspace.GetDocumentId("./TodoComponents.yaml");
            var schemasPath = "/components/schemas/";
            var parametersPath = "/components/parameters/";

            Assert.NotNull(externalDocBaseUri);
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "todo"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "entity"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + parametersPath + "filter"));
        }
    }

    public class MockLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri uri)
        {
            return Task.FromResult<Stream>(null);
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
            var path = new Uri(new("http://example.org/V3Tests/Samples/OpenApiWorkspace/"), uri).AbsolutePath;
            path = path[1..]; // remove leading slash
            return Task.FromResult(Resources.GetStream(path));
        }
    }
}
