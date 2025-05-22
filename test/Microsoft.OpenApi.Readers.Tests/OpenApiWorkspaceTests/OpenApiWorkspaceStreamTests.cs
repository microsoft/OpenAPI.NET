using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiWorkspaceTests
{
    public class OpenApiWorkspaceStreamTests
    {
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
            settings.AddYamlReader();

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
            settings.AddYamlReader();

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("V3Tests/Samples/OpenApiWorkspace/TodoMain.yaml", settings);

            var externalDocBaseUri = result.Document.Workspace.GetDocumentId("./TodoComponents.yaml");
            var schemasPath = "#/components/schemas/";
            var parametersPath = "#/components/parameters/";

            Assert.NotNull(externalDocBaseUri);
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "todo"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + schemasPath + "entity"));
            Assert.True(result.Document.Workspace.Contains(externalDocBaseUri + parametersPath + "filter"));
        }

        [Fact]
        public async Task LoadDocumentWithExternalReferencesInSubDirectories()
        {
            var sampleFolderPath = $"V3Tests/Samples/OpenApiWorkspace/ExternalReferencesInSubDirectories";
            var referenceBaseUri = "file://" + Path.GetFullPath(sampleFolderPath);

            // Create a reader that will resolve all references also of documents located in the non-root directory
            var settings = new OpenApiReaderSettings()
            {
                LoadExternalRefs = true,
            };
            settings.AddYamlReader();

            // Act
            var result = await OpenApiDocument.LoadAsync($"{sampleFolderPath}/Root.yaml", settings);
            var document = result.Document;
            var workspace = result.Document.Workspace;

            // Assert
            Assert.True(workspace.Contains($"{Path.Combine(referenceBaseUri, "Directory", "PetsPage.yaml")}#/components/schemas/PetsPage"));
            Assert.True(workspace.Contains($"{Path.Combine(referenceBaseUri, "Directory", "Pets.yaml")}#/components/schemas/Pets"));
            Assert.True(workspace.Contains($"{Path.Combine(referenceBaseUri, "Directory", "Pets.yaml")}#/components/schemas/Pet"));

            var operationResponseSchema = document.Paths["/pets"].Operations[HttpMethod.Get].Responses["200"].Content["application/json"].Schema;
            Assert.IsType<OpenApiSchemaReference>(operationResponseSchema);
            
            var petsSchema = operationResponseSchema.Properties["pets"];
            Assert.IsType<OpenApiSchemaReference>(petsSchema);
            Assert.Equal(JsonSchemaType.Array, petsSchema.Type);
                        
            var petSchema = petsSchema.Items;
            var petSchemaReference = Assert.IsType<OpenApiSchemaReference>(petSchema);
            var petSchemaTarget = petSchemaReference.RecursiveTarget;
            Assert.NotNull(petSchemaTarget);

            Assert.Equivalent(new OpenApiSchema
            {
                Required = new HashSet<string> { "id", "name" },
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["id"] = new OpenApiSchema
                    {
                        Type =  JsonSchemaType.Integer,
                        Format = "int64"
                    },
                    ["name"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    },
                    ["tag"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            }, petSchemaTarget);
        }
    }

    public class MockLoader : IStreamLoader
    {
        public Stream Load(Uri uri)
        {
            return null;
        }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
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

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            var path = new Uri(new("http://example.org/V3Tests/Samples/OpenApiWorkspace/"), uri).AbsolutePath;
            path = path[1..]; // remove leading slash
            return Task.FromResult(Resources.GetStream(path));
        }
    }
}
