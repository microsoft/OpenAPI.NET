// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Text.Json.Nodes;
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
            var stream = GetStream(url).GetAwaiter().GetResult();
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
            var stream = GetStream(url).Result;
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


        /// <summary>
        /// Takes in an input URL and parses it into an Open API document
        /// </summary>
        /// <param name="url">The path to the Open API file</param>
        /// <param name="version">The OpenAPI specification version.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public T Read<T>(string url,
                         OpenApiSpecVersion version,
                         out OpenApiDiagnostic diagnostic,
                         OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            settings ??= new OpenApiReaderSettings();
            var stream = GetStream(url).GetAwaiter().GetResult();
            return Read<T>(stream, version, out diagnostic, settings);
        }

        /// <inheritdoc/>
        public T Read<T>(Stream input,
                         OpenApiSpecVersion version,
                         out OpenApiDiagnostic diagnostic,
                         OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return new OpenApiStreamReader(settings).ReadFragment<T>(input, version, out diagnostic);
        }

        /// <inheritdoc/>
        public T Read<T>(TextReader input,
                         OpenApiSpecVersion version,
                         out OpenApiDiagnostic diagnostic,
                         OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return new OpenApiTextReaderReader(settings).ReadFragment<T>(input, version, out diagnostic);
        }

        /// <inheritdoc/>
        public T Read<T>(JsonNode input,
                         OpenApiSpecVersion version,
                         out OpenApiDiagnostic diagnostic,
                         OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return new OpenApiYamlDocumentReader(settings).ReadFragment<T>(input, version, out diagnostic);
        }

        /// <summary>
        /// Parses an input string into an Open API document.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public T Parse<T>(string input,
                          OpenApiSpecVersion version,
                          out OpenApiDiagnostic diagnostic,
                          OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            settings ??= new OpenApiReaderSettings();
            using var reader = new StringReader(input);
            return Read<T>(reader, version, out diagnostic, settings);
        }

        private async Task<Stream> GetStream(string url)
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

            return stream;
        } 
    }
}
