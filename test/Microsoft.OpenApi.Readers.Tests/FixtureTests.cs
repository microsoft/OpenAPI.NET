// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class FixtureTests
    {
        private YamlNode LoadNode(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                return yamlStream.Documents.First().RootNode;
            }
        }
        // Load json,yaml for both 2.0 and 3.0.  Ensure resulting DOM is not empty and equivalent?
        // Load files from ../../fixtures/(v3.0|v2.0|v1.0 ???)/(json|yaml)/general/basicInfoObject.json

        [Fact]
        public void TestBasicInfoObject()
        {
            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/basicInfoObject.json");

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Equal("support@swagger.io", info.Contact.Email);
            Assert.Empty(diagnostic.Errors);
        }

        [Fact]
        public void TestMinimalInfoObject()
        {
            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/minimalInfoObject.json");

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal("Swagger Sample App", info.Title);
            Assert.Equal("1.0.1", info.Version.ToString());
            Assert.Empty(diagnostic.Errors);
        }

        [Fact]
        public void TestNegativeInfoObject()
        {
            var yamlNode = LoadNode("../../../../fixtures/v3.0/json/general/negative/negativeInfoObject.json");

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal(2, diagnostic.Errors.Count);
        }
    }
}