// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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

        public static OpenApiDocument Load(string url, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            var format = GetFormat(url);
            return OpenApiReaderRegistry.GetReader(format).Read(url, out diagnostic, settings);
        }

        public static OpenApiDocument Load(Stream stream,
                                           string format,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return reader.Read(stream, out diagnostic, settings);
        }

        public static OpenApiDocument Load(TextReader input,
                                           string format,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return reader.Read(input, out diagnostic, settings);
        }

        public static async Task<ReadResult> LoadAsync(Stream stream, string format, OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(stream, settings);
        }

        public static async Task<ReadResult> LoadAsync(TextReader input, string format, OpenApiReaderSettings settings = null)
        {
            Utils.CheckArgumentNull(format, nameof(format));
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(input, settings);
        }

        public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings settings = null)
        {
            var format = GetFormat(url);
            var reader = OpenApiReaderRegistry.GetReader(format);
            return await reader.ReadAsync(url, settings);
        }

        public static OpenApiDocument Parse(string input,
                                            out OpenApiDiagnostic diagnostic,
                                            string format = null,
                                            OpenApiReaderSettings settings = null)
        {
            format ??= OpenApiConstants.Json;
            return OpenApiReaderRegistry.GetReader(format).Parse(input, out diagnostic, settings);
        }

        private static string GetContentType(string url)
        {
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            var contentType = response.Content.Headers.ContentType.MediaType;
            if (contentType.EndsWith(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
            {
                return OpenApiConstants.Json;
            }
            else if (contentType.EndsWith(OpenApiConstants.Yaml, StringComparison.OrdinalIgnoreCase))
            {
                return OpenApiConstants.Yaml;
            }
            return null;
        }

        private static string GetFormat(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                    {
                    if (url.EndsWith(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase)
                                    || GetContentType(url).Equals(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiConstants.Json;
                    }
                    else if (url.EndsWith(OpenApiConstants.Yaml, StringComparison.OrdinalIgnoreCase)
                        || url.EndsWith(OpenApiConstants.Yml, StringComparison.OrdinalIgnoreCase)
                                    || GetContentType(url).Equals(OpenApiConstants.Yml, StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiConstants.Yaml;
                    }
                }
                else
                {
                    if (url.EndsWith(OpenApiConstants.Json, StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiConstants.Json;
                    }
                    else if (url.EndsWith(OpenApiConstants.Yaml, StringComparison.OrdinalIgnoreCase) 
                        || url.EndsWith(OpenApiConstants.Yml, StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiConstants.Yaml;
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported file format");
                    }
                }
            }
            return null;
        }
    }
}
