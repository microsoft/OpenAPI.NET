using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Reader.Services
{
    internal class OpenApiWorkspaceLoader
    {
        private readonly OpenApiWorkspace _workspace;
        private readonly IStreamLoader _loader;
        private readonly OpenApiReaderSettings _readerSettings;

        public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IStreamLoader loader, OpenApiReaderSettings readerSettings)
        {
            _workspace = workspace;
            _loader = loader;
            _readerSettings = readerSettings;
        }

        internal async Task<OpenApiDiagnostic> LoadAsync(OpenApiReference reference,
                                                         OpenApiDocument document,
                                                         OpenApiDiagnostic diagnostic = null,
                                                         CancellationToken cancellationToken = default)
        {
            _workspace.AddDocumentId(reference.ExternalResource, document.BaseUri);
            var version = diagnostic?.SpecificationVersion ?? OpenApiSpecVersion.OpenApi3_0;
            _workspace.RegisterComponents(document);
            document.Workspace = _workspace;

            // Collect remote references by walking document
            var referenceCollector = new OpenApiRemoteReferenceCollector();
            var collectorWalker = new OpenApiWalker(referenceCollector);
            collectorWalker.Walk(document);

            diagnostic ??= new() { SpecificationVersion = version };

            // Walk references
            foreach (var item in referenceCollector.References)
            {

                // If not already in workspace, load it and process references
                if (!_workspace.Contains(item.ExternalResource))
                {
                    var input = await _loader.LoadAsync(new(item.ExternalResource, UriKind.RelativeOrAbsolute));
                    var result = await OpenApiDocument.LoadAsync(input, _readerSettings, cancellationToken);
                    // Merge diagnostics
                    if (result.OpenApiDiagnostic != null)
                    {
                        diagnostic.AppendDiagnostic(result.OpenApiDiagnostic, item.ExternalResource);
                    }
                    if (result.OpenApiDocument != null)
                    {
                        var loadDiagnostic = await LoadAsync(item, result.OpenApiDocument, diagnostic, cancellationToken);
                        diagnostic = loadDiagnostic;
                    }
                }
            }

            return diagnostic;
        }
    }
}
