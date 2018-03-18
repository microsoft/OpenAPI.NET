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
    internal class OpenApiWorkspaceLoader<TInput,TDiagnostic> where TDiagnostic: IDiagnostic
    {
        private OpenApiWorkspace _workspace;
        private IInputLoader<TInput> _loader;
        private TDiagnostic _diagnostics;
        private IOpenApiReader<TInput, TDiagnostic> _reader;

        public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IInputLoader<TInput> loader, IOpenApiReader<TInput, TDiagnostic> reader)
        {
            _workspace = workspace;
            _loader = loader;
            _reader = reader;
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
                    var input = await _loader.LoadAsync(new Uri(item.ExternalResource));
                    var newDocument = _reader.Read(input, out _diagnostics);
                    await LoadAsync(item, newDocument);
                }
            }
        }
    }
}
