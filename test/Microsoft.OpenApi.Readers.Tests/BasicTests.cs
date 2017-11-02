// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Readers.YamlReaders;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class BasicTests
    {
        [Fact]
        public void CheckOpenAPIVersion()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.petstore30.yaml");
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal("3.0.0", openApiDoc.SpecVersion.ToString());
        }

        [Fact]
        public void InlineExample()
        {
            var openApiDoc = new OpenApiStringReader().Read(
                
@"                    openapi: 3.0.0
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

            Assert.Equal("3.0.0", openApiDoc.SpecVersion.ToString());
        }

        [Fact]
        public void ParseBrokenSimplest()
        {
            var stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.BrokenSimplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal(1, context.Errors.Count);
              Assert.NotNull(
                context.Errors.Where(s => s.ToString() == "title is a required property of #/info").FirstOrDefault());
        }

        [Fact]
        public void ParseSimplestOpenApiEver()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(BasicTests), "Samples.Simplest.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            Assert.Equal("1.0.0", openApiDoc.SpecVersion.ToString());
            Assert.Empty(openApiDoc.Paths);
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version.ToString());
            Assert.Empty(context.Errors);
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