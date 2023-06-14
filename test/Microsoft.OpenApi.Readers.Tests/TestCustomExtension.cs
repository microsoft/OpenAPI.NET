﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class TestCustomExtension
    {
        [Fact]
        public void ParseCustomExtension()
        {
            var description = @"
openapi: 3.0.0
info: 
    title: A doc with an extension
    version: 1.0.0
    x-foo: 
        bar: hey
        baz: hi!
paths: {}
";
            var settings = new OpenApiReaderSettings()
            {
                ExtensionParsers = { { "x-foo", (a,v) => {
                        var fooNode = (JsonObject)a.Node;
                        return new FooExtension() {
                              Bar = (fooNode["bar"].ToString()),
                              Baz = (fooNode["baz"].ToString())
                        };
                } } }
            };

            var reader = new OpenApiStringReader(settings);

            var diag = new OpenApiDiagnostic();
            var doc = reader.Read(description, out diag);

            var fooExtension = doc.Info.Extensions["x-foo"] as FooExtension;
            //var fooExtension = JsonSerializer.Deserialize<FooExtension>(fooExtensionNode);

            fooExtension.Should().NotBeNull();
            fooExtension.Bar.Should().Be("hey");
            fooExtension.Baz.Should().Be("hi!");
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
