// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiExample/";

        public OpenApiExampleTests() 
        {     
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParseAdvancedExampleShouldSucceed()
        {
            var example = OpenApiModelFactory.Load<OpenApiExample>(Path.Combine(SampleFolderPath, "advancedExample.yaml"), OpenApiSpecVersion.OpenApi3_0, out var diagnostic);
            var expected = new OpenApiExample
            {
                Value = new JsonObject
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
                }
            };

            var actualRoot = example.Value["versions"][0]["status"].Root;
            var expectedRoot = expected.Value["versions"][0]["status"].Root;

            diagnostic.Errors.Should().BeEmpty();

            example.Should().BeEquivalentTo(expected, options => options.IgnoringCyclicReferences()
            .Excluding(e => e.Value["versions"][0]["status"].Root)
            .Excluding(e => e.Value["versions"][0]["id"].Root)
            .Excluding(e => e.Value["versions"][0]["links"][0]["href"].Root)
            .Excluding(e => e.Value["versions"][0]["links"][0]["rel"].Root)
            .Excluding(e => e.Value["versions"][1]["status"].Root)
            .Excluding(e => e.Value["versions"][1]["id"].Root)
            .Excluding(e => e.Value["versions"][1]["links"][0]["href"].Root)
            .Excluding(e => e.Value["versions"][1]["links"][0]["rel"].Root));
        }

        [Fact]
        public void ParseExampleForcedStringSucceed()
        {
            var result= OpenApiDocument.Load(Path.Combine(SampleFolderPath, "explicitString.yaml"));
            result.Diagnostic.Errors.Should().BeEmpty();
        }
    }
}
