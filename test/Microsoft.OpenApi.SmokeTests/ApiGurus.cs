// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.SmokeTests
{
    [Collection("DefaultSettings")]
    public class ApisGuruTests
    {
        private static HttpClient _httpClient;
        private readonly ITestOutputHelper _output;
    
        public ApisGuruTests(ITestOutputHelper output)
        {
            _output = output;
        }

        static ApisGuruTests()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("OpenApi.Net.Tests", "1.0"));
        }

        public static IEnumerable<object[]> GetSchemas()
        {
            var listJsonStr = _httpClient
                    .GetStringAsync("https://api.apis.guru/v2/list.json")
                    .GetAwaiter().GetResult();

                var json = JObject.Parse(listJsonStr);
                foreach (var item in json.Properties())
                {
                    var versions = GetProp(item.Value, "versions") as JObject;
                    if (versions == null)
                        continue;
                    foreach (var prop in versions.Properties())
                    {
                        var urlToJson = GetProp(prop.Value, "swaggerUrl")?.ToObject<string>();
                        if (urlToJson != null)
                            yield return new object[] { urlToJson };

                        var utlToYaml = GetProp(prop.Value, "swaggerYamlUrl")?.ToObject<string>();
                        if (utlToYaml != null)
                            yield return new object[] { utlToYaml };
                    }
            }

            JToken GetProp(JToken obj, string prop)
            {
                if (!(obj is JObject jObj))
                    return null;
                if (!jObj.TryGetValue(prop, out var jToken))
                    return null;
                return jToken;
            }
        }

        // Disable as some APIs are currently invalid [Theory(DisplayName = "APIs.guru")]
        [MemberData(nameof(GetSchemas))]
        public async Task EnsureThatICouldParse(string url)
        {  
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _output.WriteLine($"Couldn't load {url}");
                return;
            }

            await response.Content.LoadIntoBufferAsync();
            var stream = await response.Content.ReadAsStreamAsync();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var reader = new OpenApiStreamReader();
            var openApiDocument = reader.Read(stream, out var diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                _output.WriteLine($"Errors parsing {url}");
                _output.WriteLine(String.Join("\n", diagnostic.Errors));
 //               Assert.True(false);  // Uncomment to identify descriptions with errors.
            }

            Assert.NotNull(openApiDocument);
            stopwatch.Stop();
                _output.WriteLine($"Parsing {url} took {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}