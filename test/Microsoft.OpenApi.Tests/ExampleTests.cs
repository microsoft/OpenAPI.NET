using Microsoft.OpenApi;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace OpenApi.Tests
{
    public class ExampleTests
    {
        [Fact] //Temporarily disabled until I create a JsonPointer
        public void WriteResponseExample()
        {
            var doc = new OpenApiDocument
            {
                Info = new Info()
                {
                    Title = "test",
                    Version = "1.0"
                }
            };

            doc.CreatePath("/test", 
                p => p.CreateOperation(OperationType.Get, 
                  o => o.CreateResponse("200", r =>
                 {
                     r.Description = "foo";
                     r.CreateContent("application/json", c =>
                     {
                         c.Example = "xyz"; ///"{ \"foo\": \"bar\" }"; This doesn't work because parser treats it as a node
                     });
                })));

            var stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;

            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));

            var yamlDocument = yamlStream.Documents.First();
            var rootNode = new RootNode(new ParsingContext(), yamlDocument);

            var node = rootNode.Find(new JsonPointer("/paths/~1test/get/responses/200/content/application~1json/example"));
            string example = node.GetScalarValue();

            Assert.Equal("xyz", example);
        }

        

    }

    public static class YamlExtensions
    {
        public static YamlMappingNode AsMap(this YamlNode node)
        {
            return node as YamlMappingNode;
        }
    }
}
