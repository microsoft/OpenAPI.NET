// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Models
{
    internal static class OpenApiModelFactory
    {
        private static readonly HttpClient _httpClient = HttpClientFactory.GetHttpClient();

        static OpenApiModelFactory()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Json, new OpenApiJsonReader());
        }

        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The input to read from.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static OpenApiDocument Load(string url, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            var format = GetFormat(url);
            return OpenApiReaderRegistry.GetReader(format).Read(url, out diagnostic, settings);
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream"> The input stream.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The OpenAPI format.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static OpenApiDocument Load(Stream stream,
                                           string format,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return reader.Read(stream, out diagnostic, settings);
        }

        /// <summary>
        /// Loads the TextReader input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The TextReader input.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The Open API format</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static OpenApiDocument Load(TextReader input,
                                           string format,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return reader.Read(input, out diagnostic, settings);
        }

        /// <summary>
        /// Loads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="format">The Open API format</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream stream, string format, OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(stream, settings);
        }

        /// <summary>
        /// Loads the TextReader input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The TextReader input.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(TextReader input, string format, OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(input, settings);
        }

        /// <summary>
        /// Loads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The input URL.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null)
        {
            var format = GetFormat(url);
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(url, settings);
        }

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="format">The Open API format</param>
        /// <param name="settings">The OpenApi reader settings.</param>
        /// <returns>An OpenAPI document instance.</returns>
        public static OpenApiDocument Parse(string input,
                                            out OpenApiDiagnostic diagnostic,
                                            string format = null,
                                            OpenApiReaderSettings settings = null)
        {
            format ??= OpenApiConstants.Json;
            return OpenApiReaderRegistry.GetReader(format).Parse(input, out diagnostic, settings);
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
            return OpenApiReaderRegistry.GetReader(format).Parse<T>(input, version, out diagnostic, settings);
        }

        public static T Load<T>(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            var format = GetFormat(url);
            return OpenApiReaderRegistry.GetReader(format).Read<T>(url, version, out diagnostic, settings);
        }

        public static T Load<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, string format, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= OpenApiConstants.Json;
            return OpenApiReaderRegistry.GetReader(format).Read<T>(input, version, out diagnostic, settings);
        }

        public static T Load<T>(TextReader input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, string format, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            format ??= OpenApiConstants.Json;
            return OpenApiReaderRegistry.GetReader(format).Read<T>(input, version, out diagnostic, settings);
        }


        private static string GetContentType(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                var mediaType = response.Content.Headers.ContentType.MediaType;
                var contentType = mediaType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).First();
                return contentType.Split('/').LastOrDefault();
            }

            return null;
        }

        private static string GetFormat(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    // URL examples ---> https://example.com/path/to/file.json, https://example.com/path/to/file.yaml
                    var path = new Uri(url);
                    var urlSuffix = path.Segments[path.Segments.Length - 1].Split('.').LastOrDefault();

                    return !string.IsNullOrEmpty(urlSuffix) ? urlSuffix : GetContentType(url);
                }
                else
                {
                    return Path.GetExtension(url).Split('.').LastOrDefault();
                }
            }
            return null;
        }
    }
}
