// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using System.IO;
using Xunit;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Readers.V31;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{

    public class OpenApiLicenseTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiLicense/";

        [Fact]
        public void ParseLicenseWithSpdxIdentifierShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "licenseWithSpdxIdentifier.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var license = OpenApiV31Deserializer.LoadLicense(node);

            // Assert
            license.Should().BeEquivalentTo(
                new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Identifier = "Apache-2.0"
                });
        }
    }
}
