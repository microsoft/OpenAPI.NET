// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers.Services
{
    /// <summary>
    /// Implementation of IInputLoader that loads streams from URIs
    /// </summary>
    internal class DefaultStreamLoader : IInputLoader<Stream>
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<Stream> LoadAsync(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "file":
                    return File.OpenRead(uri.AbsolutePath);
                case "http":
                case "https":
                    return await _httpClient.GetStreamAsync(uri);

                default:
                    throw new ArgumentException("Unsupported scheme");
            }
        }
    }
}
