

namespace OpenApiTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OpenApi;
    using Xunit;
    using Microsoft.OpenApi.Readers;

    public class OpenApiReferenceTests
    {
        [Fact]
        public void ParseLocalParameterReference()
        {
            var reference = new OpenApiReference("#/components/parameters/foobar");

            Assert.Equal(ReferenceType.Parameter, reference.ReferenceType);
            Assert.Equal(String.Empty, reference.ExternalFilePath);
            Assert.Equal("foobar", reference.TypeName);
        }

        [Fact]
        public void ParseLocalSchemaReference()
        {
            var reference = new OpenApiReference("foobar");

            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal(String.Empty, reference.ExternalFilePath);
            Assert.Equal("foobar", reference.TypeName);
        }

        [Fact]
        public void ParseExternalHeaderReference()
        {
            var reference = new OpenApiReference("externalschema.json#/components/headers/blah");

            Assert.Equal(ReferenceType.Header, reference.ReferenceType);
            Assert.Equal("externalschema.json", reference.ExternalFilePath);
            Assert.Equal("blah", reference.TypeName);
        }

        [Fact]
        public void TranslateV2Reference()
        {
            
            var reference = OpenApiV2Builder.ParseReference("#/definitions/blahblah");

            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal(string.Empty, reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }

        [Fact]
        public void TranslateV2LocalReference()
        {

            var reference = OpenApiV2Builder.ParseReference("blahblah");

            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal(string.Empty, reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }

        [Fact]
        public void TranslateV2ExternalReference()
        {

            var reference = OpenApiV2Builder.ParseReference("swagger.json#/parameters/blahblah");

            Assert.Equal(ReferenceType.Parameter, reference.ReferenceType);
            Assert.Equal("swagger.json", reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }
    }
}
