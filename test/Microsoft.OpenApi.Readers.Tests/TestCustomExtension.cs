// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class TestCustomExtension
    {
        [Fact]
        public void ParseCustomExtension()
        {
            var description =
                """
                openapi: 3.0.0
                info:
                    title: A doc with an extension
                    version: 1.0.0
                    x-foo:
                        bar: hey
                        baz: hi!
                paths: {}
                """;
            var settings = new OpenApiReaderSettings
            {
                ExtensionParsers = { { "x-foo", (a,v) => {
                        var fooNode = (JsonObject)a;
                        return new FooExtension() {
                              Bar = (fooNode["bar"].ToString()),
                              Baz = (fooNode["baz"].ToString())
                        };
                } } }
            };
            settings.AddYamlReader();

            var diag = new OpenApiDiagnostic();
            var actual = OpenApiDocument.Parse(description, "yaml", settings: settings);

            var fooExtension = actual.Document.Info.Extensions["x-foo"] as FooExtension;

            Assert.NotNull(fooExtension);
            Assert.Equal("hey", fooExtension.Bar);
            Assert.Equal("hi!", fooExtension.Baz);
        }

        [Fact]
        public void ExtensionParserThrowingOpenApiException_V2_ShouldHaveCorrectPointer()
        {
            var json = """
{
  "swagger": "2.0",
  "info": {
    "title": "Demo",
    "version": "1"
  },
  "paths": {},
  "definitions": {
    "demo": {
      "x-tag": null
    }
  }
}
""";
            var settings = new OpenApiReaderSettings
            {
                ExtensionParsers =
                {
                    { "x-tag", (any, version) => throw new OpenApiException("Testing") }
                }
            };

            var result = OpenApiDocument.Parse(json, "json", settings);

            Assert.NotNull(result.Diagnostic);
            Assert.NotEmpty(result.Diagnostic.Errors);
            var error = result.Diagnostic.Errors[0];
            Assert.Equal("Testing", error.Message);
            Assert.Equal("#/definitions/demo/x-tag", error.Pointer);
        }

        [Fact]
        public void ExtensionParserThrowingOpenApiException_V3_ShouldHaveCorrectPointer()
        {
            var json = """
{
  "openapi": "3.0.0",
  "info": {
    "title": "Demo",
    "version": "1"
  },
  "paths": {},
  "components": {
    "schemas": {
      "demo": {
        "x-tag": null
      }
    }
  }
}
""";
            var settings = new OpenApiReaderSettings
            {
                ExtensionParsers =
                {
                    { "x-tag", (any, version) => throw new OpenApiException("Testing") }
                }
            };

            var result = OpenApiDocument.Parse(json, "json", settings);

            Assert.NotNull(result.Diagnostic);
            Assert.NotEmpty(result.Diagnostic.Errors);
            var error = result.Diagnostic.Errors[0];
            Assert.Equal("Testing", error.Message);
            Assert.Equal("#/components/schemas/demo/x-tag", error.Pointer);
        }

        [Fact]
        public void ExtensionParserThrowingOpenApiException_V31_ShouldHaveCorrectPointer()
        {
            var json = """
{
  "openapi": "3.1.0",
  "info": {
    "title": "Demo",
    "version": "1"
  },
  "components": {
    "schemas": {
      "demo": {
        "x-tag": null
      }
    }
  }
}
""";
            var settings = new OpenApiReaderSettings
            {
                ExtensionParsers =
                {
                    { "x-tag", (any, version) => throw new OpenApiException("Testing") }
                }
            };

            var result = OpenApiDocument.Parse(json, "json", settings);

            Assert.NotNull(result.Diagnostic);
            Assert.NotEmpty(result.Diagnostic.Errors);
            var error = result.Diagnostic.Errors[0];
            Assert.Equal("Testing", error.Message);
            Assert.Equal("#/components/schemas/demo/x-tag", error.Pointer);
        }
    }

    internal class FooExtension : IOpenApiExtension, IOpenApiElement
    {
        public string Baz { get; set; }

        public string Bar { get; set; }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            writer.WriteProperty("baz", Baz);
            writer.WriteProperty("bar", Bar);
            writer.WriteEndObject();
        }
    }
}
