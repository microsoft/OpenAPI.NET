﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
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
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The default stream loader
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="httpClient">The HttpClient to use to retrieve documents when needed</param>
        public DefaultStreamLoader(Uri baseUrl, HttpClient httpClient)
        {
            this.baseUrl = baseUrl;
            _httpClient = Utils.CheckArgumentNull(httpClient);
        }

        /// <inheritdoc/>
        public async Task<Stream> LoadAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            var absoluteUri = (baseUrl.AbsoluteUri.Equals(OpenApiConstants.BaseRegistryUri), baseUrl.IsAbsoluteUri, uri.IsAbsoluteUri) switch
            {
                (true, _, _) => new Uri(Path.Combine(Directory.GetCurrentDirectory(), uri.ToString())),
                // this overcomes a URI concatenation issue for local paths on linux OSes
                (_, true, false) when baseUrl.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase) && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) =>
                    new Uri(Path.Combine(baseUrl.AbsoluteUri, uri.ToString())),
                (_, _, _) => new Uri(baseUrl, uri),
            };

            return absoluteUri.Scheme switch
            {
                "file" => File.OpenRead(absoluteUri.AbsolutePath),
                "http" or "https" =>
#if NET5_0_OR_GREATER
                    await _httpClient.GetStreamAsync(absoluteUri, cancellationToken).ConfigureAwait(false),
#else
                    await _httpClient.GetStreamAsync(absoluteUri).ConfigureAwait(false),
#endif
                _ => throw new ArgumentException("Unsupported scheme"),
            };
        }
    }
}
