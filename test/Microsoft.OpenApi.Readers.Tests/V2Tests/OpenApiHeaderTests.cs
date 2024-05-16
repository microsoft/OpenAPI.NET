// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Json.Schema;
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
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Number)
                                .Format("float")
                                .Default(5)
                },
                options => options
                .IgnoringCyclicReferences()
                .Excluding(x => x.Schema.BaseUri));
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
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Number)
                                .Format("float")
                                .Enum(7, 8, 9)
                }, options => options.IgnoringCyclicReferences());
        }
    }
}
