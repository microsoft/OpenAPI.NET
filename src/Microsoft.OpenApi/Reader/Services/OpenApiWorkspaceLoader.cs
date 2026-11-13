using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader
{
    internal class OpenApiWorkspaceLoader
    {
        private readonly OpenApiWorkspace _workspace;
        private readonly IStreamLoader _loader;
        private readonly OpenApiReaderSettings _readerSettings;
        private readonly int _maxDepth;

        public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IStreamLoader loader, OpenApiReaderSettings readerSettings, int maxDepth)
        {
            _workspace = workspace;
            _loader = loader;
            _readerSettings = readerSettings;
            _maxDepth = maxDepth;
        }

        internal async Task<bool> LoadAsync(OpenApiDocument document, string format, OpenApiDiagnostic diagnostic, Uri? sourceUri, CancellationToken cancellationToken)
        {
            var documents = new Dictionary<string, OpenApiDocument?>(StringComparer.Ordinal);
            if (sourceUri is not null)
            {
                var location = DefaultStreamLoader.ResolveUri(sourceUri, sourceUri).AbsoluteUri;
                documents[location] = document;
                _workspace.AddDocumentId(location, document.BaseUri);
            }
            var references = new List<IOpenApiReferenceHolder>();
            var pending = new Stack<(OpenApiDocument Document, int Depth, IEnumerator<BaseOpenApiReference> References)>();
            RegisterDocument(document, 0, references, pending);
            try
            {
                while (pending.Count > 0)
                {
                    if (!await LoadNextReferenceAsync(pending, documents, references, format, diagnostic, cancellationToken).ConfigureAwait(false))
                    {
                        return false;
                    }
                }

                return ValidateReferenceChains(references, documents, diagnostic, cancellationToken);
            }
            finally
            {
                foreach (var frame in pending)
                {
                    frame.References.Dispose();
                }
            }
        }

        private async Task<bool> LoadNextReferenceAsync(
            Stack<(OpenApiDocument Document, int Depth, IEnumerator<BaseOpenApiReference> References)> pending,
            Dictionary<string, OpenApiDocument?> documents,
            List<IOpenApiReferenceHolder> references,
            string format,
            OpenApiDiagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var frame = pending.Peek();
            if (!frame.References.MoveNext())
            {
                frame.References.Dispose();
                pending.Pop();
                return true;
            }

            var reference = frame.References.Current;
            var resource = reference.ExternalResource!;
            var uri = DefaultStreamLoader.ResolveUri(frame.Document.BaseUri, new Uri(resource, UriKind.RelativeOrAbsolute));
            if (_workspace.Contains(uri.AbsoluteUri))
            {
                _workspace.AddDocumentId(resource, uri);
                _workspace.AddDocumentId(uri.AbsoluteUri, uri);
                return true;
            }
            if (!documents.TryGetValue(uri.AbsoluteUri, out var loadedDocument))
            {
                if (frame.Depth >= _maxDepth)
                {
                    diagnostic.Errors.Add(new(reference.ReferenceV3,
                        $"External reference depth limit {_maxDepth} exceeded while loading '{uri}' from '{frame.Document.BaseUri}'."));
                    return false;
                }

                using var input = await _loader.LoadAsync(frame.Document.BaseUri, new Uri(resource, UriKind.RelativeOrAbsolute), cancellationToken).ConfigureAwait(false);
                var result = await OpenApiModelFactory.ReadDocumentAsync(input, format, uri, _readerSettings, cancellationToken).ConfigureAwait(false);
                if (result.Diagnostic is { } externalDiagnostic)
                {
                    diagnostic.AppendDiagnostic(externalDiagnostic, uri.AbsoluteUri);
                }
                loadedDocument = result.Document;
                documents.Add(uri.AbsoluteUri, loadedDocument);
                if (loadedDocument is not null)
                {
                    RegisterDocument(loadedDocument, frame.Depth + 1, references, pending);
                }
            }

            _workspace.AddDocumentId(resource, loadedDocument?.BaseUri);
            _workspace.AddDocumentId(uri.AbsoluteUri, loadedDocument?.BaseUri);
            return true;
        }

        private void RegisterDocument(
            OpenApiDocument loaded,
            int depth,
            List<IOpenApiReferenceHolder> references,
            Stack<(OpenApiDocument Document, int Depth, IEnumerator<BaseOpenApiReference> References)> pending)
        {
            loaded.Workspace = _workspace;
            _workspace.RegisterComponents(loaded);
            var collector = new OpenApiRemoteReferenceCollector();
            new OpenApiWalker(collector).Walk(loaded);
            references.AddRange(collector.ReferenceHolders);
            pending.Push((loaded, depth, collector.References.GetEnumerator()));
        }

        private static bool ValidateReferenceChains(List<IOpenApiReferenceHolder> references, Dictionary<string, OpenApiDocument?> documents, OpenApiDiagnostic diagnostic, CancellationToken cancellationToken)
        {
            var completed = new HashSet<IOpenApiReferenceHolder>(ReferenceIdentityComparer.Instance);
            foreach (var start in references)
            {
                if (!ValidateReferenceChain(start, completed, documents, diagnostic, cancellationToken))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateReferenceChain(
            IOpenApiReferenceHolder start,
            HashSet<IOpenApiReferenceHolder> completed,
            Dictionary<string, OpenApiDocument?> documents,
            OpenApiDiagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var path = new Dictionary<IOpenApiReferenceHolder, int>(ReferenceIdentityComparer.Instance);
            var chain = new List<BaseOpenApiReference>();
            IOpenApiReferenceHolder? current = start;
            while (current is not null && !completed.Contains(current))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var reference = OpenApiRemoteReferenceCollector.GetReference(current);
                if (current is OpenApiSchemaReference schema && !schema.Reference.IsReferenceOnly)
                {
                    break;
                }
                if (reference.IsExternal && reference.HostDocument is { } host)
                {
                    var location = DefaultStreamLoader.ResolveUri(host.BaseUri, new Uri(reference.ExternalResource!, UriKind.RelativeOrAbsolute));
                    if (!documents.TryGetValue(location.AbsoluteUri, out var loaded) || loaded is null)
                    {
                        break;
                    }
                }
                if (path.TryGetValue(current, out var cycleStart))
                {
                    if (chain.Skip(cycleStart).Any(static item => item.IsExternal))
                    {
                        diagnostic.Errors.Add(new(reference.ReferenceV3,
                            $"Circular external reference-only chain detected: {string.Join(" -> ", chain.Select(static item => $"{item.HostDocument?.BaseUri}: {item.ReferenceV3}"))}."));
                        return false;
                    }
                    break;
                }

                path.Add(current, chain.Count);
                chain.Add(reference);
                IOpenApiReferenceable? target;
                try
                {
                    target = current is OpenApiTagReference tag
                        ? tag.Target
                        : reference.HostDocument?.ResolveReference(reference, reference.IsExternal, null);
                    if (target is null && current is IOpenApiSchema parentSchema)
                    {
                        target = reference.HostDocument?.ResolveReference(reference, reference.IsExternal, parentSchema);
                    }
                }
                catch (Exception ex) when (ex is InvalidOperationException or IndexOutOfRangeException or ArgumentOutOfRangeException)
                {
                    if (chain.Any(static item => item.IsExternal))
                    {
                        diagnostic.Errors.Add(new(reference.ReferenceV3,
                            $"Could not resolve external reference chain from '{chain[0].HostDocument?.BaseUri}' ({chain[0].ReferenceV3}): {ex.Message}"));
                        return false;
                    }
                    break;
                }
                current = target as IOpenApiReferenceHolder;
            }
            completed.UnionWith(path.Keys);
            return true;
        }

        private sealed class ReferenceIdentityComparer : IEqualityComparer<IOpenApiReferenceHolder>
        {
            public static ReferenceIdentityComparer Instance { get; } = new();
            public bool Equals(IOpenApiReferenceHolder? x, IOpenApiReferenceHolder? y) => ReferenceEquals(x, y);
            public int GetHashCode(IOpenApiReferenceHolder obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
