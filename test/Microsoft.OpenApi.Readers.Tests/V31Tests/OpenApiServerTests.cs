using System.IO;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiServerTests
    {
        [Fact]
        public void ParseServerWithXOaiNameExtensionShouldSucceed()
        {
            var input =
                """
                url: https://dev.example.com
                description: Development server
                x-oai-name: dev-server
                """;
            
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiServer = OpenApiV31Deserializer.LoadServer(node, new());

            // Assert
            Assert.Equal("https://dev.example.com", openApiServer.Url);
            Assert.Equal("dev-server", openApiServer.Name);
            Assert.Equal("Development server", openApiServer.Description);
            // The x-oai-name extension should not be in the extensions collection since it's parsed to Name property
            Assert.Null(openApiServer.Extensions);
        }

        [Fact]
        public void ParseServerWithOtherExtensionShouldKeepExtension()
        {
            var input =
                """
                url: https://example.com
                description: Sample server
                x-custom-extension: custom-value
                """;
            
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiServer = OpenApiV31Deserializer.LoadServer(node, new());

            // Assert
            Assert.Equal("https://example.com", openApiServer.Url);
            Assert.Null(openApiServer.Name);
            Assert.Equal("Sample server", openApiServer.Description);
            Assert.NotNull(openApiServer.Extensions);
            Assert.Single(openApiServer.Extensions);
            Assert.True(openApiServer.Extensions.ContainsKey("x-custom-extension"));
        }
    }
}