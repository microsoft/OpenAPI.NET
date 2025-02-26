using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests;

public class OpenApiReaderSettingsExtensionsTests
{
    [Fact]
    public void AddsYamlReader()
    {
        var settings = new OpenApiReaderSettings();
        Assert.Single(settings.Readers);
        Assert.DoesNotContain(OpenApiConstants.Yaml, settings.Readers.Keys);
        Assert.DoesNotContain(OpenApiConstants.Yml, settings.Readers.Keys);

        settings.AddYamlReader();
        Assert.Equal(3, settings.Readers.Count);
        Assert.Contains(OpenApiConstants.Yaml, settings.Readers.Keys);
        Assert.Contains(OpenApiConstants.Yml, settings.Readers.Keys);
        Assert.IsType<OpenApiYamlReader>(settings.GetReader(OpenApiConstants.Yaml));
        Assert.IsType<OpenApiYamlReader>(settings.GetReader(OpenApiConstants.Yml));
    }
    [Fact]
    public void IsAvailableOnSameNamespace()
    {
        var settingsNS = typeof(OpenApiReaderSettings).Namespace;
        var extensionsNS = typeof(OpenApiReaderSettingsExtensions).Namespace;
        Assert.Equal(settingsNS, extensionsNS, StringComparer.Ordinal);
    }
}
