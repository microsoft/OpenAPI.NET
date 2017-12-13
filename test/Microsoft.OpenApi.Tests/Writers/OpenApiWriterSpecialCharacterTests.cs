// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
            var outputStringWriter = new StringWriter();
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
        public void WriteStringWithSpecialCharactersAsYamlWorks(string input, string expected)
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);

            // Act
            writer.WriteValue(input);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual.Should().Be(expected);
        }
    }
}