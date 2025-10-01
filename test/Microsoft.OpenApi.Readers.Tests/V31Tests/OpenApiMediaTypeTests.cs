// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiMediaType/";

        [Fact]
        public async Task ParseMediaTypeWithExampleShouldSucceed()
        {
            // Act
            var mediaType = await OpenApiModelFactory.LoadAsync<OpenApiMediaType>(Path.Combine(SampleFolderPath, "mediaTypeWithExample.yaml"), OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Example = 5,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(m => m.Example.Parent)
                );
        }

        [Fact]
        public async Task ParseMediaTypeWithExamplesShouldSucceed()
        {
            // Act
            var mediaType = await OpenApiModelFactory.LoadAsync<OpenApiMediaType>(Path.Combine(SampleFolderPath, "mediaTypeWithExamples.yaml"), OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            // Assert
            mediaType.Should().BeEquivalentTo(
                new OpenApiMediaType
                {
                    Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["example1"] = new OpenApiExample()
                        {
                            Value = 5
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value = 7.5
                        }
                    },
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(m => m.Examples["example1"].Value.Parent)
                .Excluding(m => m.Examples["example2"].Value.Parent));
        }

        [Fact]
        public async Task ParseMediaTypeWithEmptyArrayInExamplesWorks()
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
      ""summary"": ""empty array summary"",
      ""description"": ""empty array description"",
      ""value"": [ ]
    },
    ""Success response - with results"": {
      ""summary"": ""array summary"",
      ""description"": ""array description"",
      ""value"": [ 
        1
      ]
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
            var mediaType = OpenApiV31Deserializer.LoadMediaType(node, new());
            var serialized = await mediaType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), serialized.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task ParseMediaTypeWithXOaiItemSchemaShouldSucceed()
        {
            // Act
            var mediaType = await OpenApiModelFactory.LoadAsync<OpenApiMediaType>(
                Path.Combine(SampleFolderPath, "mediaTypeWithXOaiItemSchema.yaml"),
                OpenApiSpecVersion.OpenApi3_1,
                new(),
                SettingsFixture.ReaderSettings);

            // Assert
            var expected = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array
                },
                ItemSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    MaxLength = 100
                }
            };
            Assert.Equivalent(expected, mediaType);
        }
    }
}
