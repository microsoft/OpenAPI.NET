// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class JsonWriterTests
    {
        [Fact]
        public void WriteList()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            writer.WriteStartArray();
            writer.WriteValue("hello");
            writer.WriteValue("world");
            writer.WriteEndArray();
            writer.Flush();

            var jarray = JArray.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal(2, jarray.Count);
        }

        [Fact]
        public void WriteMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            writer.WriteStartObject();
            writer.WriteStringProperty("hello", "world");
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndObject();
            writer.Flush();

            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("world", jObject["hello"]);
        }

        [Fact]
        public void WriteNestedEmptyMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);

            writer.WriteStartObject();
            writer.WritePropertyName("intro");
            writer.WriteStartObject();
            writer.WriteEndObject();

            writer.WritePropertyName("outro");
            writer.WriteStartObject();
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndObject();

            writer.WriteEndObject();
            writer.Flush();

            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("bye", jObject["outro"]["good"]);
        }

        [Fact]
        public void WriteNestedMap()
        {
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            writer.WriteStartObject();
            writer.WritePropertyName("intro");
            writer.WriteStartObject();
            writer.WriteStringProperty("hello", "world");
            writer.WriteEndObject();

            writer.WritePropertyName("outro");
            writer.WriteStartObject();
            writer.WriteStringProperty("good", "bye");
            writer.WriteEndObject();

            writer.WriteEndObject();
            writer.Flush();

            var jObject = JObject.Parse(outputString.GetStringBuilder().ToString());

            Assert.Equal("world", jObject["intro"]["hello"]);
        }
    }
}