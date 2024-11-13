// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// A factory class for loading OpenAPI models from various sources.
    /// </summary>
    public static class OpenApiModelFactory
    {
        private static readonly HttpClient _httpClient = new();

        static OpenApiModelFactory()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Json, new OpenApiJsonReader());
        }

        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            var format = await GetFormatAsync(url);
            var stream = await GetStreamAsync(url, cancellationToken);
            return await LoadAsync(stream, format, settings);
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
        /// <param name="format">The Open API format</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream input, string format = null, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            settings ??= new OpenApiReaderSettings();
            format ??= InspectStreamFormat(input);

            Stream preparedStream;

            // Avoid buffering for JSON documents
            if (input is MemoryStream || format.Equals(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
            {
                preparedStream = input;
            }
            else
            {
                // Buffer stream for non-JSON formats (e.g., YAML) since they require synchronous reading
                preparedStream = new MemoryStream();
                await input.CopyToAsync(preparedStream, 81920, cancellationToken);
                preparedStream.Position = 0;
            }

            // Use StreamReader to process the prepared stream (buffered for YAML, direct for JSON)
            using var reader = new StreamReader(preparedStream, default, true, -1, settings.LeaveStreamOpen);
            var result = await LoadAsync(reader, format, settings, cancellationToken);

            if (!settings.LeaveStreamOpen)
            {
                input.Dispose();
            }
            return result;
        }


        /// <summary>
        /// Loads the TextReader input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The TextReader input.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(TextReader input, string format = null, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            format ??= InspectTextReaderFormat(input);
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(input, settings, cancellationToken);
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<ReadResult> ParseAsync(string input,
                                                        OpenApiReaderSettings settings = null)
        {
            var format = input.StartsWith("{") || input.StartsWith("[") ? OpenApiConstants.Json : OpenApiConstants.Yaml;
            settings ??= new OpenApiReaderSettings();
            using var reader = new StringReader(input);
            return await LoadAsync(reader, format, settings);
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="version"></param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static async Task<ReadFragmentResult> ParseAsync<T>(string input,
                                                                   OpenApiSpecVersion version,
                                                                   OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            var format = input.StartsWith("{") || input.StartsWith("[") ? OpenApiConstants.Json : OpenApiConstants.Yaml;
            settings ??= new OpenApiReaderSettings();
            using var reader = new StringReader(input);
            return await LoadAsync<T>(reader, version, format, settings);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static async Task<ReadFragmentResult> LoadAsync<T>(string url,
                                                                  OpenApiSpecVersion version,
                                                                  OpenApiReaderSettings settings = null,
                                                                  CancellationToken cancellationToken = default) where T : IOpenApiElement
        {
            var format = await GetFormatAsync(url);
            settings ??= new OpenApiReaderSettings();
            var stream = await GetStreamAsync(url, cancellationToken);
            return await LoadAsync<T>(stream, version, format, settings);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="format"></param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static async Task<ReadFragmentResult> LoadAsync<T>(Stream input,
                                                                  OpenApiSpecVersion version,
                                                                  string format = null,
                                                                  OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= InspectStreamFormat(input);
            using var reader = new StreamReader(input);
            return await LoadAsync<T>(reader, version, format, settings);
        }

        /// <summary>
        /// Reads the TextReader input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="format">The OpenAPI format.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static async Task<ReadFragmentResult> LoadAsync<T>(TextReader input,
                                                                  OpenApiSpecVersion version,
                                                                  string format = null,
                                                                  OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= InspectTextReaderFormat(input);
            return await OpenApiReaderRegistry.GetReader(format).ReadFragmentAsync<T>(input, version, settings);
        }

        private static async Task<string> GetContentTypeAsync(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var response = await _httpClient.GetAsync(url);
                var mediaType = response.Content.Headers.ContentType.MediaType;
                var contentType = mediaType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
                return contentType;
            }

            return null;
        }

        /// <summary>
        /// Infers the OpenAPI format from the input URL.
        /// </summary>
        /// <param name="url">The input URL.</param>
        /// <returns>The OpenAPI format.</returns>
        public static async Task<string> GetFormatAsync(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || 
                    url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    // URL examples ---> https://example.com/path/to/file.json, https://example.com/path/to/file.yaml
                    var path = new Uri(url);
                    var urlSuffix = path.Segments.LastOrDefault().Split('.').LastOrDefault();

                    if (!string.IsNullOrEmpty(urlSuffix))
                    {
                        return urlSuffix;
                    }
                    else
                    {
                        var contentType = await GetContentTypeAsync(url);
                        return contentType.Split('/').LastOrDefault();
                    }
                }
                else
                {
                    return Path.GetExtension(url).Split('.').LastOrDefault();
                }
            }
            return null;
        }

        private static string InspectStreamFormat(Stream stream)
        {
            try
            {
                if (stream == null) throw new ArgumentNullException(nameof(stream));
                if (!stream.CanSeek) throw new InvalidOperationException("Stream must support seeking."); // ensure stream supports seeking to reset position

                long initialPosition = stream.Position;
                int firstByte = stream.ReadByte();

                // Check if stream is empty or contains only whitespace
                if (firstByte == -1)
                {
                    stream.Position = initialPosition;
                    throw new InvalidOperationException("Stream is empty or contains only whitespace.");
                }

                // Skip whitespace if present and read the next non-whitespace byte
                if (char.IsWhiteSpace((char)firstByte))
                {
                    firstByte = stream.ReadByte();

                    // If still whitespace or end of stream, throw an error
                    if (firstByte == -1 || char.IsWhiteSpace((char)firstByte))
                    {
                        stream.Position = initialPosition;
                        throw new InvalidOperationException("Stream is empty or contains only whitespace.");
                    }
                }

                stream.Position = initialPosition; // Reset the stream position to the beginning

                char firstChar = (char)firstByte;
                return firstChar switch
                {
                    '{' or '[' => OpenApiConstants.Json,  // If the first character is '{' or '[', assume JSON
                    _ => OpenApiConstants.Yaml             // Otherwise assume YAML
                };
            }
            catch(Exception ex)
            {
                throw new OpenApiException(ex.Message);
            }            
        }

        private static string InspectTextReaderFormat(TextReader reader)
        {
            // Read the first line or a few characters from the input
            var input = reader.ReadLine().Trim();
            return input.StartsWith("{") || input.StartsWith("[") ? OpenApiConstants.Json : OpenApiConstants.Yaml;
        }

        private static async Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default)
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
