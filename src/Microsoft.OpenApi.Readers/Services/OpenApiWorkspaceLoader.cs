using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.Services
{
    internal class OpenApiWorkspaceLoader
    {
        private OpenApiWorkspace _workspace;
        private IStreamLoader _streamLoader;
        private OpenApiDiagnostic _diagnostics;
        private OpenApiReaderSettings _readerSettings;

        public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IStreamLoader streamloader, OpenApiReaderSettings readerSettings)
        {
            _workspace = workspace;
            _streamLoader = streamloader;
            _readerSettings = readerSettings;
            _readerSettings.ReferenceResolution = ReferenceResolutionSetting.DoNotResolveReferences;
        }

        internal async Task LoadAsync(OpenApiReference reference, OpenApiDocument document)
        {
            _workspace.AddDocument(reference.ExternalResource, document);
            document.Workspace = _workspace;

            // Collect remote references by walking document
            var referenceCollector = new OpenApiRemoteReferenceCollector(document);
            var collectorWalker = new OpenApiWalker(referenceCollector);
            collectorWalker.Walk(document);

            // Walk references
            foreach (var item in referenceCollector.References)
            {
                // If not already in workspace, load it and process references
                if (!_workspace.Contains(item.ExternalResource))
                {
                    var stream = await _streamLoader.LoadAsync(new Uri(item.ExternalResource));
                    var reader = new OpenApiStreamReader(_readerSettings);
                    var newDocument = reader.Read(stream, out _diagnostics);
                    await LoadAsync(item, newDocument);
                }
            }
        }
    }
}
