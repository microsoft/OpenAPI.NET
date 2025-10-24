// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V32;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{

    public class OpenApiLicenseTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiLicense/";

        [Fact]
        public void ParseLicenseWithSpdxIdentifierShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "licenseWithSpdxIdentifier.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents[0].RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var license = OpenApiV32Deserializer.LoadLicense(node, new());

            // Assert
            Assert.Equivalent(
                new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Identifier = "Apache-2.0"
                }, license);
        }
    }
}

