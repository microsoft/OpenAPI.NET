using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ApisGuruTests
{
    [Collection("DefaultSettings")]
    public class ApisGuruTests
    {
        public static IEnumerable<object[]> GetSchemas()
        {
            using (var client = new HttpClient())
            {
                var listJsonStr = client
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
            using (var httpClient = new HttpClient())
            {
                var stream = await httpClient.GetStreamAsync(url);
                var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

                Assert.Equal(OpenApiSpecVersion.OpenApi2_0, diagnostic.SpecificationVersion);
                Assert.Equal(0, diagnostic.Errors.Count);

                Assert.NotNull(openApiDocument);
            }
        }
    }
}
