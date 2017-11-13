// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Text;
using FluentAssertions;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class BasicTests
    {
        [Fact]
        public void CheckOpenApiVersion()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.petstore30.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            openApiDoc.SpecVersion.ToString().Should().Be("3.0.0");
        }

        [Fact]
        public void InlineExample()
        {
            var openApiDoc = new OpenApiStringReader().Read(
                @"
                    openapi: 3.0.0
                    info:
                        title: A simple inline example
                        version: 1.0.0
                    paths:
                      /api/home:
                        get:
                          responses:
                            200:
                              description: A home document
                    ",
                out var parsingContext);

            openApiDoc.SpecVersion.ToString().Should().Be("3.0.0");
        }

        [Fact]
        public void ParseBrokenSimplest()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.BrokenSimplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            context.Errors.Count.Should().Be(1);
            context.Errors[0].ToString().Should().Be("title is a required property of #/info");
        }

        [Fact]
        public void ParseSimplestOpenApiEver()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.Simplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            openApiDoc.SpecVersion.ToString().Should().Be("1.0.0");
            openApiDoc.Paths.Should().BeEmpty();
            openApiDoc.Info.Title.Should().Be("The Api");
            openApiDoc.Info.Version.ToString().Should().Be("0.9.1");
            context.Errors.Should().BeEmpty();
        }

        [Fact]
        public void TestYamlBuilding()
        {
            var doc = new YamlDocument(
                new YamlMappingNode(
                    new YamlScalarNode("a"),
                    new YamlScalarNode("apple"),
                    new YamlScalarNode("b"),
                    new YamlScalarNode("banana"),
                    new YamlScalarNode("c"),
                    new YamlScalarNode("cherry")
                ));

            var s = new YamlStream();
            s.Documents.Add(doc);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            s.Save(writer);
            var output = sb.ToString();
        }
    }
}