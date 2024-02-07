// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Reader for parsing YAML files into an OpenAPI document.
    /// </summary>
    public class OpenApiYamlReader : IOpenApiReader
    {
        private static readonly HttpClient _httpClient = HttpClientFactory.GetHttpClient();

        /// <inheritdoc/>
        public OpenApiDocument Parse(string input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            using var reader = new StringReader(input);
            return Read(reader, out diagnostic, settings);
        }

        /// <inheritdoc/>
        public OpenApiDocument Read(string url, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            Stream stream;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    stream = _httpClient.GetStreamAsync(new Uri(url)).GetAwaiter().GetResult();
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException($"Could not download the file at {url}", ex);
                }
            }
            else
            {
                try
                {
                    var fileInput = new FileInfo(url);
                    stream = fileInput.OpenRead();
                }
                catch (Exception ex) when (
                    ex is
                        FileNotFoundException or
                        PathTooLongException or
                        DirectoryNotFoundException or
                        IOException or
                        UnauthorizedAccessException or
                        SecurityException or
                        NotSupportedException)
                {
                    throw new InvalidOperationException($"Could not open the file at {url}", ex);
                }
            }

            return Read(stream, out diagnostic, settings);
        }

        /// <inheritdoc/>        
        public OpenApiDocument Read(Stream stream, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiStreamReader(settings).Read(stream, out diagnostic);
        }

        /// <inheritdoc/>
        public OpenApiDocument Read(TextReader input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiTextReaderReader(settings).Read(input, out diagnostic);
        }

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(string url, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            Stream stream;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    stream = await _httpClient.GetStreamAsync(new Uri(url));
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException($"Could not download the file at {url}", ex);
                }
            }
            else
            {
                try
                {
                    var fileInput = new FileInfo(url);
                    stream = fileInput.OpenRead();
                }
                catch (Exception ex) when (
                    ex is
                        FileNotFoundException or
                        PathTooLongException or
                        DirectoryNotFoundException or
                        IOException or
                        UnauthorizedAccessException or
                        SecurityException or
                        NotSupportedException)
                {
                    throw new InvalidOperationException($"Could not open the file at {url}", ex);
                }
            }

            return await ReadAsync(stream, settings, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(Stream stream, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            return await new OpenApiStreamReader(settings).ReadAsync(stream, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(TextReader input,
                                                OpenApiReaderSettings settings = null,
                                                CancellationToken cancellationToken = default)
        {
            return await new OpenApiTextReaderReader(settings).ReadAsync(input, cancellationToken);
        }
    }
}
