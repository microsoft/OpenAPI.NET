using FluentAssertions;
using Microsoft.OpenApi.Models;
using System.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiServerTests
    {
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
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            Assert.Empty(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            Assert.Empty(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var _);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://www.foo.com/spec.yaml")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com/foo")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://dev.bing.com/api/description.yaml")
            });

            var doc = reader.Read(input, out var _);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://dev.bing.com/api")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(2, doc.Servers.Count);
            Assert.Equal("http://dev.bing.com/api", server.Url);
            Assert.Equal("https://dev.bing.com/api", doc.Servers.Last().Url);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
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
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);
            doc.Servers.Count.Should().Be(0);
            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic
                {
                    Errors =
                    {
                        new OpenApiError("#/", "Invalid host")
                    },
                    SpecificationVersion = OpenApiSpecVersion.OpenApi2_0
                });
        }

        [Fact]
        public void BaseUrlWithPortShouldPreservePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("http://demo.testfire.net:8080")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("http://demo.testfire.net:8080", server.Url);
        }

        [Fact]
        public void BaseUrlWithPortAndPathShouldPreservePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("http://demo.testfire.net:8080/swagger/properties.json")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("http://demo.testfire.net:8080/swagger/properties.json", server.Url);
        }

        [Fact]
        public void BaseUrlWithNonStandardPortShouldPreservePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://api.example.com:9443/v1/openapi.yaml")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("https://api.example.com:9443/v1/openapi.yaml", server.Url);
        }

        [Fact]
        public void BaseUrlWithStandardHttpsPortShouldRemovePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("https://foo.bar:443/api")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("https://foo.bar/api", server.Url);
        }

        [Fact]
        public void BaseUrlWithStandardHttpPortShouldRemovePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
                BaseUrl = new("http://foo.bar:80/api")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("http://foo.bar/api", server.Url);
        }

        [Fact]
        public void HostWithStandardHttpsPortShouldRemovePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: foo.bar:443
                schemes:
                  - https
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("https://foo.bar", server.Url);
        }

        [Fact]
        public void HostWithStandardHttpPortShouldRemovePort()
        {
            var input =
                """
                swagger: 2.0
                info:
                  title: test
                  version: 1.0.0
                host: foo.bar:80
                schemes:
                  - http
                paths: {}
                """;
            var reader = new OpenApiStringReader(new()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Single(doc.Servers);
            Assert.Equal("http://foo.bar", server.Url);
        }
    }
}
