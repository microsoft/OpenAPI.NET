using System.IO;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    public class OpenApiServerTests
    {
        [Fact]
        public void ParseServerWithNameShouldSucceed()
        {
            var input =
                """
                url: https://dev.example.com
                name: dev-server
                description: Development server
                """;
            
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiServer = OpenApiV32Deserializer.LoadServer(node, new());

            // Assert
            Assert.Equal("https://dev.example.com", openApiServer.Url);
            Assert.Equal("dev-server", openApiServer.Name);
            Assert.Equal("Development server", openApiServer.Description);
        }

        [Fact]
        public void ParseServerWithoutNameShouldSucceed()
        {
            var input =
                """
                url: https://example.com
                description: Sample server
                """;
            
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiServer = OpenApiV32Deserializer.LoadServer(node, new());

            // Assert
            Assert.Equal("https://example.com", openApiServer.Url);
            Assert.Null(openApiServer.Name);
            Assert.Equal("Sample server", openApiServer.Description);
        }
    }
}