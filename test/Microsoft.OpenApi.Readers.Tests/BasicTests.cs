using Microsoft.OpenApi.Readers;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers.YamlReaders;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class BasicTests
    {
        [Fact]
        public void TestYamlBuilding()
        {
            var doc = new YamlDocument(new YamlMappingNode(
                            new YamlScalarNode("a"), new YamlScalarNode("apple"),
                            new YamlScalarNode("b"), new YamlScalarNode("banana"),
                            new YamlScalarNode("c"), new YamlScalarNode("cherry")
                    ));

            var s = new YamlStream();
            s.Documents.Add(doc);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            s.Save(writer);
            var output = sb.ToString();
        }

        [Fact]
        public void ParseSimplestOpenApiEver()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.Simplest.yaml");
            
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal("1.0.0", openApiDoc.Version);
            Assert.Empty(openApiDoc.Paths);
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version.ToString());
            Assert.Empty(context.ParseErrors);
        }

        [Fact]
        public void ParseBrokenSimplest()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(BasicTests),"Samples.BrokenSimplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal(2, context.ParseErrors.Count);
            Assert.NotNull(context.ParseErrors.Where(s=> s.ToString() == "`openapi` property does not match the required format major.minor.patch at #/openapi").FirstOrDefault());
            Assert.NotNull(context.ParseErrors.Where(s => s.ToString() == "title is a required property of #/info").FirstOrDefault());

        }

        [Fact]
        public void CheckOpenAPIVersion()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.petstore30.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal("3.0.0", openApiDoc.Version);

        }

        [Fact]
        public void InlineExample()
        {
            var openApiDoc = new OpenApiStringReader().Read(
                @"
                    openapi: 3.0.0
                    info:
                        title: A simple inline example
                        version: 1.0.0
                    paths:
                      /api/home:
                        get:
                          responses:
                            200:
                              description: A home document
                    ",
                out var parsingContext);

            Assert.Equal("3.0.0", openApiDoc.Version);

        }


    }
}
