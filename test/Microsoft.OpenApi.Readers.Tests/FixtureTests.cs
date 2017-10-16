using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenApiTests
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
            var node = new MapNode(ctx, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Reader.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Equal("support@swagger.io", info.Contact.Email);
            Assert.Equal(0, ctx.ParseErrors.Count);
        }

        [Fact]
        public void TestMinimalInfoObject()
        {

            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/minimalInfoObject.json");

            var ctx = new ParsingContext();
            var node = new MapNode(ctx, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Reader.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Equal(0, ctx.ParseErrors.Count);
        }

        [Fact]
        public void TestNegativeInfoObject()
        {

            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/negative/negativeInfoObject.json");

            var ctx = new ParsingContext();
            var node = new MapNode(ctx, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Reader.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal(2, ctx.ParseErrors.Count);
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

