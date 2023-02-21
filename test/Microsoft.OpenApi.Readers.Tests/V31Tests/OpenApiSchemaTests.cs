using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiSchemaTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/";

        [Fact]
        public void ParseV3SchemaShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schema.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var node = new MapNode(context, (YamlMappingNode)yamlNode);

                // Act
                var schema = OpenApiV31Deserializer.LoadSchema(node);

                // Assert
                //diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

                //schema.Should().BeEquivalentTo(
                //    new OpenApiSchema
                //    {
                //        Type = "string",
                //        Format = "email"
                //    });
            }
        }

        [Fact]
        public void ParseStandardSchemaExampleSucceeds()
        {
            // Arrange
            var builder = new JsonSchemaBuilder();
            var myschema = builder.Title("My Schema")
                .Description("A schema for testing")
                .Type(SchemaValueType.Object)
                .Properties(
                ("name",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                        .Description("The name of the person")),
                ("age",
                    new JsonSchemaBuilder()
                        .Type(SchemaValueType.Integer)
                        .Description("The age of the person")))
                .Build();

            // Act
            var title = myschema.Get<TitleKeyword>().Value;    
            var description = myschema.Get<DescriptionKeyword>().Value;
            var nameProperty = myschema.Get<PropertiesKeyword>().Properties["name"];

            // Assert
            Assert.Equal("My Schema", title);
            Assert.Equal("A schema for testing", description);
        }
    }

    public static class SchemaExtensions
    {
        public static T Get<T>(this JsonSchema schema)
        {
            return (T)schema.Keywords.FirstOrDefault(x => x is T);
        }
    }
}
