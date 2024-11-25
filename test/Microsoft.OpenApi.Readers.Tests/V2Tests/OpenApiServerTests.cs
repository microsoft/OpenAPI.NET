using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiServerTests
    {
        public OpenApiServerTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public async Task NoServer()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;

            var result = await OpenApiDocument.ParseAsync(input);

            Assert.Empty(result.OpenApiDocument.Servers);
        }

        [Fact]
        public async Task JustSchemeNoDefault()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                schemes:
                  - http
                paths: {}
                """;
            var result = await OpenApiDocument.ParseAsync(input);

            Assert.Empty(result.OpenApiDocument.Servers);
        }

        [Fact]
        public async Task JustHostNoDefault()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: www.foo.com
                paths: {}
                """;
            var result = await OpenApiDocument.ParseAsync(input);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("//www.foo.com", server.Url);
        }

        [Fact]
        public async Task NoBasePath()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: www.foo.com
                schemes:
                  - http
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://www.foo.com/spec.yaml")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);
            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("http://www.foo.com", server.Url);
        }

        [Fact]
        public async Task JustBasePathNoDefault()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                basePath: /baz
                paths: {}
                """;
            var result = await OpenApiDocument.ParseAsync(input);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("/baz", server.Url);
        }

        [Fact]
        public async Task JustSchemeWithCustomHost()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                schemes:
                  - http
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com/foo")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("http://bing.com/foo", server.Url);
        }

        [Fact]
        public async Task JustSchemeWithCustomHostWithEmptyPath()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                schemes:
                  - http
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("http://bing.com", server.Url);
        }

        [Fact]
        public async Task JustBasePathWithCustomHost()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                basePath: /api
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("https://bing.com/api", server.Url);
        }

        [Fact]
        public async Task JustHostWithCustomHost()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: www.example.com
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("https://www.example.com", server.Url);
        }

        [Fact]
        public async Task JustHostWithCustomHostWithApi()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: prod.bing.com
                paths: {}
                """;

            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://dev.bing.com/api/description.yaml")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);
            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("https://prod.bing.com", server.Url);
        }

        [Fact]
        public async Task MultipleServers()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                schemes:
                  - http
                  - https
                paths: {}
                """;

            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://dev.bing.com/api")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);
            var server = result.OpenApiDocument.Servers.First();
            Assert.Equal(2, result.OpenApiDocument.Servers.Count);
            Assert.Equal("http://dev.bing.com/api", server.Url);
            Assert.Equal("https://dev.bing.com/api", result.OpenApiDocument.Servers.Last().Url);
        }

        [Fact]
        public async Task LocalHostWithCustomHost()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: localhost:23232
                paths: {}
                """;

            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);

            var server = result.OpenApiDocument.Servers.First();
            Assert.Single(result.OpenApiDocument.Servers);
            Assert.Equal("https://localhost:23232", server.Url);
        }

        [Fact]
        public async Task InvalidHostShouldYieldError()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: http://test.microsoft.com
                paths: {}
                """;

            var settings = new OpenApiReaderSettings
            {
                BaseUrl = new("https://bing.com")
            };

            var result = await OpenApiDocument.ParseAsync(input, settings);
            result.OpenApiDocument.Servers.Count.Should().Be(0);
            result.OpenApiDiagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                        new OpenApiError("#/", "Invalid host"),
                        new OpenApiError("", "Paths is a REQUIRED field at #/")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi2_0
                });
        }
    }
}
