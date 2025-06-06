using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader
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

        internal async Task<OpenApiDiagnostic> LoadAsync(BaseOpenApiReference reference,
                                                         OpenApiDocument? document,
                                                         string? format = null,
                                                         OpenApiDiagnostic? diagnostic = null,
                                                         CancellationToken cancellationToken = default)
        {
            _workspace.AddDocumentId(reference.ExternalResource, document?.BaseUri);
            var version = diagnostic?.SpecificationVersion ?? OpenApiSpecVersion.OpenApi3_0;
            if (document is not null)
            {
                _workspace.RegisterComponents(document);
                document.Workspace = _workspace;
            }
            
            // Collect remote references by walking document
            var referenceCollector = new OpenApiRemoteReferenceCollector();
            var collectorWalker = new OpenApiWalker(referenceCollector);
            collectorWalker.Walk(document);

            diagnostic ??= new() { SpecificationVersion = version };

            // Walk references
            foreach (var item in referenceCollector.References)
            {

                // If not already in workspace, load it and process references
                if (item.ExternalResource is not null && !_workspace.Contains(item.ExternalResource))
                {
                    var uri = new Uri(item.ExternalResource, UriKind.RelativeOrAbsolute);
                    var input = await _loader.LoadAsync(item.HostDocument!.BaseUri, uri, cancellationToken).ConfigureAwait(false);
                    var result = await OpenApiDocument.LoadAsync(input, format, _readerSettings, cancellationToken).ConfigureAwait(false);
                    // Merge diagnostics
                    if (result.Diagnostic != null)
                    {
                        diagnostic.AppendDiagnostic(result.Diagnostic, item.ExternalResource);
                    }
                    if (result.Document != null)
                    {
                        var loadDiagnostic = await LoadAsync(item, result.Document, format, diagnostic, cancellationToken).ConfigureAwait(false);
                        diagnostic = loadDiagnostic;
                    }
                }
            }

            return diagnostic;
        }
    }
}
