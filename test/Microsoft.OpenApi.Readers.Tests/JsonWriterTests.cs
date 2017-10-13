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
            var outputStream = new MemoryStream();
            var writer = new JsonParseNodeWriter(outputStream);
            writer.WriteStartDocument();
            writer.WriteStartMap();
            writer.WriteStringProperty("hello","world");
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndMap();
            writer.WriteEndDocument();
            writer.Flush();

            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);

            Assert.Equal("world", jObject["hello"]);
        }

        [Fact]
        public void WriteList()
        {
            var outputStream = new MemoryStream();
            var writer = new JsonParseNodeWriter(outputStream);
            writer.WriteStartDocument();
            writer.WriteStartList();
            writer.WriteListItem("hello", (w,s) => w.WriteValue(s));
            writer.WriteListItem("world", (w, s) => w.WriteValue(s));
            writer.WriteEndList();
            writer.WriteEndDocument();
            writer.Flush();

            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jarray = JArray.Parse(json);

            Assert.Equal(2, jarray.Count);
        }

        [Fact]
        public void WriteNestedMap()
        {
            var outputStream = new MemoryStream();
            var writer = new JsonParseNodeWriter(outputStream);
            writer.WriteStartDocument();
                writer.WriteStartMap();
                    writer.WritePropertyName("intro");
                    writer.WriteStartMap();
                    writer.WriteStringProperty("hello", "world");
                    writer.WriteEndMap();

                    writer.WritePropertyName("outro");
                    writer.WriteStartMap();
                    writer.WriteStringProperty("good", "bye");
                    writer.WriteEndMap();

                writer.WriteEndMap();
            writer.WriteEndDocument();
            writer.Flush();

            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);

            Assert.Equal("world", jObject["intro"]["hello"]);
        }

        [Fact]
        public void WriteNestedEmptyMap()
        {
            var outputStream = new MemoryStream();
            var writer = new JsonParseNodeWriter(outputStream);
            writer.WriteStartDocument();
            writer.WriteStartMap();
            writer.WritePropertyName("intro");
            writer.WriteStartMap();
            writer.WriteEndMap();

            writer.WritePropertyName("outro");
            writer.WriteStartMap();
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndMap();

            writer.WriteEndMap();
            writer.WriteEndDocument();
            writer.Flush();

            outputStream.Position = 0;
            var json = new StreamReader(outputStream).ReadToEnd();
            var jObject = JObject.Parse(json);

            Assert.Equal("bye", jObject["outro"]["good"]);
        }

    }
}
