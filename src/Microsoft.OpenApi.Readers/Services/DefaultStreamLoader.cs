// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers.Services
{
    /// <summary>
    /// Implementation of IInputLoader that loads streams from URIs
    /// </summary>
    internal class DefaultStreamLoader : IStreamLoader
    {
        private readonly Uri baseUrl;
        private HttpClient _httpClient = new();

        public DefaultStreamLoader(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Stream Load(Uri uri)
        {
            var absoluteUri = new Uri(baseUrl, uri);
            switch (uri.Scheme)
            {
                case "file":
                    return File.OpenRead(absoluteUri.AbsolutePath);
                case "http":
                case "https":
                    return _httpClient.GetStreamAsync(absoluteUri).GetAwaiter().GetResult();
                default:
                    throw new ArgumentException("Unsupported scheme");
            }
        }

        public async Task<Stream> LoadAsync(Uri uri)
        {
            var absoluteUri = new Uri(baseUrl, uri);

            switch (absoluteUri.Scheme)
            {
                case "file":
                    return File.OpenRead(absoluteUri.AbsolutePath);
                case "http":
                case "https":
                    return await _httpClient.GetStreamAsync(absoluteUri);
                default:
                    throw new ArgumentException("Unsupported scheme");
            }
        }
    }
}
