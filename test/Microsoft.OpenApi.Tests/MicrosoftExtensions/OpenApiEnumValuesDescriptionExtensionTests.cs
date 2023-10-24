using System.IO;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.MicrosoftExtensions;

public class OpenApiEnumValuesDescriptionExtensionTests
{
    [Fact]
    public void ExtensionNameMatchesExpected()
    {
        // Act
        var name = OpenApiEnumValuesDescriptionExtension.Name;
        var expectedName = "x-ms-enum";

        // Assert
        Assert.Equal(expectedName, name);
    }
    [Fact]
    public void WritesNothingWhenNoValues()
    {
        // Arrange
        OpenApiEnumValuesDescriptionExtension extension = new();
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Empty(extension.EnumName);
        Assert.Empty(extension.ValuesDescriptions);
        Assert.Empty(result);
    }
    [Fact]
    public void WritesEnumDescription()
    {
        // Arrange
        OpenApiEnumValuesDescriptionExtension extension = new()
        {
            EnumName = "TestEnum",
            ValuesDescriptions = new()
        {
            new() {
                Description = "TestDescription",
                Value = "TestValue",
                Name = "TestName"
            }
        }
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        // Act
        extension.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();

        // Assert
        Assert.Contains("values", result);
        Assert.Contains("modelAsString\": false", result);
        Assert.Contains("name\": \"TestEnum", result);
        Assert.Contains("description\": \"TestDescription", result);
        Assert.Contains("value\": \"TestValue", result);
        Assert.Contains("name\": \"TestName", result);
    }
}

