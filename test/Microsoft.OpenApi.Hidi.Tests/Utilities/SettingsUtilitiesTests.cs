using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Hidi.Utilities;
using Microsoft.OpenApi.OData;
using Xunit;

namespace Microsoft.OpenApi.Hidi.Tests;

public class SettingsUtilitiesTests
{
    [Fact]
    public void GetOpenApiConvertSettingsThrowsWhenConfigurationIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => SettingsUtilities.GetOpenApiConvertSettings(null!, null));
    }

    [Fact]
    public void GetOpenApiConvertSettingsUsesMetadataVersionWhenSectionIsMissing()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        var settings = SettingsUtilities.GetOpenApiConvertSettings(configuration, "2.1");

        Assert.Equal("2.1", settings.SemVerVersion);
    }

    [Fact]
    public void GetOpenApiConvertSettingsBindsConfiguredValuesOverMetadataVersion()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{nameof(OpenApiConvertSettings)}:{nameof(OpenApiConvertSettings.SemVerVersion)}"] = "3.0",
                [$"{nameof(OpenApiConvertSettings)}:{nameof(OpenApiConvertSettings.EnablePagination)}"] = bool.TrueString
            })
            .Build();

        var settings = SettingsUtilities.GetOpenApiConvertSettings(configuration, "2.1");

        Assert.Equal("3.0", settings.SemVerVersion);
        Assert.True(settings.EnablePagination);
    }
}
