// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// A factory class for loading OpenAPI models from various sources.
    /// </summary>
    public static class OpenApiModelFactory
    {
        /// <summary>
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream"> The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The OpenAPI format.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Load(MemoryStream stream,
                                      string? format = null,
                                      OpenApiReaderSettings? settings = null)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(stream);
#else
            if (stream is null) throw new ArgumentNullException(nameof(stream));
#endif
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
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="format"></param>
        /// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static T? Load<T>(MemoryStream input, OpenApiSpecVersion version, string? format, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
            format ??= InspectStreamFormat(input);
            settings ??= DefaultReaderSettings.Value;
            return settings.GetReader(format).ReadFragment<T>(input, version, openApiDocument, out diagnostic, settings);
        }

        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings? settings = null, CancellationToken token = default)
        {
            settings ??= DefaultReaderSettings.Value;
            var (stream, format) = await RetrieveStreamAndFormatAsync(url, settings, token).ConfigureAwait(false);
            using (stream)
            {
                return await LoadAsync(stream, format, settings, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The path to the OpenAPI file</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
        /// <param name="token"></param>
        /// <returns>Instance of newly created IOpenApiElement.</returns>
        /// <returns>The OpenAPI element.</returns>
        public static async Task<T?> LoadAsync<T>(string url, OpenApiSpecVersion version, OpenApiDocument openApiDocument, OpenApiReaderSettings? settings = null, CancellationToken token = default) where T : IOpenApiElement
        {
            settings ??= DefaultReaderSettings.Value;
            var (stream, format) = await RetrieveStreamAndFormatAsync(url, settings, token).ConfigureAwait(false);
            using (stream)
            {
                return await LoadAsync<T>(stream, version, openApiDocument, format, settings, token);
            }
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.  If the stream is not buffered and it contains yaml, it will be buffered before parsing.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
        /// <param name="format">The Open API format</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream input, string? format = null, OpenApiReaderSettings? settings = null, CancellationToken cancellationToken = default)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(input);
#else
            if (input is null) throw new ArgumentNullException(nameof(input));
#endif
            settings ??= new OpenApiReaderSettings();

            Stream? preparedStream = null;
            if (format is null)
            {
                (preparedStream, format) = await PrepareStreamForReadingAsync(input, format, cancellationToken).ConfigureAwait(false);
            }

            // Use StreamReader to process the prepared stream (buffered for YAML, direct for JSON)
            var result = await InternalLoadAsync(preparedStream ?? input, format, settings, cancellationToken).ConfigureAwait(false);
            if (!settings.LeaveStreamOpen)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
                await input.DisposeAsync().ConfigureAwait(false);
#else
                input.Dispose();
#endif
            }
            if (preparedStream is not null && preparedStream != input)
            {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
                await preparedStream.DisposeAsync().ConfigureAwait(false);
#else
                preparedStream.Dispose();
#endif
            }
            return result;
        }

        /// <summary>
        /// Reads the stream input and ensures it is buffered before passing it to the Load method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="version"></param>
        /// <param name="openApiDocument">The document used to lookup tag or schema references.</param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T?> LoadAsync<T>(Stream input,
                                                 OpenApiSpecVersion version,
                                                 OpenApiDocument openApiDocument,
                                                 string? format = null,
                                                 OpenApiReaderSettings? settings = null,
                                                 CancellationToken token = default) where T : IOpenApiElement
        {
            Utils.CheckArgumentNull(openApiDocument);
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(input);
#else
            if (input is null) throw new ArgumentNullException(nameof(input));
#endif
            if (input is MemoryStream memoryStream)
            {
                return Load<T>(memoryStream, version, format, openApiDocument, out var _, settings);
            }
            else
            {
                memoryStream = new MemoryStream();
                await input.CopyToAsync(memoryStream, 81920, token).ConfigureAwait(false);
                memoryStream.Position = 0;
                return Load<T>(memoryStream, version, format, openApiDocument, out var _, settings);
            }
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static ReadResult Parse(string input,
                                       string? format = null,
                                       OpenApiReaderSettings? settings = null)
        {
#if NET6_0_OR_GREATER
            ArgumentException.ThrowIfNullOrEmpty(input);
#else
            if (string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));
#endif
            format ??= InspectInputFormat(input);
            settings ??= new OpenApiReaderSettings();

            // Copy string into MemoryStream
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return InternalLoad(stream, format, settings);
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="version"></param>
        /// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static T? Parse<T>(string input,
                                 OpenApiSpecVersion version,
                                 OpenApiDocument openApiDocument,
                                 out OpenApiDiagnostic diagnostic,
                                 string? format = null,
                                 OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
#if NET6_0_OR_GREATER
            ArgumentException.ThrowIfNullOrEmpty(input);
#else
            if (string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));
#endif
            format ??= InspectInputFormat(input);
            settings ??= new OpenApiReaderSettings();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            return Load<T>(stream, version, format, openApiDocument, out diagnostic, settings);
        }

        private static readonly Lazy<OpenApiReaderSettings> DefaultReaderSettings = new(() => new OpenApiReaderSettings());

        private static async Task<ReadResult> InternalLoadAsync(Stream input, string format, OpenApiReaderSettings settings, CancellationToken cancellationToken = default)
        {
            settings ??= DefaultReaderSettings.Value;
            var reader = settings.GetReader(format);
            var location =
                        (input is FileStream fileStream ? new Uri(fileStream.Name) : null) ??
                        settings.BaseUrl ??
                        new Uri(OpenApiConstants.BaseRegistryUri);

            var readResult = await reader.ReadAsync(input, location, settings, cancellationToken).ConfigureAwait(false);

            if (settings.LoadExternalRefs)
            {
                var diagnosticExternalRefs = await LoadExternalRefsAsync(readResult.Document, settings, format, cancellationToken).ConfigureAwait(false);
                // Merge diagnostics of external reference
                if (diagnosticExternalRefs != null)
                {
                    readResult.Diagnostic?.Errors.AddRange(diagnosticExternalRefs.Errors);
                    readResult.Diagnostic?.Warnings.AddRange(diagnosticExternalRefs.Warnings);
                }
            }

            return readResult;
        }

        private static async Task<OpenApiDiagnostic> LoadExternalRefsAsync(OpenApiDocument? document, OpenApiReaderSettings settings, string? format = null, CancellationToken token = default)
        {
            // Load this document into the workspace
            var streamLoader = new DefaultStreamLoader(settings.HttpClient);
            var workspace = document?.Workspace ?? new OpenApiWorkspace();
            var workspaceLoader = new OpenApiWorkspaceLoader(workspace, settings.CustomExternalLoader ?? streamLoader, settings);
            return await workspaceLoader.LoadAsync(new BaseOpenApiReference() { ExternalResource = "/" }, document, format ?? OpenApiConstants.Json, null, token).ConfigureAwait(false);
        }

        private static ReadResult InternalLoad(MemoryStream input, string format, OpenApiReaderSettings settings)
        {
            settings ??= DefaultReaderSettings.Value;
            if (settings.LoadExternalRefs)
            {
                throw new InvalidOperationException("Loading external references are not supported when using synchronous methods.");
            }
            if (input.Length == 0 || input.Position == input.Length)
            {
                throw new ArgumentException($"Cannot parse the stream: {nameof(input)} is empty or contains no elements.");
            }

            var location = new Uri(OpenApiConstants.BaseRegistryUri);
            var reader = settings.GetReader(format);
            var readResult = reader.Read(input, location, settings);
            return readResult;
        }

        private static async Task<(Stream, string?)> RetrieveStreamAndFormatAsync(string url, OpenApiReaderSettings settings, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"Parameter {nameof(url)} is null or empty. Please provide the correct path or URL to the file.");
            }
            else
            {
                Stream stream;
                string? format;

                if (url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                    || url.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await settings.HttpClient.GetAsync(url, token).ConfigureAwait(false);
                    var mediaType = response.Content.Headers.ContentType?.MediaType;
                    var contentType = mediaType?.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                    format = contentType?.Split('/').Last().Split('+').Last().Split('-').Last();

                    // for non-standard MIME types e.g. text/x-yaml used in older libs or apps
#if NETSTANDARD2_0
                    stream = await response.Content.ReadAsStreamAsync();
#else
                    stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
#endif
                    return (stream, format);
                }
                else
                {
                    try
                    {
                        string fileName;
                        if (url.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName = new Uri(url).LocalPath;
                        }
                        else
                        {
                            fileName = url;
                        }

                        format = Path.GetExtension(fileName).Split('.').LastOrDefault();

                        var fileInput = new FileInfo(fileName);
                        stream = fileInput.OpenRead();
                    }
                    catch (Exception ex) when (
                        ex is
                            UriFormatException or
                            FormatException or
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
        }

        private static string InspectInputFormat(string input)
        {
            return input.StartsWith("{", StringComparison.OrdinalIgnoreCase) || input.StartsWith("[", StringComparison.OrdinalIgnoreCase) ? OpenApiConstants.Json : OpenApiConstants.Yaml;
        }

        /// <summary>
        /// Reads the initial bytes of the stream to determine if it is JSON or YAML.
        /// </summary>
        /// <remarks>
        /// It is important NOT TO change the stream type from MemoryStream.
        /// In Asp.Net core 3.0+ we could get passed a stream from a request or response body.
        /// In such case, we CAN'T use the ReadByte method as it throws NotSupportedException.
        /// Therefore, we need to ensure that the stream is a MemoryStream before calling this method.
        /// Maintaining this type ensures there won't be any unforeseen wrong usage of the method.
        /// </remarks>
        /// <param name="stream">The stream to inspect</param>
        /// <returns>The format of the stream.</returns>
        private static string InspectStreamFormat(MemoryStream stream)
        {
            return TryInspectStreamFormat(stream, out var format) ? format! : throw new InvalidOperationException("Could not determine the format of the stream.");
        }
        private static bool TryInspectStreamFormat(Stream stream, out string? format)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(stream);
#else
            if (stream is null) throw new ArgumentNullException(nameof(stream));
#endif

            try
            {
                var initialPosition = stream.Position;
                var firstByte = (char)stream.ReadByte();

                // Skip whitespace if present and read the next non-whitespace byte
                if (char.IsWhiteSpace(firstByte))
                {
                    firstByte = (char)stream.ReadByte();
                }

                stream.Position = initialPosition; // Reset the stream position to the beginning

                format = firstByte switch
                {
                    '{' or '[' => OpenApiConstants.Json,  // If the first character is '{' or '[', assume JSON
                    _ => OpenApiConstants.Yaml             // Otherwise assume YAML
                };
                return true;
            }
            catch (NotSupportedException)
            {
                // https://github.com/dotnet/aspnetcore/blob/c9d0750396e1d319301255ba61842721ab72ab10/src/Servers/Kestrel/Core/src/Internal/Http/HttpResponseStream.cs#L40
            }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
            catch (InvalidOperationException ex) when (ex.Message.Contains("AllowSynchronousIO", StringComparison.Ordinal))
#else
            catch (InvalidOperationException ex) when (ex.Message.Contains("AllowSynchronousIO"))
#endif
            {
                // https://github.com/dotnet/aspnetcore/blob/c9d0750396e1d319301255ba61842721ab72ab10/src/Servers/HttpSys/src/RequestProcessing/RequestStream.cs#L100-L108
                // https://github.com/dotnet/aspnetcore/blob/c9d0750396e1d319301255ba61842721ab72ab10/src/Servers/IIS/IIS/src/Core/HttpRequestStream.cs#L24-L30
                // https://github.com/dotnet/aspnetcore/blob/c9d0750396e1d319301255ba61842721ab72ab10/src/Servers/Kestrel/Core/src/Internal/Http/HttpRequestStream.cs#L54-L60
            }
            format = null;
            return false;
        }

        private static async Task<MemoryStream> CopyToMemoryStreamAsync(Stream input, CancellationToken token)
        {
            var bufferStream = new MemoryStream();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
            await input.CopyToAsync(bufferStream, token).ConfigureAwait(false);
#else
            await input.CopyToAsync(bufferStream, 81920, token).ConfigureAwait(false);
#endif
            bufferStream.Position = 0;
            return bufferStream;
        } 

        private static async Task<(Stream, string)> PrepareStreamForReadingAsync(Stream input, string? format, CancellationToken token = default)
        {
            Stream preparedStream = input;

            if (input is MemoryStream ms)
            {
                format ??= InspectStreamFormat(ms);
            }
            else if (!input.CanSeek)
            {
                // Copy to a MemoryStream to enable seeking and perform format inspection
                var bufferStream = await CopyToMemoryStreamAsync(input, token).ConfigureAwait(false);
                return await PrepareStreamForReadingAsync(bufferStream, format, token).ConfigureAwait(false);
            }
            else if (!TryInspectStreamFormat(input, out format!))
            {
                var bufferStream = await CopyToMemoryStreamAsync(input, token).ConfigureAwait(false);
                return await PrepareStreamForReadingAsync(bufferStream, format, token).ConfigureAwait(false);
            }

            return (preparedStream, format);
        }
    }
}
