﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                var anyMap = node.CreateAny();

                diagnostic.Errors.Should().BeEmpty();

                anyMap.ShouldBeEquivalentTo(
                    new OpenApiObject
                    {
                        ["aString"] = new OpenApiString("fooBar"),
                        ["aInteger"] = new OpenApiInteger(10),
                        ["aDouble"] = new OpenApiDouble(2.34),
                        ["aDateTime"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01"))
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
            var yamlNode = yamlStream.Documents.First().RootNode;

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new ListNode(context, diagnostic, (YamlSequenceNode)yamlNode);

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.ShouldBeEquivalentTo(
                new OpenApiArray
                {
                    new OpenApiString("fooBar"),
                    new OpenApiInteger(10),
                    new OpenApiDouble(2.34),
                    new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01"))
                });
        }

        [Fact]
        public void ParseScalarIntegertAsAnyShouldSucceed()
        {

            var input = @"
10
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var node = new ValueNode(context, diagnostic, (YamlScalarNode)yamlNode);

            var any = node.CreateAny();

            diagnostic.Errors.Should().BeEmpty();

            any.ShouldBeEquivalentTo(
                    new OpenApiInteger(10)
                );
        }

    }
}

