using System;
using Microsoft.OpenApi.MicrosoftExtensions;
using Xunit;

namespace Microsoft.OpenApi.Reader.Tests;

public class OpenApiReaderSettingsTests
{
    [Fact]
    public void Defensive()
    {
        var settings = new OpenApiReaderSettings();
        Assert.Throws<ArgumentNullException>(() => settings.GetReader(null));
        Assert.Throws<ArgumentNullException>(() => settings.GetReader(string.Empty));

        Assert.Throws<ArgumentNullException>(() => settings.TryAddReader(null, null));
        Assert.Throws<ArgumentNullException>(() => settings.TryAddReader(string.Empty, null));
        Assert.Throws<ArgumentNullException>(() => settings.TryAddReader(null, new OpenApiJsonReader()));
        Assert.Throws<ArgumentNullException>(() => settings.TryAddReader(string.Empty, new OpenApiJsonReader()));
        Assert.Throws<ArgumentNullException>(() => settings.TryAddReader("json", null));
    }

    [Fact]
    public void Defaults()
    {
        var settings = new OpenApiReaderSettings();
        Assert.NotNull(settings.HttpClient);

        Assert.IsType<OpenApiJsonReader>(settings.GetReader(OpenApiConstants.Json));
        Assert.Throws<NotSupportedException>(() =>settings.GetReader(OpenApiConstants.Yaml));
        Assert.Single(settings.Readers);

        Assert.Equal(StringComparer.OrdinalIgnoreCase, settings.Readers.Comparer);

        Assert.False(settings.TryAddReader("json", new OpenApiJsonReader()));
        Assert.Empty(settings.ExtensionParsers);
    }
    [Fact]
    public void InitializesReadersWithComparer()
    {
        var settings = new OpenApiReaderSettings
        {
            Readers = []
        };

        Assert.Equal(StringComparer.OrdinalIgnoreCase, settings.Readers.Comparer);
    }
    [Fact]
    public void AddsMicrosoftExtensions()
    {
        var settings = new OpenApiReaderSettings();
        Assert.Empty(settings.ExtensionParsers);
        settings.AddMicrosoftExtensionParsers();

        Assert.NotEmpty(settings.ExtensionParsers);
        Assert.Contains(OpenApiPagingExtension.Name, settings.ExtensionParsers.Keys);
        Assert.Contains(OpenApiEnumValuesDescriptionExtension.Name, settings.ExtensionParsers.Keys);
        Assert.Contains(OpenApiPrimaryErrorMessageExtension.Name, settings.ExtensionParsers.Keys);
        Assert.Contains(OpenApiDeprecationExtension.Name, settings.ExtensionParsers.Keys);
        Assert.Contains(OpenApiReservedParameterExtension.Name, settings.ExtensionParsers.Keys);
        Assert.Contains(OpenApiEnumFlagsExtension.Name, settings.ExtensionParsers.Keys);
    }
    [Fact]
    public void AddsJsonReader()
    {
        var settings = new OpenApiReaderSettings()
        {
            Readers = []
        };

        Assert.Empty(settings.Readers);

        settings.AddJsonReader();
        Assert.Single(settings.Readers);
        Assert.IsType<OpenApiJsonReader>(settings.GetReader(OpenApiConstants.Json));
    }
}
