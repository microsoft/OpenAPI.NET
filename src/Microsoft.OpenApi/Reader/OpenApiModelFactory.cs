// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.Services;
using Microsoft.OpenApi.Services;

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
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream"> The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The OpenAPI format.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Load(MemoryStream stream,
                                      string format = null,
                                      OpenApiReaderSettings settings = null)
        {
            settings ??= new OpenApiReaderSettings();

            // Get the format of the stream if not provided
            format ??= InspectStreamFormat(stream);
            var result = InternalLoad(stream, format, settings);

            if (!settings.LeaveStreamOpen)
            {
                stream.Dispose();
            }

            return result;
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
        public static T Load<T>(Stream input,
                                OpenApiSpecVersion version,
                                out OpenApiDiagnostic diagnostic,
                                string format = null,
                                OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            if (input is MemoryStream memoryStream)
            {
                return Load<T>(memoryStream, version, format, out diagnostic, settings);
            }
            else
            {
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
            format ??= InspectStreamFormat(input);
            return OpenApiReaderRegistry.GetReader(format).ReadFragment<T>(input, version, out diagnostic, settings);
        }

        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null)
        {
            var result = await RetrieveStreamAndFormatAsync(url);
            return await LoadAsync(result.Item1, result.Item2, settings).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static async Task<T> LoadAsync<T>(string url, OpenApiSpecVersion version, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            var result = await RetrieveStreamAndFormatAsync(url);
            return Load<T>(result.Item1, version, out var _, result.Item2, settings);
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.  If the stream is not buffered and it contains yaml, it will be buffered before parsing.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
        /// <param name="format">The Open API format</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream input, string format = null, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            settings ??= new OpenApiReaderSettings();
            Stream preparedStream;
            if (format is null)
            {
                var readResult = await PrepareStreamForReadingAsync(input, format, cancellationToken);
                preparedStream = readResult.Item1;
                format = readResult.Item2;
            }
            else
            {
                preparedStream = input;
            }

            // Use StreamReader to process the prepared stream (buffered for YAML, direct for JSON)
            var result = await InternalLoadAsync(preparedStream, format, settings, cancellationToken);
            if (!settings.LeaveStreamOpen)
            {
                input.Dispose();
            }

            return result;
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
            format ??= InspectInputFormat(input);
            settings ??= new OpenApiReaderSettings();

            // Copy string into MemoryStream
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return InternalLoad(stream, format, settings);
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
            format ??= InspectInputFormat(input);
            settings ??= new OpenApiReaderSettings();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return Load<T>(stream, version, format, out diagnostic, settings);
        }

        private static readonly OpenApiReaderSettings DefaultReaderSettings = new();

        private static async Task<ReadResult> InternalLoadAsync(Stream input, string format, OpenApiReaderSettings settings, CancellationToken cancellationToken = default)
        {
            var reader = OpenApiReaderRegistry.GetReader(format);
            var readResult = await reader.ReadAsync(input, settings, cancellationToken);

            if (settings?.LoadExternalRefs ?? DefaultReaderSettings.LoadExternalRefs)
            {
                var diagnosticExternalRefs = await LoadExternalRefsAsync(readResult.OpenApiDocument, cancellationToken, settings, format);
                // Merge diagnostics of external reference
                if (diagnosticExternalRefs != null)
                {
                    readResult.OpenApiDiagnostic.Errors.AddRange(diagnosticExternalRefs.Errors);
                    readResult.OpenApiDiagnostic.Warnings.AddRange(diagnosticExternalRefs.Warnings);
                }
            }

            return readResult;
        }

        private static async Task<OpenApiDiagnostic> LoadExternalRefsAsync(OpenApiDocument document, CancellationToken cancellationToken, OpenApiReaderSettings settings, string format = null)
        {
            // Create workspace for all documents to live in.
            var baseUrl = settings.BaseUrl ?? new Uri(OpenApiConstants.BaseRegistryUri);
            var openApiWorkSpace = new OpenApiWorkspace(baseUrl);

            // Load this root document into the workspace
            var streamLoader = new DefaultStreamLoader(settings.BaseUrl);
            var workspaceLoader = new OpenApiWorkspaceLoader(openApiWorkSpace, settings.CustomExternalLoader ?? streamLoader, settings);
            return await workspaceLoader.LoadAsync(new OpenApiReference() { ExternalResource = "/" }, document, format ?? OpenApiConstants.Json, null, cancellationToken).ConfigureAwait(false);
        }

        private static ReadResult InternalLoad(MemoryStream input, string format, OpenApiReaderSettings settings)
        {
            if (settings?.LoadExternalRefs ?? DefaultReaderSettings.LoadExternalRefs)
            {
                throw new InvalidOperationException("Loading external references are not supported when using synchronous methods.");
            }

            var reader = OpenApiReaderRegistry.GetReader(format);
            var readResult = reader.Read(input, settings);

            return readResult;
        }

        private static async Task<(Stream, string)> RetrieveStreamAndFormatAsync(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Stream stream;
                string format;

                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await _httpClient.GetAsync(url);
                    var mediaType = response.Content.Headers.ContentType.MediaType;
                    var contentType = mediaType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                    format = contentType.Split('/').LastOrDefault();
                    stream = await response.Content.ReadAsStreamAsync();
                    return (stream, format);
                }
                else
                {
                    format = Path.GetExtension(url).Split('.').LastOrDefault();

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

                    return (stream, format);
                }
            }
            return (null, null);
        }

        private static string InspectInputFormat(string input)
        {
            return input.StartsWith("{", StringComparison.OrdinalIgnoreCase) || input.StartsWith("[", StringComparison.OrdinalIgnoreCase) ? OpenApiConstants.Json : OpenApiConstants.Yaml;
        }

        private static string InspectStreamFormat(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));            
            
            long initialPosition = stream.Position;
            int firstByte = stream.ReadByte();

            // Skip whitespace if present and read the next non-whitespace byte
            if (char.IsWhiteSpace((char)firstByte))
            {
                firstByte = stream.ReadByte();
            }

            stream.Position = initialPosition; // Reset the stream position to the beginning

            char firstChar = (char)firstByte;
            return firstChar switch
            {
                '{' or '[' => OpenApiConstants.Json,  // If the first character is '{' or '[', assume JSON
                _ => OpenApiConstants.Yaml             // Otherwise assume YAML
            };
        }

        private static async Task<(Stream, string)> PrepareStreamForReadingAsync(Stream input, string format, CancellationToken token = default)
        {
            Stream preparedStream = input;

            if (!input.CanSeek)
            {
                // Use a temporary buffer to read a small portion for format detection
                using var bufferStream = new MemoryStream();
                await input.CopyToAsync(bufferStream, 1024, token).ConfigureAwait(false);
                bufferStream.Position = 0;

                // Inspect the format from the buffered portion
                format ??= InspectStreamFormat(bufferStream);

                // If format is JSON, no need to buffer further — use the original stream.
                if (format.Equals(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
                {
                    preparedStream = input;
                }
                else
                {
                    // YAML or other non-JSON format; copy remaining input to a new stream.
                    preparedStream = new MemoryStream();
                    bufferStream.Position = 0;
                    await bufferStream.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(false); // Copy buffered portion
                    await input.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(false); // Copy remaining data
                    preparedStream.Position = 0;
                }
            }
            else
            {
                format ??= InspectStreamFormat(input);

                if (!format.Equals(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
                {
                    // Buffer stream for non-JSON formats (e.g., YAML) since they require synchronous reading
                    preparedStream = new MemoryStream();
                    await input.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(false);
                    preparedStream.Position = 0;
                }
            }

            return (preparedStream, format);
        }
    }
}
