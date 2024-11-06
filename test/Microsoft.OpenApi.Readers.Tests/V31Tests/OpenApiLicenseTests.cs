// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V31;
using SharpYaml.Serialization;
using Xunit;

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

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

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
