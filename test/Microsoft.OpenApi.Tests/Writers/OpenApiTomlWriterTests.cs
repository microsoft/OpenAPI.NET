// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiTomlWriterTests
    {
        private static async Task<string> SerializeAsync(Action<OpenApiTomlWriter> act)
        {
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiTomlWriter(outputString);
            act(writer);
            await ((IOpenApiWriter)writer).FlushAsync();
            return outputString.GetStringBuilder().ToString();
        }

        [Fact]
        public async Task WriteStringProperty_ShouldProduceQuotedTomlValue()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("title");
                w.WriteValue("My API");
                w.WriteEndObject();
            });

            Assert.Contains("title = \"My API\"", toml);
        }

        [Fact]
        public async Task WriteIntProperty_ShouldProduceTomlInteger()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("port");
                w.WriteValue(8080);
                w.WriteEndObject();
            });

            Assert.Contains("port = 8080", toml);
        }

        [Fact]
        public async Task WriteBoolProperty_ShouldProduceLowercaseBool()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("required");
                w.WriteValue(true);
                w.WriteEndObject();
            });

            Assert.Contains("required = true", toml);
        }

        [Fact]
        public async Task WriteDecimalProperty_ShouldProduceTomlFloat()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("ratio");
                w.WriteValue(3.14m);
                w.WriteEndObject();
            });

            Assert.Contains("ratio = ", toml);
            Assert.Contains("3.14", toml);
        }

        [Fact]
        public async Task WriteNullValue_ShouldBeOmittedFromOutput()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("missing");
                w.WriteNull();
                w.WritePropertyName("present");
                w.WriteValue("here");
                w.WriteEndObject();
            });

            Assert.DoesNotContain("missing", toml);
            Assert.Contains("present = \"here\"", toml);
        }

        [Fact]
        public async Task WriteStringWithSpecialChars_ShouldEscapeCorrectly()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("msg");
                w.WriteValue("line1\nline2\ttabbed \"quoted\"");
                w.WriteEndObject();
            });

            Assert.Contains(@"\n", toml);
            Assert.Contains(@"\t", toml);
            Assert.Contains(@"\""", toml);
        }

        [Fact]
        public async Task KeyWithSpecialChars_ShouldBeQuoted()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("/pets");
                w.WriteValue("path");
                w.WriteEndObject();
            });

            Assert.Contains("\"/pets\" = \"path\"", toml);
        }

        [Fact]
        public async Task BareKey_ShouldNotBeQuoted()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("openapi");
                w.WriteValue("3.0.1");
                w.WriteEndObject();
            });

            // The key must appear unquoted
            Assert.Matches(new Regex(@"^openapi\s*=", RegexOptions.Multiline), toml);
        }

        [Fact]
        public async Task WriteStringArray_ShouldProduceTomlInlineArray()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("tags");
                w.WriteStartArray();
                w.WriteValue("pets");
                w.WriteValue("store");
                w.WriteEndArray();
                w.WriteEndObject();
            });

            Assert.Contains("tags = [\"pets\", \"store\"]", toml);
        }

        [Fact]
        public async Task WriteEmptyArray_ShouldProduceEmptyInlineArray()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("tags");
                w.WriteStartArray();
                w.WriteEndArray();
                w.WriteEndObject();
            });

            Assert.Contains("tags = []", toml);
        }

        [Fact]
        public async Task WriteArrayOfObjects_ShouldProduceArrayOfTableHeaders()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("servers");
                w.WriteStartArray();

                w.WriteStartObject();
                w.WritePropertyName("url");
                w.WriteValue("https://example.com");
                w.WriteEndObject();

                w.WriteStartObject();
                w.WritePropertyName("url");
                w.WriteValue("https://staging.example.com");
                w.WriteEndObject();

                w.WriteEndArray();
                w.WriteEndObject();
            });

            Assert.Contains("[[servers]]", toml);
            Assert.Contains("url = \"https://example.com\"", toml);
            Assert.Contains("url = \"https://staging.example.com\"", toml);
            // Should appear exactly twice
            Assert.Equal(2, CountOccurrences(toml, "[[servers]]"));
        }

        [Fact]
        public async Task WriteNestedObject_ShouldProduceSectionHeader()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("info");
                w.WriteStartObject();
                w.WritePropertyName("title");
                w.WriteValue("Sample API");
                w.WritePropertyName("version");
                w.WriteValue("1.0.0");
                w.WriteEndObject();
                w.WriteEndObject();
            });

            Assert.Contains("[info]", toml);
            Assert.Contains("title = \"Sample API\"", toml);
            Assert.Contains("version = \"1.0.0\"", toml);
        }

        [Fact]
        public async Task WriteDeepNestedObject_ShouldProduceDottedSectionHeader()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("paths");
                w.WriteStartObject();
                w.WritePropertyName("/pets");
                w.WriteStartObject();
                w.WritePropertyName("get");
                w.WriteStartObject();
                w.WritePropertyName("summary");
                w.WriteValue("List pets");
                w.WriteEndObject();
                w.WriteEndObject();
                w.WriteEndObject();
                w.WriteEndObject();
            });

            Assert.Contains("[paths.\"/pets\".get]", toml);
            Assert.Contains("summary = \"List pets\"", toml);
        }

        [Fact]
        public async Task WriteRootScalar_ShouldAppearWithoutSectionHeader()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();
                w.WritePropertyName("openapi");
                w.WriteValue("3.0.1");
                w.WriteEndObject();
            });

            // Root scalars must not be under a [header]
            var lines = toml.Split('\n');
            var openapiLine = Array.FindIndex(lines, l => l.Contains("openapi = "));
            Assert.True(openapiLine >= 0);

            // No [section] header before the openapi line
            for (var i = 0; i < openapiLine; i++)
            {
                Assert.DoesNotMatch(new Regex(@"^\["), lines[i].TrimStart());
            }
        }

        [Fact]
        public async Task WriteMinimalOpenApiDocument_ShouldProduceValidToml()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();

                w.WritePropertyName("openapi");
                w.WriteValue("3.0.1");

                w.WritePropertyName("info");
                w.WriteStartObject();
                w.WritePropertyName("title");
                w.WriteValue("Minimal API");
                w.WritePropertyName("version");
                w.WriteValue("0.1.0");
                w.WriteEndObject();

                w.WritePropertyName("paths");
                w.WriteStartObject();
                w.WriteEndObject();

                w.WriteEndObject();
            });

            Assert.Contains("openapi = \"3.0.1\"", toml);
            Assert.Contains("[info]", toml);
            Assert.Contains("title = \"Minimal API\"", toml);
            Assert.Contains("version = \"0.1.0\"", toml);
        }

        [Fact]
        public async Task WriteDocumentWithParameters_ShouldUseArrayOfTableHeaders()
        {
            var toml = await SerializeAsync(w =>
            {
                w.WriteStartObject();

                w.WritePropertyName("paths");
                w.WriteStartObject();
                w.WritePropertyName("/pets");
                w.WriteStartObject();
                w.WritePropertyName("get");
                w.WriteStartObject();

                w.WritePropertyName("summary");
                w.WriteValue("List pets");

                w.WritePropertyName("parameters");
                w.WriteStartArray();

                w.WriteStartObject();
                w.WritePropertyName("name");
                w.WriteValue("limit");
                w.WritePropertyName("in");
                w.WriteValue("query");
                w.WriteEndObject();

                w.WriteEndArray();

                w.WriteEndObject(); // get
                w.WriteEndObject(); // /pets
                w.WriteEndObject(); // paths
                w.WriteEndObject(); // root
            });

            Assert.Contains("[paths.\"/pets\".get]", toml);
            Assert.Contains("summary = \"List pets\"", toml);
            Assert.Contains("[[paths.\"/pets\".get.parameters]]", toml);
            Assert.Contains("name = \"limit\"", toml);
            Assert.Contains("in = \"query\"", toml);
        }

        [Fact]
        public async Task SerializeAsTomlAsync_Stream_ShouldProduceTomlOutput()
        {
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo { Title = "Test", Version = "1.0" },
            };

            var toml = await document.SerializeAsTomlAsync(OpenApiSpecVersion.OpenApi3_0);

            Assert.Contains("title = \"Test\"", toml);
            Assert.Contains("[info]", toml);
        }

        private static int CountOccurrences(string source, string pattern)
        {
            var count = 0;
            var index = 0;
            while ((index = source.IndexOf(pattern, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += pattern.Length;
            }
            return count;
        }
    }
}
