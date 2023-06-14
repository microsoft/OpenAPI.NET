﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using System.IO;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{

    public class OpenApiLicenseTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiLicense/";

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
            var license = OpenApiV3Deserializer.LoadLicense(node);

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
