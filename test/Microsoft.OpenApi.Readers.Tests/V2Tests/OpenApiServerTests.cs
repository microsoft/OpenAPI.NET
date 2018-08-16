using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class OpenApiServerTests
    {
        [Fact]
        public void NoServer()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings() {
            });

            var doc = reader.Read(input, out var diagnostic);

            Assert.Empty(doc.Servers);
        }

        [Fact]
        public void JustSchemeNoDefault()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
schemes:
  - http
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            Assert.Equal(0, doc.Servers.Count);
        }

        [Fact]
        public void JustHostNoDefault()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
host: www.foo.com
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("//www.foo.com", server.Url);
        }

        [Fact]
        public void JustBasePathNoDefault()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
basePath: /baz
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("/baz", server.Url);
        }

        [Fact]
        public void JustSchemeWithCustomHost()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
schemes:
  - http
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://bing.com/foo")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("http://bing.com/foo", server.Url);
        }

        [Fact]
        public void JustSchemeWithCustomHostWithEmptyPath()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
schemes:
  - http
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("http://bing.com", server.Url);
        }

        [Fact]
        public void JustBasePathWithCustomHost()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
basePath: /api
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("https://bing.com/api", server.Url);
        }

        [Fact]
        public void JustHostWithCustomHost()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
host: www.example.com
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("https://www.example.com", server.Url);
        }

        [Fact]
        public void JustHostWithCustomHostWithApi()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
host: prod.bing.com
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://dev.bing.com/api")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("https://prod.bing.com/api", server.Url);
        }

        [Fact]
        public void MultipleServers()
        {
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
schemes:
  - http
  - https
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://dev.bing.com/api")
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
            var input = @"
swagger: 2.0
info: 
  title: test
  version: 1.0.0
host: localhost:23232
paths: {}
";
            var reader = new OpenApiStringReader(new OpenApiReaderSettings()
            {
                BaseUrl = new Uri("https://bing.com")
            });

            var doc = reader.Read(input, out var diagnostic);

            var server = doc.Servers.First();
            Assert.Equal(1, doc.Servers.Count);
            Assert.Equal("https://localhost:23232", server.Url);
        }
    }
}
