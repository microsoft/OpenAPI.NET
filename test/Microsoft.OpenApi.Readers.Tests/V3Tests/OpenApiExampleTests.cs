// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
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
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedExample.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);

                var example = OpenApiV3Deserializer.LoadExample(node);
                var expected = new OpenApiExample
                {
                    Value = new OpenApiAny(new JsonObject
                    {
                        ["versions"] = new JsonArray
                            {
                                new JsonObject
                                {
                                    ["status"] = "Status1",
                                    ["id"] = "v1",
                                    ["links"] = new JsonArray
                                    {
                                        new JsonObject
                                        {
                                            ["href"] = "http://example.com/1",
                                            ["rel"] = "sampleRel1"
                                        }
                                    }
                                }
                            },

                                new JsonObject
                                {
                                    ["status"] = "Status2",
                                    ["id"] = "v2",
                                    ["links"] = new JsonArray
                                    {
                                        new JsonObject
                                        {
                                            ["href"] = "http://example.com/2",
                                            ["rel"] = "sampleRel2"
                                        }
                                    }
                                }
                            }
                    })
                };

                var actualRoot = example.Value.Node["versions"][0]["status"].Root;
                var expectedRoot = expected.Value.Node["versions"][0]["status"].Root;

                diagnostic.Errors.Should().BeEmpty();

                example.Should().BeEquivalentTo(expected, options => options.IgnoringCyclicReferences()
                .Excluding(e => e.Value.Node["versions"][0]["status"].Root)
                .Excluding(e => e.Value.Node["versions"][0]["id"].Root)
                .Excluding(e => e.Value.Node["versions"][0]["links"][0]["href"].Root)
                .Excluding(e => e.Value.Node["versions"][0]["links"][0]["rel"].Root)
                .Excluding(e => e.Value.Node["versions"][1]["status"].Root)
                .Excluding(e => e.Value.Node["versions"][1]["id"].Root)
                .Excluding(e => e.Value.Node["versions"][1]["links"][0]["href"].Root)
                .Excluding(e => e.Value.Node["versions"][1]["links"][0]["rel"].Root));
            }
        }

        [Fact]
        public void ParseExampleForcedStringSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "explicitString.yaml"));
            new OpenApiStreamReader().Read(stream, out var diagnostic);
            diagnostic.Errors.Should().BeEmpty();
        }
    }
}
