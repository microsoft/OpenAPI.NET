// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
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
            var input =
                """
                aString: fooBar
                aInteger: 10
                aDouble: 2.34
                aDateTime: 2017-01-01
                """;
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            var anyMap = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(
                new OpenApiObject
                {
                    ["aString"] = new OpenApiString("fooBar"),
                    ["aInteger"] = new OpenApiString("10"),
                    ["aDouble"] = new OpenApiString("2.34"),
                    ["aDateTime"] = new OpenApiString("2017-01-01")
                });
        }

        [Fact]
        public void ParseListAsAnyShouldSucceed()
        {
            var input =
                """
                - fooBar
                - 10
                - 2.34
                - 2017-01-01
                """;
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new ListNode(context, (YamlSequenceNode)yamlNode);

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(
                new OpenApiArray
                {
                    new OpenApiString("fooBar"),
                    new OpenApiString("10"),
                    new OpenApiString("2.34"),
                    new OpenApiString("2017-01-01")
                });
        }

        [Fact]
        public void ParseScalarIntegerAsAnyShouldSucceed()
        {
            var input = "10";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new ValueNode(context, (YamlScalarNode)yamlNode);

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(
                new OpenApiString("10")
            );
        }

        [Fact]
        public void ParseScalarDateTimeAsAnyShouldSucceed()
        {
            var input = "2012-07-23T12:33:00";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new ValueNode(context, (YamlScalarNode)yamlNode);

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.Should().BeEquivalentTo(
                new OpenApiString("2012-07-23T12:33:00")
            );
        }
    }
}
