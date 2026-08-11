using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.YamlReader;
using SharpYaml;
using SharpYaml.Serialization;
using System;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

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
        Assert.True(jsonNode.IsJsonNullSentinel());
    }

    [Fact]
    public void ToYamlNode_StringValue_NotQuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""fooString"": ""fooStringValue""}"));

        // Act
        var yamlNode = json.ToYamlNode();
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
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""fooStringOfNumber"": ""200""}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("fooStringOfNumber: \"200\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualNumber_NotQuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""actualNumber"": 200}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualNumber: 200", yamlOutput);
        Assert.DoesNotContain("\"200\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeDecimal_QuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""decimalString"": ""123.45""}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("decimalString: \"123.45\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualDecimal_NotQuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""actualDecimal"": 123.45}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualDecimal: 123.45", yamlOutput);
        Assert.DoesNotContain("\"123.45\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeBoolean_QuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""boolString"": ""true""}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("boolString: \"true\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_ActualBoolean_NotQuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""actualBool"": true}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("actualBool: true", yamlOutput);
        Assert.DoesNotContain("\"true\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringThatLooksLikeNull_QuotedInYaml()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{""nullString"": ""null""}"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("nullString: \"null\"", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_MixedTypes_CorrectQuoting()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{
            ""str"": ""hello"",
            ""numStr"": ""42"",
            ""num"": 42,
            ""boolStr"": ""false"",
            ""bool"": false
        }"));

        // Act
        var yamlNode = json.ToYamlNode();
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
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{
            ""fooString"": ""fooStringValue"",
            ""fooStringOfNumber"": ""200""
        }"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Assert
        Assert.Contains("fooString: fooStringValue", yamlOutput);
        Assert.Contains("fooStringOfNumber: \"200\"", yamlOutput);
        
        // Ensure no extra quotes on regular strings
        Assert.DoesNotContain("\"fooStringValue\"", yamlOutput);
        Assert.DoesNotContain("'fooStringValue'", yamlOutput);
    }

    [Fact]
    public void ToYamlNode_StringWithLineBreaks_PreservesLineBreaks()
    {
        // Arrange
        var json = Assert.IsType<JsonObject>(JsonNode.Parse(@"{
            ""multiline"": ""Line 1\nLine 2\nLine 3"",
            ""description"": ""This is a description\nwith line breaks\nin it""
        }"));

        // Act
        var yamlNode = json.ToYamlNode();
        var yamlOutput = ConvertYamlNodeToString(yamlNode);

        // Convert back to JSON to verify round-tripping
        var jsonBack = ConvertYamlStringToJsonNode(yamlOutput);

        // Assert - line breaks should be preserved during round-trip
        var originalMultiline = json["multiline"]?.GetValue<string>();
        var roundTripMultiline = jsonBack?["multiline"]?.GetValue<string>();
        Assert.Equal(originalMultiline, roundTripMultiline);
        Assert.Contains("\n", roundTripMultiline);

        var originalDescription = json["description"]?.GetValue<string>();
        var roundTripDescription = jsonBack?["description"]?.GetValue<string>();
        Assert.Equal(originalDescription, roundTripDescription);
        Assert.Contains("\n", roundTripDescription);
    }

    [Fact]
    public void NumericPropertyNamesShouldRemainStringsFromJson()
    {
        // Given
        var yamlInput =
        """
        "123": value1
        "456": value2
        """;

        // Given
        var jsonNode = Assert.IsType<JsonObject>(JsonNode.Parse(@"{
            ""123"": ""value1"",
            ""456"": ""value2""
        }"));

        // When
        var convertedBack = jsonNode.ToYamlNode();
        var convertedBackOutput = ConvertYamlNodeToString(convertedBack);

        // Then
        Assert.Equal(yamlInput.MakeLineBreaksEnvironmentNeutral(), convertedBackOutput.MakeLineBreaksEnvironmentNeutral());
    }

    [Fact]
    public void NumericPropertyNamesShouldRemainStringsFromYaml()
    {
        // Given
        var yamlInput =
        """
        "123": value1
        "456": value2
        """;

        var jsonNode = ConvertYamlStringToJsonNode(yamlInput);

        var convertedBack = jsonNode.ToYamlNode();
        var convertedBackOutput = ConvertYamlNodeToString(convertedBack);
        // Then
        Assert.Equal(yamlInput.MakeLineBreaksEnvironmentNeutral(), convertedBackOutput.MakeLineBreaksEnvironmentNeutral());
    }

    [Fact]
    public void BooleanPropertyNamesShouldRemainStringsFromYaml()
    {
        // Given
        var yamlInput =
        """
        "true": value1
        "false": value2
        """;

        var jsonNode = ConvertYamlStringToJsonNode(yamlInput);

        var convertedBack = jsonNode.ToYamlNode();
        var convertedBackOutput = ConvertYamlNodeToString(convertedBack);
        // Then
        Assert.Equal(yamlInput.MakeLineBreaksEnvironmentNeutral(), convertedBackOutput.MakeLineBreaksEnvironmentNeutral());
    }

    [Fact]
    public void LineBreaksShouldRoundTrip()
    {
        var yamlInput =
        """
        python: |-
          from openai import OpenAI

          client = OpenAI(
              api_key="My API Key",
          )
          page = client.beta.assistants.list()
          page = page.data[0]
          print(page.id)
        """;
        // When
        var jsonNode = ConvertYamlStringToJsonNode(yamlInput);
        var convertedBack = jsonNode.ToYamlNode();
        var convertedBackOutput = ConvertYamlNodeToString(convertedBack);

        // Then
        Assert.Equal(yamlInput.MakeLineBreaksEnvironmentNeutral(), convertedBackOutput.MakeLineBreaksEnvironmentNeutral());
    }
    [Fact]
    public void ItDoesNotSerializeTheSentinelValue()
    {
        var yamlValue = JsonNullSentinel.JsonNull.ToYamlNode();

        var scalarNode = Assert.IsType<YamlScalarNode>(yamlValue);
        Assert.Equal("null", scalarNode.Value, StringComparer.OrdinalIgnoreCase);
    }
    [Fact]
    public void RoundTripEmptyStringsValues()
    {
        // Given
        var yamlInput =
        """
        emptyString: ""
        """;

        // When
        var jsonNode = ConvertYamlStringToJsonNode(yamlInput);
        var convertedBack = jsonNode.ToYamlNode();
        var convertedBackOutput = ConvertYamlNodeToString(convertedBack);

        // Then
        Assert.Equal(yamlInput.MakeLineBreaksEnvironmentNeutral(), convertedBackOutput.MakeLineBreaksEnvironmentNeutral());
    }

    [Fact]
    public void ExponentialAliasExpansionIsRejected()
    {
        // A "billion laughs" YAML bomb: each level references the previous one multiple times,
        // so materializing the shared node graph into an independent JSON tree expands
        // exponentially. The conversion must fail fast instead of exhausting memory.
        var yamlBomb =
        """
        a: &a ["x","x","x","x","x","x","x","x","x"]
        b: &b [*a,*a,*a,*a,*a,*a,*a,*a,*a]
        c: &c [*b,*b,*b,*b,*b,*b,*b,*b,*b]
        d: &d [*c,*c,*c,*c,*c,*c,*c,*c,*c]
        e: &e [*d,*d,*d,*d,*d,*d,*d,*d,*d]
        f: &f [*e,*e,*e,*e,*e,*e,*e,*e,*e]
        g: &g [*f,*f,*f,*f,*f,*f,*f,*f,*f]
        h: &h [*g,*g,*g,*g,*g,*g,*g,*g,*g]
        i: &i [*h,*h,*h,*h,*h,*h,*h,*h,*h]
        """;

        Assert.Throws<OpenApiReaderException>(() => ConvertYamlStringToJsonNode(yamlBomb));
    }

    [Fact]
    public void ExcessiveNestingDepthIsRejected()
    {
        // Deeper than the conversion depth limit (mirrors the System.Text.Json default of 64),
        // which protects the recursive converter from stack exhaustion.
        const int depth = 70;
        var deeplyNested = new string('[', depth) + new string(']', depth);

        Assert.Throws<OpenApiReaderException>(() => ConvertYamlStringToJsonNode(deeplyNested));
    }

    [Fact]
    public void LegitimateAliasesStillConvert()
    {
        var yamlInput =
        """
        a: &val hello
        b: *val
        """;

        var jsonNode = Assert.IsType<JsonObject>(ConvertYamlStringToJsonNode(yamlInput));

        Assert.Equal("hello", jsonNode["a"]?.GetValue<string>());
        Assert.Equal("hello", jsonNode["b"]?.GetValue<string>());
    }

    [Fact]
    public void ConversionLimitsDefaultToDocumentedValues()
    {
        Assert.Equal(64u, YamlConverter.DefaultMaxDepth);
        Assert.Equal(5_000_000u, YamlConverter.DefaultMaxNodeCount);
        Assert.Equal(YamlConverter.DefaultMaxDepth, YamlConverter.MaxDepth);
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, YamlConverter.MaxNodeCount);
    }

    [Fact]
    public void SettingMaxDepthToZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxDepth = 0);
        // The invalid assignment must not have changed the effective limit.
        Assert.Equal(YamlConverter.DefaultMaxDepth, YamlConverter.MaxDepth);
    }

    [Fact]
    public void SettingMaxNodeCountToZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxNodeCount = 0);
        // The invalid assignment must not have changed the effective limit.
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, YamlConverter.MaxNodeCount);
    }

    [Fact]
    public void RaisingMaxDepthAllowsDocumentsDeeperThanTheDefault()
    {
        // A document nested deeper than the default depth limit (64) is rejected by default
        // but can be permitted by a consumer that opts into a higher limit.
        const int depth = 70;
        var deeplyNested = new string('[', depth) + new string(']', depth);

        try
        {
            YamlConverter.MaxDepth = depth + 10;
            var jsonNode = ConvertYamlStringToJsonNode(deeplyNested);
            Assert.IsType<JsonArray>(jsonNode);
        }
        finally
        {
            YamlConverter.MaxDepth = YamlConverter.DefaultMaxDepth;
        }
    }

    private static JsonNode ConvertYamlStringToJsonNode(string yamlInput)
    {
        var yamlDocument = new YamlStream();
        using var sr = new StringReader(yamlInput);
        yamlDocument.Load(sr);
        var yamlRoot = yamlDocument.Documents[0].RootNode;
        return yamlRoot.ToJsonNode();
    }

    private static string ConvertYamlNodeToString(YamlNode yamlNode)
    {
        using var ms = new MemoryStream();
        var yamlStream = new YamlStream(new YamlDocument(yamlNode));
        var writer = new StreamWriter(ms);
        yamlStream.Save(writer, isLastDocumentEndImplicit: true);
        writer.Flush();
        ms.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(ms);
        return reader.ReadToEnd();
    }
}
