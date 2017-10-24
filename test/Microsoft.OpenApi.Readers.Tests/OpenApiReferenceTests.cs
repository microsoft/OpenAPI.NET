

using System;
using Microsoft.OpenApi.Readers.YamlReaders;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
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
            
            var reference = OpenApiV2Translator.ParseReference("#/definitions/blahblah");

            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal(string.Empty, reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }

        [Fact]
        public void TranslateV2LocalReference()
        {

            var reference = OpenApiV2Translator.ParseReference("blahblah");

            Assert.Equal(ReferenceType.Schema, reference.ReferenceType);
            Assert.Equal(string.Empty, reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }

        [Fact]
        public void TranslateV2ExternalReference()
        {

            var reference = OpenApiV2Translator.ParseReference("swagger.json#/parameters/blahblah");

            Assert.Equal(ReferenceType.Parameter, reference.ReferenceType);
            Assert.Equal("swagger.json", reference.ExternalFilePath);
            Assert.Equal("blahblah", reference.TypeName);
        }
    }
}
