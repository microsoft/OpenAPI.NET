using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenApiTests
{
    public class YamlWriterTests
    {
        [Fact]
        public void WriteMap()
        {
            var stream = new MemoryStream();
            var writer = new YamlParseNodeWriter(stream);

            writer.WriteStartDocument();
            writer.WriteStartMap();
            writer.WriteEndMap();
            writer.WriteEndDocument();
            var debug = writer.GetDebugInfo();

            Assert.Equal(0, debug.StackState.Count);
            Assert.Equal("", debug.Indent);
        }

        [Fact]
        public void WriteList()
        {
            var stream = new MemoryStream();
            var writer = new YamlParseNodeWriter(stream);

            writer.WriteStartDocument();
            writer.WriteStartList();
            writer.WriteStartMap();
            writer.WriteEndMap();
            writer.WriteEndList();
            writer.WriteEndDocument();
            var debug = writer.GetDebugInfo();

            Assert.Equal(0, debug.StackState.Count);
            Assert.Equal("", debug.Indent);
        }


        }
    }
