// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
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

            anyMap.Should().BeEquivalentTo(@"{
  ""aString"": {
    ""type"": ""string"",
    ""value"": ""fooBar""
  },
  ""aInteger"": {
    ""type"": ""integer"",
    ""value"": 10
  },
  ""aDouble"": {
    ""type"": ""number"",
    ""format"": ""double"",
    ""value"": 2.34
  },
  ""aDateTime"": {
    ""type"": ""string"",
    ""format"": ""date-time"",
    ""value"": ""2017-01-01T00:00:00+00:00""
  }
}");
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
                @"[
  ""fooBar"",
  ""10"",
  ""2.34"",
  ""2017-01-01""
]");
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

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(@"""10""");
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

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(@"""2012-07-23T12:33:00""");
        }
    }
}
