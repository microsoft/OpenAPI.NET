using SharpYaml;
using SharpYaml.Serialization;
using Xunit;
using Microsoft.OpenApi.YamlReader;

namespace Microsoft.OpenApi.Readers.Tests;

public class YamlConverterTests
{
    [Theory]
    [InlineData("~")]
    [InlineData("null")]
    [InlineData("Null")]
    [InlineData("NULL")]
    public void YamlNullValuesReturnNullJsonNode(string value)
    {
        // Given
        var yamlNull = new YamlScalarNode(value)
        {
            Style = ScalarStyle.Plain
        };

        // When
        var jsonNode = yamlNull.ToJsonNode();

        // Then
        Assert.Null(jsonNode);
    }
}
