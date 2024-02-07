// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Creates a single instance of HttpClient for reuse
    /// </summary>
    public static class HttpClientFactory
    {
        private static readonly HttpClient _httpClient;

        static HttpClientFactory()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Returns a static http client instance
        /// </summary>
        /// <returns>A http client.</returns>
        public static HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
