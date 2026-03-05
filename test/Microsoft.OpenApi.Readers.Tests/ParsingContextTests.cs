// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
    }
}
