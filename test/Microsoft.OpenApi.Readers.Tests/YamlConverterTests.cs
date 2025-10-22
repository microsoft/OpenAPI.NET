using SharpYaml;
using SharpYaml.Serialization;
using Xunit;
using Microsoft.OpenApi.YamlReader;
using System.IO;
using System.Text.Json.Nodes;

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

    [Fact]
    public void ToYamlNode_StringValue_NotQuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""fooString"": ""fooStringValue""}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("fooString: fooStringValue", yamlOutput);
        Assert.DoesNotContain("\"fooStringValue\"", yamlOutput);
        Assert.DoesNotContain("'fooStringValue'", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeNumber_QuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""fooStringOfNumber"": ""200""}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("fooStringOfNumber: \"200\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualNumber_NotQuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""actualNumber"": 200}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualNumber: 200", yamlOutput);
        Assert.DoesNotContain("\"200\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeDecimal_QuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""decimalString"": ""123.45""}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("decimalString: \"123.45\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualDecimal_NotQuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""actualDecimal"": 123.45}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualDecimal: 123.45", yamlOutput);
        Assert.DoesNotContain("\"123.45\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeBoolean_QuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""boolString"": ""true""}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("boolString: \"true\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualBoolean_NotQuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""actualBool"": true}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualBool: true", yamlOutput);
        Assert.DoesNotContain("\"true\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeNull_QuotedInYaml()
    {
        // Arrange
        var json = JsonNode.Parse(@"{""nullString"": ""null""}");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("nullString: \"null\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_MixedTypes_CorrectQuoting()
    {
        // Arrange
        var json = JsonNode.Parse(@"{
            ""str"": ""hello"",
            ""numStr"": ""42"",
            ""num"": 42,
            ""boolStr"": ""false"",
            ""bool"": false
        }");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("str: hello", yamlOutput);
        Assert.Contains("numStr: \"42\"", yamlOutput);
        Assert.Contains("num: 42", yamlOutput);
        Assert.DoesNotContain("num: \"42\"", yamlOutput);
        Assert.Contains("boolStr: \"false\"", yamlOutput);
        Assert.Contains("bool: false", yamlOutput);
        Assert.DoesNotContain("bool: \"false\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_FromIssueExample_CorrectOutput()
    {
        // Arrange - Example from issue #1951
        var json = JsonNode.Parse(@"{
            ""fooString"": ""fooStringValue"",
            ""fooStringOfNumber"": ""200""
        }");

        // Act
        var yamlNode = json!.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("fooString: fooStringValue", yamlOutput);
        Assert.Contains("fooStringOfNumber: \"200\"", yamlOutput);
        
        // Ensure no extra quotes on regular strings
        Assert.DoesNotContain("\"fooStringValue\"", yamlOutput);
        Assert.DoesNotContain("'fooStringValue'", yamlOutput);
    }

    private static string ConvertYamlNodeToString(YamlNode yamlNode)
    {
        using var ms = new MemoryStream();
        var yamlStream = new YamlStream(new YamlDocument(yamlNode));
        var writer = new StreamWriter(ms);
        yamlStream.Save(writer);
        writer.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(ms);
        return reader.ReadToEnd();
    }
}
