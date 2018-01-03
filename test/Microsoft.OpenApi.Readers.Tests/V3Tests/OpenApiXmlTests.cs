// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiXmlTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiXml/";

        [Fact]
        public void ParseBasicXmlShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicXml.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var xml = OpenApiV3Deserializer.LoadXml(node);

                // Assert
                xml.ShouldBeEquivalentTo(
                    new OpenApiXml
                    {
                        Name = "name1",
                        Namespace = new Uri("http://example.com/schema/namespaceSample"),
                        Prefix = "samplePrefix",
                        Wrapped = true
                    });
            }
        }
    }
}