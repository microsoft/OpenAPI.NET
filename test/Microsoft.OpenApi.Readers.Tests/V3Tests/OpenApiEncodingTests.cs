// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiEncoding/";

        [Fact]
        public void ParseBasicEncodingShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicEncoding.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

            // Act
            var encoding = OpenApiV3Deserializer.LoadEncoding(node);

            // Assert
            encoding.Should().BeEquivalentTo(
                new OpenApiEncoding
                {
                    ContentType = "application/xml; charset=utf-8"
                });
        }

        [Fact]
        public void ParseAdvancedEncodingShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedEncoding.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

            // Act
            var encoding = OpenApiV3Deserializer.LoadEncoding(node);

            // Assert
            encoding.Should().BeEquivalentTo(
                new OpenApiEncoding
                {
                    ContentType = "image/png, image/jpeg",
                    Headers =
                    {
                        ["X-Rate-Limit-Limit"] =
                            new()
                            {
                                Description = "The number of allowed requests in the current period",
                                Schema = new JsonSchemaBuilder().Type(SchemaValueType.Integer)
                            }
                    }
                });
        }
    }
}
