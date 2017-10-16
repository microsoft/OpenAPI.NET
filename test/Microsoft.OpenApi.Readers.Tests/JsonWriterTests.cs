using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;
using Microsoft.OpenApi.Writers;

namespace OpenApiTests
{
    public class JsonWriterTests
    {
        [Fact]
        public void WriteMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            writer.WriteStartObject();
            writer.WriteStringProperty("hello","world");
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndObject();
            writer.Flush();


            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("world", jObject["hello"]);
        }

        [Fact]
        public void WriteList()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            writer.WriteStartArray();
            writer.WriteValue("hello");
            writer.WriteValue("world");
            writer.WriteEndArray();
            writer.Flush();

            var jarray = JArray.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal(2, jarray.Count);
        }

        [Fact]
        public void WriteNestedMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
                writer.WriteStartObject();
                    writer.WritePropertyName("intro");
                    writer.WriteStartObject();
                    writer.WriteStringProperty("hello", "world");
                    writer.WriteEndObject();

                    writer.WritePropertyName("outro");
                    writer.WriteStartObject();
                    writer.WriteStringProperty("good", "bye");
                    writer.WriteEndObject();

                writer.WriteEndObject();
            writer.Flush();

            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("world", jObject["intro"]["hello"]);
        }

        [Fact]
        public void WriteNestedEmptyMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);

            writer.WriteStartObject();
            writer.WritePropertyName("intro");
            writer.WriteStartObject();
            writer.WriteEndObject();

            writer.WritePropertyName("outro");
            writer.WriteStartObject();
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndObject();

            writer.WriteEndObject();
            writer.Flush();

            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("bye", jObject["outro"]["good"]);
        }

    }
}
