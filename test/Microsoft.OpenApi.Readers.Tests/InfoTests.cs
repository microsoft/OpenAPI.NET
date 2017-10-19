using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class InfoTests
    {


        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.petstore30.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var info = openApiDoc.Info;
            Assert.Equal("Swagger Petstore (Simple)", openApiDoc.Info.Title);
            Assert.Equal("A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification", info.Description);
            Assert.Equal("1.0.0", info.Version.ToString());

        }


        [Fact]
        public void ParseCompleteHeaderOpenApi()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream(typeof(InfoTests), "Samples.CompleteHeader.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal("1.0.0", openApiDoc.Version);

            Assert.Empty(openApiDoc.Paths);
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version.ToString());
            Assert.Equal("This is an api", openApiDoc.Info.Description);
            Assert.Equal("http://example.org/Dowhatyouwant", openApiDoc.Info.TermsOfService);
            Assert.Equal("Darrel Miller", openApiDoc.Info.Contact.Name);
         //   Assert.Equal("@darrel_miller", openApiDoc.Info.Contact.Extensions["x-twitter"].GetValueNode().GetScalarValue());
        }

    }
}
