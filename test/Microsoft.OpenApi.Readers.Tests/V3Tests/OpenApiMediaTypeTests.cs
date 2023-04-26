// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiMediaType/";

        [Fact]
        public void ParseMediaTypeWithExampleShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "mediaTypeWithExample.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var mediaType = OpenApiV3Deserializer.LoadMediaType(node);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Example = 5.0,
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float"
                    }
                });
        }

        [Fact]
        public void ParseMediaTypeWithExamplesShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "mediaTypeWithExamples.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var mediaType = OpenApiV3Deserializer.LoadMediaType(node);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Examples =
                    {
                        ["example1"] = new OpenApiExample()
                        {
                            Value = 5.0,
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value = (float)7.5,
                        }
                    },
                    Schema = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "float"
                    }
                });
        }
    }
}
