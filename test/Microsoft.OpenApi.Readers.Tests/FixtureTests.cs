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
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Readers.Tests
{
    /// <summary>
    /// Load JSON and YAML file from the fixtures folder at the root of the solution, and
    /// verify that the deserializer products expected objects.
    /// </summary>
    /// <remarks>The embedding of the file is specified in the csproj of this test project.</remarks>
    public class FixtureTests
    {
        private readonly ITestOutputHelper _output;

        public FixtureTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private YamlNode LoadNode(string fileName)
        {
            using (var stream = GetType()
                .Assembly.GetManifestResourceStream(GetType(), fileName))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                return yamlStream.Documents.First().RootNode;
            }
        }

        [Fact]
        public void TestBasicInfoObject()
        {
            var yamlNode = LoadNode("basicInfoObject.json");

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
            var yamlNode = LoadNode("minimalInfoObject.json");

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
            var yamlNode = LoadNode("negativeInfoObject.json");

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);
            var info = OpenApiV3Deserializer.LoadInfo(node);

            Assert.NotNull(info);
            Assert.Equal(2, diagnostic.Errors.Count);
        }
    }
}