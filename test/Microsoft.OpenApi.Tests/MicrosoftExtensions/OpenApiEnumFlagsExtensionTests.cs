using System.IO;
using Microsoft.OpenApi.MicrosoftExtensions;

using Microsoft.OpenApi.Writers;

using Xunit;

namespace Microsoft.OpenApi.Tests.MicrosoftExtensions;

public class OpenApiEnumFlagsExtensionTests
{
    [Fact]
    public void ExtensionNameMatchesExpected()
    {
        // Act
        var name = OpenApiEnumFlagsExtension.Name;
        var expectedName = "x-ms-enum-flags";

        // Assert
        Assert.Equal(expectedName, name);
    }

    [Fact]
    public void WritesDefaultValues()
    {
        // Arrange
        OpenApiEnumFlagsExtension extension = new();
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Contains("\"isFlags\": false", result);
        Assert.DoesNotContain("\"style\"", result);
        Assert.False(extension.IsFlags);
    }

    [Fact]
    public void WritesAllDefaultValues()
    {
        // Arrange
        OpenApiEnumFlagsExtension extension = new()
        {
            IsFlags = true
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Contains("\"isFlags\": true", result);
        Assert.True(extension.IsFlags);
    }

    [Fact]
    public void WritesAllValues()
    {
        // Arrange
        OpenApiEnumFlagsExtension extension = new()
        {
            IsFlags = true
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.True(extension.IsFlags);
        Assert.Contains("\"isFlags\": true", result);
    }
}

