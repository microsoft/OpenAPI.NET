using System;
using System.IO;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Tests.MicrosoftExtensions;

public class OpenApiPagingExtensionsTests
{
    [Fact]
    public void ExtensionNameMatchesExpected()
    {
        // Act
        var name = OpenApiPagingExtension.Name;
        var expectedName = "x-ms-pageable";

        // Assert
        Assert.Equal(expectedName, name);
    }

    [Fact]
    public void ThrowsOnMissingWriter()
    {
        // Arrange
        OpenApiPagingExtension extension = new();

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() => extension.Write(null, OpenApiSpecVersion.OpenApi3_0));
    }

    [Fact]
    public void WritesNothingWhenNoValues()
    {
        // Arrange
        OpenApiPagingExtension extension = new();
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Equal("value", extension.ItemName);
        Assert.Equal("nextLink", extension.NextLinkName);
        Assert.Empty(extension.OperationName);
    }

    [Fact]
    public void WritesPagingInfo()
    {
        // Arrange
        OpenApiPagingExtension extension = new();
        extension.NextLinkName = "nextLink";
        extension.OperationName = "usersGet";
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Contains("value", result);
        Assert.Contains("itemName\": \"value", result);
        Assert.Contains("nextLinkName\": \"nextLink", result);
        Assert.Contains("operationName\": \"usersGet", result);
    }

    [Fact]
    public void ParsesPagingInfo()
    {
        // Arrange
        var obj = new JsonObject
        {
            ["nextLinkName"] = new OpenApiAny("@odata.nextLink").Node,
            ["operationName"] = new OpenApiAny("more").Node,
            ["itemName"] = new OpenApiAny("item").Node,
        };

        // Act
        var extension = OpenApiPagingExtension.Parse(obj);

        // Assert
        Assert.Equal("@odata.nextLink", extension.NextLinkName);
        Assert.Equal("item", extension.ItemName);
        Assert.Equal("more", extension.OperationName);
    }
}
