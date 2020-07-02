// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiWriterSpecialCharacterTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiWriterSpecialCharacterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("Test\bTest", "\"Test\\bTest\"")]
        [InlineData("Test\fTest", "\"Test\\fTest\"")]
        [InlineData("Test\nTest", "\"Test\\nTest\"")]
        [InlineData("Test\rTest", "\"Test\\rTest\"")]
        [InlineData("Test\tTest", "\"Test\\tTest\"")]
        [InlineData("Test\\Test", "\"Test\\\\Test\"")]
        [InlineData("Test\"Test", "\"Test\\\"Test\"")]
        [InlineData("StringsWith\"Quotes\"", "\"StringsWith\\\"Quotes\\\"\"")]
        public void WriteStringWithSpecialCharactersAsJsonWorks(string input, string expected)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);

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
        [InlineData("multiline\r\nstring", "test: |\n  multiline\n  string")]
        [InlineData("ends with\r\nline break\r\n", "test: |+\n  ends with\n  line break")]
        [InlineData("  starts with\nspaces", "test: |2\n    starts with\n  spaces")]
        [InlineData("  starts with\nspaces, and ends with line break\n", "test: |+2\n    starts with\n  spaces, and ends with line break")]
        [InlineData("contains\n\n\nempty lines", "test: |\n  contains\n\n\n  empty lines")]
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
        [InlineData("multiline\r\nstring", "- |\n  multiline\n  string")]
        [InlineData("ends with\r\nline break\r\n", "- |+\n  ends with\n  line break")]
        [InlineData("  starts with\nspaces", "- |2\n    starts with\n  spaces")]
        [InlineData("  starts with\nspaces, and ends with line break\n", "- |+2\n    starts with\n  spaces, and ends with line break")]
        [InlineData("contains\n\n\nempty lines", "- |\n  contains\n\n\n  empty lines")]
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
    }
}
