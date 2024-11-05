// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
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
        private HttpClient _httpClient = new();

        /// <summary>
        /// The default stream loader
        /// </summary>
        /// <param name="baseUrl"></param>
        public DefaultStreamLoader(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }
/// <inheritdoc/>

        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Stream Load(Uri uri)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            return LoadAsync(uri).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        /// <summary>
        /// Use Uri to locate data and convert into an input object.
        /// </summary>
        /// <param name="uri">Identifier of some source of an OpenAPI Description</param>
        /// <returns>A data object that can be processed by a reader to generate an <see cref="OpenApiDocument"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Stream> LoadAsync(Uri uri)
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
                    return await _httpClient.GetStreamAsync(absoluteUri);
                default:
                    throw new ArgumentException("Unsupported scheme");
            }
        }
    }
}
