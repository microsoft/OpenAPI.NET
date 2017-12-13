﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiEncoding";

        [Fact]
        public void ParseBasicEncodingShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "basicEncoding.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var encoding = OpenApiV3Deserializer.LoadEncoding(node);

                // Assert
                encoding.ShouldBeEquivalentTo(
                    new OpenApiEncoding
                    {
                        ContentType = "application/xml; charset=utf-8"
                    });
            }
        }

        [Fact]
        public void ParseAdvancedEncodingShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "advancedEncoding.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var encoding = OpenApiV3Deserializer.LoadEncoding(node);

                // Assert
                encoding.ShouldBeEquivalentTo(
                    new OpenApiEncoding
                    {
                        ContentType = "image/png, image/jpeg",
                        Headers =
                        {
                            ["X-Rate-Limit-Limit"] =
                            new OpenApiHeader
                            {
                                Description = "The number of allowed requests in the current period",
                                Schema = new OpenApiSchema
                                {
                                    Type = "integer"
                                }
                            }
                        }
                    });
            }
        }
    }
}