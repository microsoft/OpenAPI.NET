// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests
{
    [Collection("DefaultSettings")]
    public class OpenApiWorkspaceLoaderTests
    {
        public OpenApiWorkspaceLoaderTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        [Fact]
        public async Task OpenApiWorkSpaceLoaderShouldCollectExternalJsonSchemaReferences()
        {
            // Create a reader that will resolve all references
            var settings = new OpenApiReaderSettings
            {
                LoadExternalRefs = true,
                CustomExternalLoader = new ResourceLoader()
            };

            ReadResult result;
            result = await OpenApiDocument.LoadAsync("OpenApiReaderTests/Samples/OpenApiWorkSpaceLoader/TodoMain.yaml", settings);
            Assert.NotNull(result);

            Uri docId = result.OpenApiDocument.Workspace.GetDocumentId("./TodoReference.yaml");
                        
            Assert.True(result.OpenApiDocument.Workspace.Contains(docId + "/components/schemas/todo"));
            Assert.True(result.OpenApiDocument.Workspace.Contains(docId + "/components/schemas/entity"));
        }
    }
}
