// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="url">The path to the OpenAPI file.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Load(string url, OpenApiReaderSettings settings = null)
        {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            return LoadAsync(url, settings).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream"> The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The OpenAPI format.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Load(MemoryStream stream,
                                      string format,
                                      OpenApiReaderSettings settings = null)
        {
            settings ??= new OpenApiReaderSettings();

            var result = InternalLoad(stream, format, settings);

            if (!settings.LeaveStreamOpen)
            {
                stream.Dispose();
            }

            return result;
        }


        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null)
        {
            var format = GetFormat(url);
            var stream = await GetStreamAsync(url);  // Get response back and then get Content
            return await LoadAsync(stream, format, settings);
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.  If the stream is not buffered and it contains yaml, it will be buffered before parsing.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
        /// <param name="format">The Open API format</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream input, string format, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            settings ??= new OpenApiReaderSettings();

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
            return await InternalLoadAsync(preparedStream, format, settings, cancellationToken);
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Parse(string input,
                                       string format = null,
                                       OpenApiReaderSettings settings = null)
        {
            format ??= OpenApiConstants.Json;
            settings ??= new OpenApiReaderSettings();

            // Copy string into MemoryStream
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return InternalLoad(stream, format, settings);
        }

        private static async Task<ReadResult> InternalLoadAsync(Stream input, string format, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            var readResult = await reader.ReadAsync(input, settings, cancellationToken);
            return readResult;
        }

        private static ReadResult InternalLoad(MemoryStream input, string format, OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            var readResult = reader.Read(input, settings);
            return readResult;
        }


        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static T Parse<T>(string input,
                                 OpenApiSpecVersion version,
                                 out OpenApiDiagnostic diagnostic,
                                 string format = null,
                                 OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= OpenApiConstants.Json;
            settings ??= new OpenApiReaderSettings();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return Load<T>(stream, version, format, out diagnostic,  settings);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static T Load<T>(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            var format = GetFormat(url);
            settings ??= new OpenApiReaderSettings();

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            var stream = GetStreamAsync(url).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

            return Load<T>(stream as MemoryStream, version, format, out diagnostic, settings);
        }


        /// <summary>
        /// Reads the stream input and ensures it is buffered before passing it to the Load method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="version"></param>
        /// <param name="format"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Load<T>(Stream input, OpenApiSpecVersion version, string format, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            if (input is MemoryStream memoryStream)
            {
                return Load<T>(memoryStream, version, format, out diagnostic, settings);
            } else {
                memoryStream = new MemoryStream();
                input.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return Load<T>(memoryStream, version, format, out diagnostic, settings);
            }
        }


        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="format"></param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static T Load<T>(MemoryStream input, OpenApiSpecVersion version, string format, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= OpenApiConstants.Json;
            return OpenApiReaderRegistry.GetReader(format).ReadFragment<T>(input, version, out diagnostic, settings);
        }

        private static string GetContentType(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

                var mediaType = response.Content.Headers.ContentType.MediaType;
                return mediaType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
            }

            return null;
        }

        /// <summary>
        /// Infers the OpenAPI format from the input URL.
        /// </summary>
        /// <param name="url">The input URL.</param>
        /// <returns>The OpenAPI format.</returns>
        public static string GetFormat(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    // URL examples ---> https://example.com/path/to/file.json, https://example.com/path/to/file.yaml
                    var path = new Uri(url);
                    var urlSuffix = path.Segments[path.Segments.Length - 1].Split('.').LastOrDefault();

                    return !string.IsNullOrEmpty(urlSuffix) ? urlSuffix : GetContentType(url).Split('/').LastOrDefault();
                }
                else
                {
                    return Path.GetExtension(url).Split('.').LastOrDefault();
                }
            }
            return null;
        }

        private static async Task<Stream> GetStreamAsync(string url)
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
