// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
    public class OpenApiDiscriminatorTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiDiscriminator/";

        [Fact]
        public void ParseBasicDiscriminatorShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicDiscriminator.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var discriminator = OpenApiV3Deserializer.LoadDiscriminator(node);

                // Assert
                discriminator.ShouldBeEquivalentTo(
                    new OpenApiDiscriminator
                    {
                        PropertyName = "pet_type",
                        Mapping =
                        {
                            ["puppy"] = "#/components/schemas/Dog",
                            ["kitten"] = "Cat"
                        }
                    });
            }
        }
    }
}