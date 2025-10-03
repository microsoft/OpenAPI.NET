// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiHeaderTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiHeader/";

        [Fact]
        public void ParseBasicHeaderShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicHeader.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var header = OpenApiV3Deserializer.LoadHeader(node);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "The number of allowed requests in the current period",
                    Schema = new OpenApiSchema()
                    {
                        Type = "integer"
                    }
                }, header);
        }

        [Fact]
        public void ParseHeaderWithContentShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithContent.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var header = OpenApiV3Deserializer.LoadHeader(node);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "A complex header with content",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                                {
                                    ["timestamp"] = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        Format = "date-time"
                                    },
                                    ["value"] = new OpenApiSchema()
                                    {
                                        Type = "integer"
                                    }
                                }
                           }
                        }
                    }
                }, header);
        }

        [Fact]
        public void ParseHeaderWithMultipleContentTypesShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithMultipleContentTypes.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var header = OpenApiV3Deserializer.LoadHeader(node);

            // Assert
            Assert.Equivalent(
                new OpenApiHeader
                {
                    Description = "A header that accepts multiple content types",
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                                {
                                    ["data"] = new OpenApiSchema()
                                    {
                                        Type = "string"
                                    }
                                }
                           }
                        },
                        ["text/plain"] = new()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = "string"
                           }
                        }
                    }
                }, header);
        }

        [Fact]
        public void ParseHeaderWithStyleAndContentShouldPreferContent()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "headerWithStyleAndContent.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var header = OpenApiV3Deserializer.LoadHeader(node);

            // Assert
            // Both content and style can be present, content takes precedence for serialization behavior
            Assert.NotNull(header.Content);
            Assert.Single(header.Content);
            Assert.True(header.Content.ContainsKey("application/json"));
            Assert.Equal(ParameterStyle.Simple, header.Style); // Style can still be present
        }
    }
}
