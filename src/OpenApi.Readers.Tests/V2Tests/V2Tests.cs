using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Xunit;
using Microsoft.OpenApi.Readers;

namespace OpenApiTests.V2Tests
{
    public class V2Tests
    {
        [Theory, 
            InlineData("simplest"),
            InlineData("host")]
        public void Tests(string filename)
        {
            var v2stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.V2Tests.V2Samples.simplest.2.yaml");
            var v3stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.V2Tests.V2Samples.simplest.3.yaml");

            var openApiDoc2 = OpenApiParser.Parse(v2stream).OpenApiDocument;

            Assert.True(AreStreamsEqual(v2stream, v3stream));

        }

        private bool AreStreamsEqual(Stream expected, Stream actual)
        {
            var reader1 = new StreamReader(expected);
            var reader2 = new StreamReader(expected);

            return reader1.ReadToEnd() == reader2.ReadToEnd();
        }
    }
}
