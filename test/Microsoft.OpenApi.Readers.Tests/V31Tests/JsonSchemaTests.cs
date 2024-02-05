using System.IO;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.Tests;
using Microsoft.OpenApi.Reader.V31;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class JsonSchemaTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiSchema/";

        [Fact]
        public void ParseV31SchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "schema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV31Deserializer.LoadSchema(node);
            var jsonString = @"{
   ""type"": ""object"",
   ""properties"": {
      ""one"": {
         ""description"": ""type array"",
         ""type"": [
            ""integer"",
            ""string""
         ]
      }
   }
}";
            var expectedSchema = JsonSerializer.Deserialize<JsonSchema>(jsonString);

            // Assert
            Assert.Equal(schema, expectedSchema);
        }

        [Fact]
        public void ParseAdvancedV31SchemaShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedSchema.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var schema = OpenApiV31Deserializer.LoadSchema(node);
            var jsonString = @"{
   ""type"": ""object"",
   ""properties"": {
      ""one"": {
         ""description"": ""type array"",
         ""type"": [
            ""integer"",
            ""string""
         ]
      },
      ""two"": {
         ""description"": ""type 'null'"",
         ""type"": ""null""
      },
      ""three"": {
         ""description"": ""type array including 'null'"",
         ""type"": [
            ""string"",
            ""null""
         ]
      },
      ""four"": {
         ""description"": ""array with no items"",
         ""type"": ""array""
      },
      ""five"": {
         ""description"": ""singular example"",
         ""type"": ""string"",
         ""examples"": [
            ""exampleValue""
         ]
      },
      ""six"": {
         ""description"": ""exclusiveMinimum true"",
         ""exclusiveMinimum"": 10
      },
      ""seven"": {
         ""description"": ""exclusiveMinimum false"",
         ""minimum"": 10
      },
      ""eight"": {
         ""description"": ""exclusiveMaximum true"",
         ""exclusiveMaximum"": 20
      },
      ""nine"": {
         ""description"": ""exclusiveMaximum false"",
         ""maximum"": 20
      },
      ""ten"": {
         ""description"": ""nullable string"",
         ""type"": [
            ""string"",
            ""null""
         ]
      },
      ""eleven"": {
         ""description"": ""x-nullable string"",
         ""type"": [
            ""string"",
            ""null""
         ]
      },
      ""twelve"": {
         ""description"": ""file/binary""
      }
   }
}";
            var expectedSchema = JsonSerializer.Deserialize<JsonSchema>(jsonString);

            // Assert
            schema.Should().BeEquivalentTo(expectedSchema);
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
