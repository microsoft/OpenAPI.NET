using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;


namespace Microsoft.OpenApi.Readers.Tests
{
    public class SchemaTests
    {

        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(SchemaTests), "Samples.petstore30.yaml");

            var openApiDoc = OpenApiParser.Parse(stream).OpenApiDocument;
            var operation = openApiDoc.Paths["/pets"].Operations["get"];
            var schema = operation.Responses["200"].Content["application/json"].Schema;
            Assert.NotNull(schema);

        }

        [Fact]
        public void CreateSchemaFromInlineJsonSchema()
        {
            var jsonSchema = " { \"type\" : \"int\" } ";

            var mapNode = MapNode.Create(jsonSchema);

            var schema = OpenApiV3Reader.LoadSchema(mapNode);

            Assert.NotNull(schema);
            Assert.Equal("int", schema.Type);
        }
    }
}
