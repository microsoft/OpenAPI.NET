// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V2;
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
                    Schema = new()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float",
                        Default = new OpenApiAny(5).Node
                    }
                },
                options => options
                .IgnoringCyclicReferences()
                .Excluding(x => x.Schema.Default.Parent));
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

            // Assert
            header.Should().BeEquivalentTo(
                new OpenApiHeader
                {
                    Schema = new()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float",
                        Enum =
                        {
                            new OpenApiAny(7).Node,
                            new OpenApiAny(8).Node,
                            new OpenApiAny(9).Node
                        }
                    }
                }, options => options.IgnoringCyclicReferences()
                                     .Excluding((IMemberInfo memberInfo) =>
                                        memberInfo.Path.EndsWith("Parent")));
        }
    }
}
