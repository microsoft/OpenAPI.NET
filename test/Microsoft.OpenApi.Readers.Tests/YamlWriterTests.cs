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
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            writer.WriteStartObject();
            writer.WriteEndObject();
            

            //Assert.Equal(0, debug.StackState.Count);
            //Assert.Equal("", debug.Indent);
        }

        [Fact]
        public void WriteList()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WriteEndObject();
            writer.WriteEndArray();

            //Assert.Equal(0, debug.StackState.Count);
            //Assert.Equal("", debug.Indent);
        }


        }
    }
