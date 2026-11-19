// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Implementation of IInputLoader that loads streams from URIs
    /// </summary>
    public class DefaultStreamLoader : IStreamLoader
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The default stream loader
        /// </summary>
        /// <param name="httpClient">The HttpClient to use to retrieve documents when needed</param>
        public DefaultStreamLoader(HttpClient httpClient)
        {
            _httpClient = Utils.CheckArgumentNull(httpClient);
        }

        /// <inheritdoc/>
        public async Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            var absoluteUri = ResolveUri(baseUrl, uri);

            return absoluteUri.Scheme switch
            {
                "file" => File.OpenRead(absoluteUri.LocalPath),
                "http" or "https" =>
#if NET5_0_OR_GREATER
                    await _httpClient.GetStreamAsync(absoluteUri, cancellationToken).ConfigureAwait(false),
#else
                    await _httpClient.GetStreamAsync(absoluteUri).ConfigureAwait(false),
#endif
                _ => throw new ArgumentException("Unsupported scheme"),
            };
        }

        internal static Uri ResolveUri(Uri baseUrl, Uri uri)
        {
            var resolved = uri.IsAbsoluteUri ? uri :
                baseUrl.AbsoluteUri.Equals(OpenApiConstants.BaseRegistryUri) ?
                    new Uri(new Uri(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar), uri) :
                    new Uri(baseUrl, uri);
            return new UriBuilder(resolved) { Fragment = string.Empty }.Uri;
        }
    }
}
