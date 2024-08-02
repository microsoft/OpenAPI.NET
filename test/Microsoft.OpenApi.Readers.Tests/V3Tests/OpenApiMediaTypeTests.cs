// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Microsoft.OpenApi.Tests;
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
                    Example = new OpenApiFloat(5),
                    Schema = new()
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
                        ["example1"] = new()
                        {
                            Value = new OpenApiFloat(5),
                        },
                        ["example2"] = new()
                        {
                            Value = new OpenApiFloat((float)7.5),
                        }
                    },
                    Schema = new()
                    {
                        Type = "number",
                        Format = "float"
                    }
                });
        }

        [Fact]
        public void ParseMediaTypeWithEmptyArrayInExamplesWorks()
        {
            // Arrange
            var expected = @"{
  ""schema"": {
    ""type"": ""array"",
    ""items"": {
      ""type"": ""object"",
      ""properties"": {
        ""id"": {
          ""type"": ""string""
        }
      }
    }
  },
  ""examples"": {
    ""Success response - no results"": {
      ""value"": [ ]
    }
  }
}
";
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "examplesWithEmptyArray.json")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var mediaType = OpenApiV3Deserializer.LoadMediaType(node);
            var serialized = mediaType.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            serialized.MakeLineBreaksEnvironmentNeutral()
                .Should().BeEquivalentTo(expected.MakeLineBreaksEnvironmentNeutral());
        }
    }
}
