// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    public class OpenApiWriterInlineReferenceTests
    {
        private const int InlineReferenceLimit = 128;
        private const int OutputCharacterLimit = 100_000;

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, false)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1, true)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, false)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2, true)]
        public void SerializeWithRepeatedInlineReferencesStopsBeforeWriterLimit(
            OpenApiSpecVersion specVersion,
            bool useYaml)
        {
            var readResult = OpenApiDocument.Parse(CreateDocumentWithRepeatedReferences(18), "json");
            Assert.Empty(readResult.Diagnostic.Errors);
            Assert.NotNull(readResult.Document);

            using var textWriter = new CharacterLimitedTextWriter(OutputCharacterLimit);
            IOpenApiWriter writer = useYaml
                ? new OpenApiYamlWriter(textWriter, CreateWriterSettings(InlineReferenceLimit))
                : new OpenApiJsonWriter(textWriter, CreateWriterSettings(InlineReferenceLimit));

            var exception = Assert.Throws<OpenApiWriterException>(
                () => Serialize(readResult.Document, writer, specVersion));
            Assert.Contains(InlineReferenceLimit.ToString(CultureInfo.InvariantCulture), exception.Message);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        [InlineData(OpenApiSpecVersion.OpenApi3_2)]
        public void SerializeWithInlineReferencesBelowLimitWorks(OpenApiSpecVersion specVersion)
        {
            var readResult = OpenApiDocument.Parse(CreateDocumentWithRepeatedReferences(4), "json");
            Assert.Empty(readResult.Diagnostic.Errors);
            Assert.NotNull(readResult.Document);

            using var textWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(textWriter, CreateWriterSettings(32));

            Serialize(readResult.Document, writer, specVersion);

            Assert.NotEmpty(textWriter.ToString());
        }

        [Fact]
        public void ReusingSettingsWithNewWriterStartsANewInlineReferenceCount()
        {
            var largeReadResult = OpenApiDocument.Parse(CreateDocumentWithRepeatedReferences(6), "json");
            Assert.Empty(largeReadResult.Diagnostic.Errors);
            Assert.NotNull(largeReadResult.Document);
            var smallReadResult = OpenApiDocument.Parse(CreateDocumentWithRepeatedReferences(4), "json");
            Assert.Empty(smallReadResult.Diagnostic.Errors);
            Assert.NotNull(smallReadResult.Document);
            var settings = CreateWriterSettings(32);

            using var firstOutput = new StringWriter(CultureInfo.InvariantCulture);
            Assert.Throws<OpenApiWriterException>(
                () => Serialize(
                    largeReadResult.Document,
                    new OpenApiJsonWriter(firstOutput, settings),
                    OpenApiSpecVersion.OpenApi3_0));

            using var secondOutput = new StringWriter(CultureInfo.InvariantCulture);
            Serialize(
                smallReadResult.Document,
                new OpenApiJsonWriter(secondOutput, settings),
                OpenApiSpecVersion.OpenApi3_0);

            Assert.DoesNotContain("$ref", secondOutput.ToString());
        }

        private static string CreateDocumentWithRepeatedReferences(int levels)
        {
            var schemas = new StringBuilder();
            for (var i = 0; i < levels; i++)
            {
                if (i > 0)
                {
                    schemas.Append(',');
                }

                schemas
                    .Append("\"s")
                    .Append(i.ToString(CultureInfo.InvariantCulture))
                    .Append("\":{\"allOf\":[{\"$ref\":\"#/components/schemas/s")
                    .Append((i + 1).ToString(CultureInfo.InvariantCulture))
                    .Append("\"},{\"$ref\":\"#/components/schemas/s")
                    .Append((i + 1).ToString(CultureInfo.InvariantCulture))
                    .Append("\"}]}");
            }

            schemas
                .Append(",\"s")
                .Append(levels.ToString(CultureInfo.InvariantCulture))
                .Append("\":{\"type\":\"string\"}");

            return $$"""
                {
                  "openapi": "3.0.0",
                  "info": {
                    "title": "Repeated references",
                    "version": "1.0.0"
                  },
                  "paths": {
                    "/test": {
                      "get": {
                        "responses": {
                          "200": {
                            "description": "OK",
                            "content": {
                              "application/json": {
                                "schema": {
                                  "$ref": "#/components/schemas/s0"
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  },
                  "components": {
                    "schemas": {
                      {{schemas}}
                    }
                  }
                }
                """;
        }

        private static void Serialize(
            OpenApiDocument document,
            IOpenApiWriter writer,
            OpenApiSpecVersion specVersion)
        {
            switch (specVersion)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    document.SerializeAsV2(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    document.SerializeAsV3(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_1:
                    document.SerializeAsV31(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_2:
                    document.SerializeAsV32(writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(specVersion), specVersion, null);
            }
        }

        private static OpenApiWriterSettings CreateWriterSettings(int maxInlineReferenceCount)
        {
            return new OpenApiWriterSettings
            {
                InlineLocalReferences = true,
                MaxInlineReferenceCount = maxInlineReferenceCount
            };
        }

        private sealed class CharacterLimitedTextWriter(int characterLimit) : TextWriter
        {
            private int _characterCount;

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
                CountCharacters(1);
            }

            public override void Write(string value)
            {
                CountCharacters(value?.Length ?? 0);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                CountCharacters(count);
            }

            public override void Write(ReadOnlySpan<char> buffer)
            {
                CountCharacters(buffer.Length);
            }

            private void CountCharacters(int count)
            {
                _characterCount += count;
                if (_characterCount > characterLimit)
                {
                    throw new CharacterLimitExceededException();
                }
            }
        }

        private sealed class CharacterLimitExceededException : Exception;
    }
}
