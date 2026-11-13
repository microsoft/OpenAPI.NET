// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class ParsingContextTests
    {
        private static ParsingContext CreateContext() => new(new OpenApiDiagnostic());

        [Fact]
        public void GetLocation_NoSegments_ReturnsRoot()
        {
            var context = CreateContext();

            var location = context.GetLocation();

            Assert.Equal("#/", location);
        }

        [Fact]
        public void GetLocation_SingleSegment_ReturnsSegment()
        {
            var context = CreateContext();
            context.StartObject("paths");

            var location = context.GetLocation();

            Assert.Equal("#/paths", location);
        }

        [Fact]
        public void GetLocation_MultipleSegments_ReturnsCombinedPath()
        {
            var context = CreateContext();
            context.StartObject("paths");
            context.StartObject("foo");
            context.StartObject("get");

            var location = context.GetLocation();

            Assert.Equal("#/paths/foo/get", location);
        }

        [Fact]
        public void GetLocation_AfterEndObject_RemovesLastSegment()
        {
            var context = CreateContext();
            context.StartObject("paths");
            context.StartObject("foo");
            context.EndObject();

            var location = context.GetLocation();

            Assert.Equal("#/paths", location);
        }

        [Fact]
        public void GetLocation_AfterAllEndObjects_ReturnsRoot()
        {
            var context = CreateContext();
            context.StartObject("paths");
            context.EndObject();

            var location = context.GetLocation();

            Assert.Equal("#/", location);
        }

        [Fact]
        public void GetLocation_SegmentWithTilde_EscapesTildeAsPerRfc6901()
        {
            var context = CreateContext();
            context.StartObject("foo~bar");

            var location = context.GetLocation();

            Assert.Equal("#/foo~0bar", location);
        }

        [Fact]
        public void GetLocation_SegmentWithSlash_EscapesSlashAsPerRfc6901()
        {
            var context = CreateContext();
            context.StartObject("application/json");

            var location = context.GetLocation();

            Assert.Equal("#/application~1json", location);
        }

        [Fact]
        public void GetLocation_SegmentWithTildeAndSlash_EscapesBothAsPerRfc6901()
        {
            var context = CreateContext();
            context.StartObject("a~b/c");

            var location = context.GetLocation();

            Assert.Equal("#/a~0b~1c", location);
        }

        [Fact]
        public void GetLocation_MultipleSegmentsWithSpecialChars_EscapesEachSegment()
        {
            var context = CreateContext();
            context.StartObject("content");
            context.StartObject("application/json");
            context.StartObject("schema");

            var location = context.GetLocation();

            Assert.Equal("#/content/application~1json/schema", location);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, true)]
        public void ParseFragment_WithNamedExamples_RestoresLocation(OpenApiSpecVersion version, bool isParameter)
        {
            var context = CreateContext();
            context.StartObject("parent");
            var node = JsonNode.Parse("""
                {
                  "examples": {
                    "empty": {},
                    "populated": { "value": "hello" }
                  }
                }
                """);
            Assert.NotNull(node);
            var document = new OpenApiDocument();

            if (isParameter)
            {
                Assert.NotNull(context.ParseFragment<OpenApiParameter>(node, version, document));
            }
            else
            {
                Assert.NotNull(context.ParseFragment<OpenApiMediaType>(node, version, document));
            }

            Assert.Equal("#/parent", context.GetLocation());
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, true)]
        public void ParseFragment_WhenNamedExampleThrows_RestoresLocation(OpenApiSpecVersion version, bool isParameter)
        {
            var context = CreateContext();
            context.StartObject("parent");
            var document = new OpenApiDocument();
            document.AddComponent("cycle", new OpenApiExampleReference("cycle", document));
            var node = JsonNode.Parse("""
                {
                  "examples": {
                    "cycle": { "$ref": "#/components/examples/cycle" }
                  }
                }
                """);
            Assert.NotNull(node);

            if (isParameter)
            {
                Assert.Throws<InvalidOperationException>(() => context.ParseFragment<OpenApiParameter>(node, version, document));
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => context.ParseFragment<OpenApiMediaType>(node, version, document));
            }

            Assert.Equal("#/parent", context.GetLocation());
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, true)]
        public void ParseFragment_WhenNamedExampleReportsError_PreservesExampleLocation(OpenApiSpecVersion version, bool isParameter)
        {
            var context = CreateContext();
            context.StartObject("parent");
            var document = new OpenApiDocument();
            document.AddComponent("unreadable", new UnreadableExampleReference());
            var node = JsonNode.Parse("""
                {
                  "examples": {
                    "unreadable": { "$ref": "#/components/examples/unreadable" }
                  }
                }
                """);
            Assert.NotNull(node);

            if (isParameter)
            {
                Assert.NotNull(context.ParseFragment<OpenApiParameter>(node, version, document));
            }
            else
            {
                Assert.NotNull(context.ParseFragment<OpenApiMediaType>(node, version, document));
            }

            var error = Assert.Single(context.Diagnostic.Errors);
            Assert.Equal("Cannot read example.", error.Message);
            Assert.Equal("#/parent/examples/unreadable", error.Pointer);
            Assert.Equal("#/parent", context.GetLocation());
        }

        private sealed class UnreadableExampleReference : OpenApiExampleReference
        {
            public UnreadableExampleReference() : base("unreadable")
            {
            }

            public override IOpenApiExample Target => throw new OpenApiException("Cannot read example.");
        }
    }
}
