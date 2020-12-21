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

        //[Fact]
        public void LoadDocumentIntoWorkspace()
        {
            // Create a reader that will resolve all references
            var reader = new OpenApiStringReader(new OpenApiReaderSettings() {
                ReferenceResolution = ReferenceResolutionSetting.ResolveAllReferences, 
                CustomExternalLoader = (url) => { return null; }
            });

            // Todo: this should be ReadAsync
            var doc = reader.Read("", out OpenApiDiagnostic diagnostic);

            Assert.NotNull(doc.Workspace);

        }
    }
}
