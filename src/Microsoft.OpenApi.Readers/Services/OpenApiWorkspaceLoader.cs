using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Services;
using SharpYaml.Model;

namespace Microsoft.OpenApi.Readers.Services
{
    internal class OpenApiWorkspaceLoader 
    {
        private OpenApiWorkspace _workspace;
        private Func<Uri, Task<Stream>> _loader;
        private OpenApiDiagnostic _diagnostics;
        private OpenApiReaderSettings _readerSettings;

        public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, Func<Uri,Task<Stream>> loader, OpenApiReaderSettings readerSettings)
        {
            _workspace = workspace;
            _loader = loader;
            _readerSettings = readerSettings;
        }

        internal async Task LoadAsync(OpenApiReference reference, OpenApiDocument document)
        {
            _workspace.AddDocument(reference.ExternalResource, document);
            document.Workspace = _workspace;

            // Collect remote references by walking document
            var referenceCollector = new OpenApiRemoteReferenceCollector(document);
            var collectorWalker = new OpenApiWalker(referenceCollector);
            collectorWalker.Walk(document);

            var reader = new OpenApiStreamReader(_readerSettings);

            // Walk references
            foreach (var item in referenceCollector.References)
            {
                // If not already in workspace, load it and process references
                if (!_workspace.Contains(item.ExternalResource))
                {
                    var input = await _loader(new Uri(item.ExternalResource));
                    var newDocument = reader.Read(input, out _diagnostics); // TODO merge _diagnositics
                    await LoadAsync(item, newDocument);
                }
            }
        }
    }
}
