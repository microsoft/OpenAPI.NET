// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace Microsoft.OpenApi.Tests;

public class JsonNullSentinelTests
{
    [Fact]
    public void JsonNull_ReturnsJsonValueWithSentinelString()
    {
        // Act
        var result = JsonNullSentinel.JsonNull;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(JsonValueKind.String, result.GetValueKind());
    }

    [Fact]
    public void JsonNull_ReturnsSameInstancesEachTime()
    {
        // Act
        var result1 = JsonNullSentinel.JsonNull;
        var result2 = JsonNullSentinel.JsonNull;

        // Assert
        Assert.Same(result1, result2);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsTrueForJsonNullSentinel()
    {
        // Arrange
        var sentinel = JsonNullSentinel.JsonNull;

        // Act
        var result = sentinel.IsJsonNullSentinel();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsTrueForMultipleSentinelInstances()
    {
        // Arrange
        var sentinel1 = JsonNullSentinel.JsonNull;
        var sentinel2 = JsonNullSentinel.JsonNull;

        // Act & Assert
        Assert.True(sentinel1.IsJsonNullSentinel());
        Assert.True(sentinel2.IsJsonNullSentinel());
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForNull()
    {
        // Arrange
        JsonNode nullNode = null;

        // Act
        var result = nullNode.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForJsonNull()
    {
        // Arrange
        var jsonNull = JsonValue.Create((string)null);

        // Act
        var result = jsonNull.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForRegularString()
    {
        // Arrange
        var regularString = JsonValue.Create("regular string");

        // Act
        var result = regularString.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForEmptyString()
    {
        // Arrange
        var emptyString = JsonValue.Create("");

        // Act
        var result = emptyString.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForNumber()
    {
        // Arrange
        var number = JsonValue.Create(42);

        // Act
        var result = number.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForBoolean()
    {
        // Arrange
        var boolean = JsonValue.Create(true);

        // Act
        var result = boolean.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForJsonObject()
    {
        // Arrange
        var jsonObject = new JsonObject();

        // Act
        var result = jsonObject.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForJsonArray()
    {
        // Arrange
        var jsonArray = new JsonArray();

        // Act
        var result = jsonArray.IsJsonNullSentinel();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsTrueForIdenticalString()
    {
        // Arrange
        var similarString1 = JsonValue.Create("openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E464");

        // Act & Assert
        Assert.True(similarString1.IsJsonNullSentinel());
    }

    [Fact]
    public void IsJsonNullSentinel_ReturnsFalseForSimilarStrings()
    {
        // Arrange
        var similarString1 = JsonValue.Create("openapi-json-null-sentinel-value");
        var similarString2 = JsonValue.Create("openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E465");
        var similarString3 = JsonValue.Create("OPENAPI-JSON-NULL-SENTINEL-VALUE-2BF93600-0FE4-4250-987A-E5DDB203E464");

        // Act & Assert
        Assert.False(similarString1.IsJsonNullSentinel());
        Assert.False(similarString2.IsJsonNullSentinel());
        Assert.False(similarString3.IsJsonNullSentinel());
    }

    [Fact]
    public void IsJsonNullSentinel_IsCaseSensitive()
    {
        // Arrange
        var lowercaseString = JsonValue.Create("openapi-json-null-sentinel-value-2bf93600-0fe4-4250-987a-e5ddb203e464");
        var uppercaseString = JsonValue.Create("OPENAPI-JSON-NULL-SENTINEL-VALUE-2BF93600-0FE4-4250-987A-E5DDB203E464");

        // Act & Assert
        Assert.False(lowercaseString.IsJsonNullSentinel());
        Assert.False(uppercaseString.IsJsonNullSentinel());
    }

    [Fact]
    public void JsonNullSentinel_CanBeAddedToJsonObject()
    {
        // Arrange
        var jsonObject = new JsonObject();
        var sentinel = JsonNullSentinel.JsonNull;

        // Act
        jsonObject["test"] = sentinel;

        // Assert
        Assert.True(jsonObject["test"].IsJsonNullSentinel());
    }

    [Fact]
    public void JsonNullSentinel_CanBeAddedToJsonArray()
    {
        // Arrange
        var jsonArray = new JsonArray();
        var sentinel = JsonNullSentinel.JsonNull.DeepClone();

        // Act
        jsonArray.Add(sentinel);

        // Assert
        Assert.True(jsonArray[0].IsJsonNullSentinel());
    }

    [Fact]
    public void JsonNullSentinel_SerializesToString()
    {
        // Arrange
        var sentinel = JsonNullSentinel.JsonNull;

        // Act
        var serialized = sentinel.ToJsonString();

        // Assert
        Assert.Contains("openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E464", serialized);
    }

    [Fact]
    public void JsonNullSentinel_DeserializedFromStringIsDetected()
    {
        // Arrange
        var sentinel = JsonNullSentinel.JsonNull;
        var serialized = sentinel.ToJsonString();

        // Act
        var deserialized = JsonNode.Parse(serialized);

        // Assert
        Assert.True(deserialized.IsJsonNullSentinel());
    }
}
