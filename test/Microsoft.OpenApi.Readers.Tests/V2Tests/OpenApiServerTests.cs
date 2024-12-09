using System.Linq;
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
        public void NoServer()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;

            var result = OpenApiDocument.Parse(input, "yaml");

            Assert.Empty(result.Document.Servers);
        }

        [Fact]
        public void JustSchemeNoDefault()
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
            var result = OpenApiDocument.Parse(input, "yaml");

            Assert.Empty(result.Document.Servers);
        }

        [Fact]
        public void JustHostNoDefault()
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
            var result = OpenApiDocument.Parse(input, "yaml");

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("//www.foo.com", server.Url);
        }

        [Fact]
        public void NoBasePath()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);
            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("http://www.foo.com", server.Url);
        }

        [Fact]
        public void JustBasePathNoDefault()
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
            var result = OpenApiDocument.Parse(input, "yaml");

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("/baz", server.Url);
        }

        [Fact]
        public void JustSchemeWithCustomHost()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("http://bing.com/foo", server.Url);
        }

        [Fact]
        public void JustSchemeWithCustomHostWithEmptyPath()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("http://bing.com", server.Url);
        }

        [Fact]
        public void JustBasePathWithCustomHost()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("https://bing.com/api", server.Url);
        }

        [Fact]
        public void JustHostWithCustomHost()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("https://www.example.com", server.Url);
        }

        [Fact]
        public void JustHostWithCustomHostWithApi()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);
            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("https://prod.bing.com", server.Url);
        }

        [Fact]
        public void MultipleServers()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);
            var server = result.Document.Servers.First();
            Assert.Equal(2, result.Document.Servers.Count);
            Assert.Equal("http://dev.bing.com/api", server.Url);
            Assert.Equal("https://dev.bing.com/api", result.Document.Servers.Last().Url);
        }

        [Fact]
        public void LocalHostWithCustomHost()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);

            var server = result.Document.Servers.First();
            Assert.Single(result.Document.Servers);
            Assert.Equal("https://localhost:23232", server.Url);
        }

        [Fact]
        public void InvalidHostShouldYieldError()
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

            var result = OpenApiDocument.Parse(input, "yaml", settings);
            result.Document.Servers.Count.Should().Be(0);
            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                        new OpenApiError("#/", "Invalid host")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi2_0
                });
        }
    }
}
