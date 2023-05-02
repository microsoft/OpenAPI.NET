// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Readers.ParseNodes;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiAnyTests
    {
        [Fact]
        public void ParseMapAsAnyShouldSucceed()
        {
            var input = @"
aString: fooBar
aInteger: 10
aDouble: 2.34
aDateTime: 2017-01-01
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);
            
            var anyMap = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(new JsonObject
            {
                ["aString"] = "fooBar",
                ["aInteger"] = 10,
                ["aDouble"] = 2.34,
                ["aDateTime"] = "2017-01-01"
            });
        }

        [Fact]
        public void ParseListAsAnyShouldSucceed()
        {
            var input = @"
- fooBar
- 10
- 2.34
- 2017-01-01
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = (YamlSequenceNode)yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new ListNode(context, yamlNode.ToJsonArray());
            
            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(
                new JsonArray
                {
                    "fooBar", "10", "2.34", "2017-01-01"
                });
        }

        [Fact]
        public void ParseScalarIntegerAsAnyShouldSucceed()
        {
            var input = @"
10
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);            

            var node = new ValueNode(context, yamlNode.ToJsonNode());

            var any = node.CreateAny();
            var root = any.Root;
            
            diagnostic.Errors.Should().BeEmpty();
            var expected = JsonNode.Parse(input);

            any.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public void ParseScalarDateTimeAsAnyShouldSucceed()
        {
            var input = @"
2012-07-23T12:33:00
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new ValueNode(context, yamlNode.ToJsonNode());
            var expected = DateTimeOffset.Parse(input.Trim('"'));
            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(JsonNode.Parse(expected.ToString()));
        }
    }
}
