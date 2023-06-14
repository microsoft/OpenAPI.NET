// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiHeaderTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiHeader/";

        [Fact]
        public void ParseHeaderWithDefaultShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithDefault.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var header = OpenApiV2Deserializer.LoadHeader(node);

            // Assert
            header.Should().BeEquivalentTo(
                new OpenApiHeader
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = "number",
                        Format = "float",
                        Default = new OpenApiAny(5)
                    }
                }, 
                options => options
                .IgnoringCyclicReferences()
                .Excluding(header => header.Schema.Default.Node.Parent));
        }

        [Fact]
        public void ParseHeaderWithEnumShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithEnum.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var header = OpenApiV2Deserializer.LoadHeader(node);
            var parent = header.Schema.Enum.Select(e => e.Node.Parent);
            
            // Assert
            header.Should().BeEquivalentTo(
                new OpenApiHeader
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = "number",
                        Format = "float",
                        Enum =
                        {
                            new OpenApiAny(7),
                            new OpenApiAny(8),
                            new OpenApiAny(9)
                        }
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(header => header.Schema.Enum[0].Node.Parent)
                .Excluding(header => header.Schema.Enum[1].Node.Parent)
                .Excluding(header => header.Schema.Enum[2].Node.Parent));
        }
    }
}
