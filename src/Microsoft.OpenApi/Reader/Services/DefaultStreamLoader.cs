// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader.Services
{
    /// <summary>
    /// Implementation of IInputLoader that loads streams from URIs
    /// </summary>
    public class DefaultStreamLoader : IStreamLoader
    {
        private readonly Uri baseUrl;
        private readonly HttpClient _httpClient = new();

        /// <summary>
        /// The default stream loader
        /// </summary>
        /// <param name="baseUrl"></param>
        public DefaultStreamLoader(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        /// <inheritdoc/>
        public async Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Uri absoluteUri;
            absoluteUri = baseUrl.AbsoluteUri.Equals(OpenApiConstants.BaseRegistryUri) ? new Uri(Directory.GetCurrentDirectory() + uri) 
                : new Uri(baseUrl, uri);

            switch (absoluteUri.Scheme)
            {
                case "file":
                    return File.OpenRead(absoluteUri.AbsolutePath);
                case "http":
                case "https":
#if NET5_0_OR_GREATER
                    return await _httpClient.GetStreamAsync(absoluteUri, cancellationToken).ConfigureAwait(false);
#else
                    return await _httpClient.GetStreamAsync(absoluteUri).ConfigureAwait(false);
#endif
                default:
                    throw new ArgumentException("Unsupported scheme");
            }
        }
    }
}
