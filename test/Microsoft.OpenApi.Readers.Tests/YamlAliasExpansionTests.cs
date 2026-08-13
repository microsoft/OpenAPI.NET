// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.ParseNodes;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    [Collection("DefaultSettings")]
    public class YamlAliasExpansionTests
    {
        // A "billion laughs" YAML bomb: each level references the previous one multiple times,
        // so materializing the shared node graph into an independent object tree expands
        // exponentially. The conversion must fail fast instead of exhausting memory.
        private const string YamlBomb =
            """
            a: &a ["x","x","x","x","x","x","x","x","x"]
            b: &b [*a,*a,*a,*a,*a,*a,*a,*a,*a]
            c: &c [*b,*b,*b,*b,*b,*b,*b,*b,*b]
            d: &d [*c,*c,*c,*c,*c,*c,*c,*c,*c]
            e: &e [*d,*d,*d,*d,*d,*d,*d,*d,*d]
            f: &f [*e,*e,*e,*e,*e,*e,*e,*e,*e]
            g: &g [*f,*f,*f,*f,*f,*f,*f,*f,*f]
            h: &h [*g,*g,*g,*g,*g,*g,*g,*g,*g]
            i: &i [*h,*h,*h,*h,*h,*h,*h,*h,*h]
            """;

        [Fact]
        public void ExponentialAliasExpansionIsRejected()
        {
            var node = ParseNode.Create(new(new()), YamlHelper.ParseYamlString(YamlBomb));

            Assert.Throws<OpenApiReaderException>(() => node.CreateAny());
        }

        [Fact]
        public void ExcessiveNestingDepthIsRejected()
        {
            // Deeper than the conversion depth limit, which protects the recursive
            // converter from stack exhaustion.
            const int depth = 70;
            var deeplyNested = new string('[', depth) + new string(']', depth);

            var node = ParseNode.Create(new(new()), YamlHelper.ParseYamlString(deeplyNested));

            Assert.Throws<OpenApiReaderException>(() => node.CreateAny());
        }

        [Fact]
        public void ReadReturnsDiagnosticErrorForExponentialAliasExpansion()
        {
            // A "billion laughs" YAML bomb must surface as a diagnostic error
            // rather than throwing or exhausting memory.
            var input =
                $$"""
                openapi: 3.0.0
                info:
                  title: bomb
                  version: 1.0.0
                paths: {}
                x-bomb:
                {{YamlBombIndented()}}
                """;

            var reader = new OpenApiStringReader();
            reader.Read(input, out var diagnostic);

            diagnostic.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void LegitimateAliasesStillConvert()
        {
            var input =
                """
                a: &val hello
                b: *val
                """;

            var node = ParseNode.Create(new(new()), YamlHelper.ParseYamlString(input));

            var anyObject = Assert.IsType<OpenApiObject>(node.CreateAny());
            Assert.Equal("hello", ((OpenApiString)anyObject["a"]).Value);
            Assert.Equal("hello", ((OpenApiString)anyObject["b"]).Value);
        }

        [Fact]
        public void ConversionLimitsDefaultToDocumentedValues()
        {
            var settings = new OpenApiReaderSettings();

            Assert.Equal(64u, OpenApiReaderSettings.DefaultMaxDepth);
            Assert.Equal(5_000_000u, OpenApiReaderSettings.DefaultMaxNodeCount);
            Assert.Equal(OpenApiReaderSettings.DefaultMaxDepth, settings.MaxDepth);
            Assert.Equal(OpenApiReaderSettings.DefaultMaxNodeCount, settings.MaxNodeCount);
        }

        [Fact]
        public void SettingMaxDepthToZeroThrows()
        {
            var settings = new OpenApiReaderSettings();

            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxDepth = 0);
            // The invalid assignment must not have changed the effective limit.
            Assert.Equal(OpenApiReaderSettings.DefaultMaxDepth, settings.MaxDepth);
        }

        [Fact]
        public void SettingMaxNodeCountToZeroThrows()
        {
            var settings = new OpenApiReaderSettings();

            Assert.Throws<ArgumentOutOfRangeException>(() => settings.MaxNodeCount = 0);
            // The invalid assignment must not have changed the effective limit.
            Assert.Equal(OpenApiReaderSettings.DefaultMaxNodeCount, settings.MaxNodeCount);
        }

        [Fact]
        public void RaisingMaxDepthAllowsDocumentsDeeperThanTheDefault()
        {
            // A document nested deeper than the default depth limit (64) is rejected by default
            // but can be permitted by a consumer that opts into a higher limit.
            const int depth = 70;
            var deeplyNested = new string('[', depth) + new string(']', depth);

            var context = new ParsingContext(new()) { MaxDepth = depth + 10 };
            var node = ParseNode.Create(context, YamlHelper.ParseYamlString(deeplyNested));

            Assert.IsType<OpenApiArray>(node.CreateAny());
        }

        private static string YamlBombIndented()
        {
            return "  " + YamlBomb.Replace("\r\n", "\n").Replace("\n", "\n  ");
        }
    }
}
