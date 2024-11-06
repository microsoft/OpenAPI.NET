using System;
using System.IO;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Tests.MicrosoftExtensions;

public class OpenApiDeprecationExtensionTests
{
    [Fact]
    public void ExtensionNameMatchesExpected()
    {
        // Act
        var name = OpenApiDeprecationExtension.Name;
        var expectedName = "x-ms-deprecation";

        // Assert
        Assert.Equal(expectedName, name);
    }

    [Fact]
    public void WritesNothingWhenNoValues()
    {
        // Arrange
        OpenApiDeprecationExtension extension = new();
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Null(extension.Date);
        Assert.Null(extension.RemovalDate);
        Assert.Empty(extension.Version);
        Assert.Empty(extension.Description);
        Assert.Empty(result);
    }

    [Fact]
    public void WritesAllValues()
    {
        // Arrange
        OpenApiDeprecationExtension extension = new() {
            Date = new DateTime(2020, 1, 1),
            RemovalDate = new DateTime(2021, 1, 1),
            Version = "1.0.0",
            Description = "This is a test"
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.NotNull(extension.Date);
        Assert.NotNull(extension.RemovalDate);
        Assert.NotNull(extension.Version);
        Assert.NotNull(extension.Description);
        Assert.Contains("2021-01-01T00:00:00.000000", result);
        Assert.Contains("removalDate", result);
        Assert.Contains("version", result);
        Assert.Contains("1.0.0", result);
        Assert.Contains("description", result);
        Assert.Contains("This is a test", result);
    }
    [Fact]
    public void Parses()
    {
        var oaiValue = new JsonObject
        {
            { "date", new OpenApiAny(new DateTimeOffset(2023,05,04, 16, 0, 0, 0, 0, new(4, 0, 0))).Node},
            { "removalDate", new OpenApiAny(new DateTimeOffset(2023,05,04, 16, 0, 0, 0, 0, new(4, 0, 0))).Node},
            { "version", new OpenApiAny("v1.0").Node},
            { "description", new OpenApiAny("removing").Node}
        };
        var value = OpenApiDeprecationExtension.Parse(oaiValue);
        Assert.NotNull(value);
        Assert.Equal("v1.0", value.Version);
        Assert.Equal("removing", value.Description);
        Assert.Equal(new DateTimeOffset(2023, 05, 04, 16, 0, 0, 0, 0, new(4, 0, 0)), value.Date);
        Assert.Equal(new DateTimeOffset(2023, 05, 04, 16, 0, 0, 0, 0, new(4, 0, 0)), value.RemovalDate);
    }
    [Fact]
    public void Serializes()
    {
        var value = new OpenApiDeprecationExtension
        {
            Date = new DateTimeOffset(2023, 05, 04, 16, 0, 0, 0, 0, new(4, 0, 0)),
            RemovalDate = new DateTimeOffset(2023, 05, 04, 16, 0, 0, 0, 0, new(4, 0, 0)),
            Version = "v1.0",
            Description = "removing"
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        value.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();
        Assert.Equal("{\n  \"removalDate\": \"2023-05-04T16:00:00.0000000+04:00\",\n  \"date\": \"2023-05-04T16:00:00.0000000+04:00\",\n  \"version\": \"v1.0\",\n  \"description\": \"removing\"\n}", result);
    }
}
