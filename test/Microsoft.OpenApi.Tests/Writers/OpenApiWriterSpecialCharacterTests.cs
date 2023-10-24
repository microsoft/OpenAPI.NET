// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiWriterSpecialCharacterTests
    {
        static bool[] shouldProduceTerseOutputValues = new[] { true, false };

        public static IEnumerable<object[]> StringWithSpecialCharacters
        {
            get =>
                from inputExpected in new[] {
                    new[]{ "Test\bTest", "\"Test\\bTest\"" },
                    new[]{ "Test\fTest", "\"Test\\fTest\""},
                    new[]{ "Test\nTest", "\"Test\\nTest\""},
                    new[]{ "Test\rTest", "\"Test\\rTest\""},
                    new[]{ "Test\tTest", "\"Test\\tTest\""},
                    new[]{ "Test\\Test", "\"Test\\\\Test\""},
                    new[]{ "Test\"Test", "\"Test\\\"Test\""},
                    new[]{ "StringsWith\"Quotes\"", "\"StringsWith\\\"Quotes\\\"\""},
                    new[]{ "0x1234", "\"0x1234\""},
                }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { inputExpected[0], inputExpected[1], shouldBeTerse };
        }

        [Theory]
        [MemberData(nameof(StringWithSpecialCharacters))]
        public void WriteStringWithSpecialCharactersAsJsonWorks(string input, string expected, bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            writer.WriteValue(input);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("", " ''")]
        [InlineData("~", " '~'")]
        [InlineData("a~", " a~")]
        [InlineData("symbols#!/", " symbols#!/")]
        [InlineData("symbols'#!/'", " symbols'#!/'")]
        [InlineData("#beginningsymbols'#!/'", " '#beginningsymbols''#!/'''")]
        [InlineData("forbiddensymbols'{!/'", " 'forbiddensymbols''{!/'''")]
        [InlineData("forbiddensymbols': !/'", " 'forbiddensymbols'': !/'''")]
        [InlineData("backslash\\", " backslash\\")]
        [InlineData("doublequotes\"", " doublequotes\"")]
        [InlineData("controlcharacters\n\r", " \"controlcharacters\\n\\r\"")]
        [InlineData("controlcharacters\"\n\r\"", " \"controlcharacters\\\"\\n\\r\\\"\"")]
        [InlineData("40", " '40'")]
        [InlineData("a40", " a40")]
        [InlineData("true", " 'true'")]
        [InlineData("trailingspace ", " 'trailingspace '")]
        [InlineData("     trailingspace", " '     trailingspace'")]
        [InlineData("terminal:", " 'terminal:'")]
        [InlineData("0x1234", " '0x1234'")]
        public void WriteStringWithSpecialCharactersAsYamlWorks(string input, string expected)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            // Act
            writer.WriteValue(input);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("multiline\r\nstring", "test: |-\n  multiline\n  string")]
        [InlineData("ends with\r\nline break\r\n", "test: |\n  ends with\n  line break")]
        [InlineData("ends with\r\n2 line breaks\r\n\r\n", "test: |+\n  ends with\n  2 line breaks\n")]
        [InlineData("ends with\r\n3 line breaks\r\n\r\n\r\n", "test: |+\n  ends with\n  3 line breaks\n\n")]
        [InlineData("  starts with\nspaces", "test: |-2\n    starts with\n  spaces")]
        [InlineData("  starts with\nspaces, and ends with line break\n", "test: |2\n    starts with\n  spaces, and ends with line break")]
        [InlineData("contains\n\n\nempty lines", "test: |-\n  contains\n\n\n  empty lines")]
        [InlineData("no line breaks fallback ", "test: 'no line breaks fallback '")]
        public void WriteStringWithNewlineCharactersInObjectAsYamlWorks(string input, string expected)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter) { UseLiteralStyle = true, };

            // Act
            writer.WriteStartObject();
            writer.WritePropertyName("test");
            writer.WriteValue(input);
            writer.WriteEndObject();
            var actual = outputStringWriter.GetStringBuilder().ToString()
                // Normalize newline for cross platform
                .Replace("\r", "");

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("multiline\r\nstring", "- |-\n  multiline\n  string")]
        [InlineData("ends with\r\nline break\r\n", "- |\n  ends with\n  line break")]
        [InlineData("ends with\r\n2 line breaks\r\n\r\n", "- |+\n  ends with\n  2 line breaks\n")]
        [InlineData("ends with\r\n3 line breaks\r\n\r\n\r\n", "- |+\n  ends with\n  3 line breaks\n\n")]
        [InlineData("  starts with\nspaces", "- |-2\n    starts with\n  spaces")]
        [InlineData("  starts with\nspaces, and ends with line break\n", "- |2\n    starts with\n  spaces, and ends with line break")]
        [InlineData("contains\n\n\nempty lines", "- |-\n  contains\n\n\n  empty lines")]
        [InlineData("no line breaks fallback ", "- 'no line breaks fallback '")]
        public void WriteStringWithNewlineCharactersInArrayAsYamlWorks(string input, string expected)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter) { UseLiteralStyle = true, };

            // Act
            writer.WriteStartArray();
            writer.WriteValue(input);
            var actual = outputStringWriter.GetStringBuilder().ToString()
                // Normalize newline for cross platform
                .Replace("\r", "");

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("1.8.0", " '1.8.0'", "en-US")]
        [InlineData("1.8.0", " '1.8.0'", "en-GB")]
        [InlineData("1.13.0", " '1.13.0'", "en-US")]
        [InlineData("1.13.0", " '1.13.0'", "en-GB")]
        public void WriteStringAsYamlDoesNotDependOnSystemCulture(string input, string expected, string culture)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            // Act
            writer.WriteValue(input);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
