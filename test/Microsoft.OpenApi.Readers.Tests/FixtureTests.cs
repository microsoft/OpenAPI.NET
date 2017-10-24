using System.IO;
using System.Linq;
using Microsoft.OpenApi.Readers.YamlReaders;
using Microsoft.OpenApi.Readers.YamlReaders.ParseNodes;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class FixtureTests
    {
        // Load json,yaml for both 2.0 and 3.0.  Ensure resulting DOM is not empty and equivalent?
        // Load files from ../../fixtures/(v3.0|v2.0|v1.0 ???)/(json|yaml)/general/basicInfoObject.json

        [Fact]
        public void TestBasicInfoObject()
        {

            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/basicInfoObject.json");

            var ctx = new ParsingContext();
            var log = new OpenApiDiagnostic();

            var node = new MapNode(ctx, log, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Equal("support@swagger.io", info.Contact.Email);
            Assert.Empty(log.Errors);
        }

        [Fact]
        public void TestMinimalInfoObject()
        {

            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/minimalInfoObject.json");

            var ctx = new ParsingContext();
            var log = new OpenApiDiagnostic();
            
            var node = new MapNode(ctx, log, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Empty(log.Errors);
        }

        [Fact]
        public void TestNegativeInfoObject()
        {

            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/negative/negativeInfoObject.json");

            var ctx = new ParsingContext();
            var log = new OpenApiDiagnostic();

            var node = new MapNode(ctx, log, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal(2, log.Errors.Count);
        }

        private YamlNode LoadNode(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {

                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                return yamlStream.Documents.First().RootNode;
            }
        }
    }
}

