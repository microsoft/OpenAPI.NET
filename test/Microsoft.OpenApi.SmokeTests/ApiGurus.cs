using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.SmokeTests
{
    [Collection("DefaultSettings")]
    public class ApisGuruTests
    {
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler() {
            AutomaticDecompression = DecompressionMethods.GZip
        });

        private readonly ITestOutputHelper output;
    
        public ApisGuruTests(ITestOutputHelper output)
        {
           
            this.output = output;
        }

        static ApisGuruTests()
        {
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


        [Theory(DisplayName = "APIs.guru")]
        [MemberData(nameof(GetSchemas))]
        public async Task EnsureThatICouldParse(string url)
        {
            var stopwatch = new Stopwatch();
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                output.WriteLine($"Couldn't load {url}");
                return;
            }
            await response.Content.LoadIntoBufferAsync();
            var stream = await response.Content.ReadAsStreamAsync();

            stopwatch.Start();

            var reader = new OpenApiStreamReader(new OpenApiReaderSettings() {
            });
            var openApiDocument = reader.Read(stream, out var diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                output.WriteLine($"Errors parsing {url}");
                output.WriteLine(String.Join("\n", diagnostic.Errors));
                Assert.True(false);
            }

            Assert.NotNull(openApiDocument);
            stopwatch.Stop();
                output.WriteLine($"Parsing {url} took {stopwatch.ElapsedMilliseconds} ms.");
        }

        //[Theory(DisplayName = "APIs.guru")]
        //[MemberData(nameof(GetSchemas))]
        public async Task EnsureAllErrorsAreHandled(string url)
        {
            var stopwatch = new Stopwatch();

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                output.WriteLine($"Couldn't load {url}");
                return;
            }
            await response.Content.LoadIntoBufferAsync();
            var stream = await response.Content.ReadAsStreamAsync();

            stopwatch.Start();

            var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

            output.WriteLine(String.Join("\n", diagnostic.Errors));
            Assert.Equal(OpenApiSpecVersion.OpenApi2_0, diagnostic.SpecificationVersion);

            Assert.NotNull(openApiDocument);
            stopwatch.Stop();
            output.WriteLine($"Parsing {url} took {stopwatch.ElapsedMilliseconds} ms and has {diagnostic.Errors.Count} errors.");
        }
    }
}