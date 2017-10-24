using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Readers.YamlReaders;
using Xunit;
using Microsoft.OpenApi.Readers.YamlReaders.ParseNodes;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class SchemaTests
    {

        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(SchemaTests), "Samples.petstore30.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var operation = openApiDoc.Paths["/pets"].Operations["get"];
            var schema = operation.Responses["200"].Content["application/json"].Schema;
            Assert.NotNull(schema);

        }

        [Fact]
        public void CreateSchemaFromInlineJsonSchema()
        {
            var jsonSchema = " { \"type\" : \"int\" } ";

            var context = new ParsingContext();
            var log = new OpenApiDiagnostic();

            var mapNode = new MapNode(context, log, jsonSchema);

            var schema = OpenApiV3Deserializer.LoadSchema(mapNode);

            Assert.NotNull(schema);
            Assert.Equal("int", schema.Type);
        }
    }
}
