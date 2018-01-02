// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiExample/";

        [Fact]
        public void ParseAdvancedExampleShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedExample.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                var example = OpenApiV3Deserializer.LoadExample(node);

                diagnostic.Errors.Should().BeEmpty();

                example.ShouldBeEquivalentTo(
                    new OpenApiExample
                    {
                        Value = new OpenApiObject
                        {
                            ["versions"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("Status1"),
                                    ["id"] = new OpenApiString("v1"),
                                    ["links"] = new OpenApiArray
                                    {
                                        new OpenApiObject
                                        {
                                            ["href"] = new OpenApiString("http://example.com/1"),
                                            ["rel"] = new OpenApiString("sampleRel1")
                                        }
                                    }
                                },

                                new OpenApiObject
                                {
                                    ["status"] = new OpenApiString("Status2"),
                                    ["id"] = new OpenApiString("v2"),
                                    ["links"] = new OpenApiArray
                                    {
                                        new OpenApiObject
                                        {
                                            ["href"] = new OpenApiString("http://example.com/2"),
                                            ["rel"] = new OpenApiString("sampleRel2")
                                        }
                                    }
                                }
                            }
                        }
                    });
            }
        }
    }
}