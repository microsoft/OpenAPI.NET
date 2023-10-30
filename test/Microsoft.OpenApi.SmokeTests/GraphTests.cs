using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using System;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.SmokeTests
{
    public class GraphTests
    {
        OpenApiDocument _graphOpenApi;
        HttpClient _httpClient;
        private readonly ITestOutputHelper _output;
        const string graphOpenApiUrl = "https://github.com/microsoftgraph/microsoft-graph-openapi/blob/master/v1.0.json?raw=true";

        public GraphTests(ITestOutputHelper output)
        {
            _output = output;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _httpClient = new(new HttpClientHandler
            {                AutomaticDecompression = DecompressionMethods.GZip
            });
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new("gzip"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new("OpenApi.Net.Tests", "1.0"));

            var response = _httpClient.GetAsync(graphOpenApiUrl)
                                .GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                _output.WriteLine($"Couldn't load graph openapi");
                return;
            }

            var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult(); ;

            var reader = new OpenApiStreamReader();
            _graphOpenApi = reader.Read(stream, out var diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                _output.WriteLine($"Errors parsing");
                _output.WriteLine(String.Join('\n', diagnostic.Errors));
                //               Assert.True(false);  // Uncomment to identify descriptions with errors.
            }
        }

        //[Fact(Skip="Run manually")]
        public void LoadOpen()
        {
            var operations = new[] { "foo","bar" };
            var workspace = new OpenApiWorkspace();
            workspace.AddDocument(graphOpenApiUrl, _graphOpenApi);
            var subset = new OpenApiDocument();
            workspace.AddDocument("subset", subset);

            Assert.NotNull(_graphOpenApi);
        }
    }
}
