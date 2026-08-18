using System;
using Microsoft.OpenApi.YamlReader;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Extensions for <see cref="OpenApiReaderSettings"/>
/// </summary>
public static class OpenApiReaderSettingsExtensions
{
    /// <summary>
    /// Adds a reader for the specified format
    /// </summary>
    /// <param name="settings">The settings to add the reader to.</param>
    public static void AddYamlReader(this OpenApiReaderSettings settings)
    {
        var yamlReader = new OpenApiYamlReader();
        settings.TryAddReader(OpenApiConstants.Yaml, yamlReader);
        settings.TryAddReader(OpenApiConstants.Yml, yamlReader);
    }

    /// <summary>
    /// Adds a YAML reader for the specified format using per-reader resource limits.
    /// </summary>
    /// <param name="settings">The settings to add the reader to.</param>
    /// <param name="yamlSettings">The YAML reader settings.</param>
    public static void AddYamlReader(this OpenApiReaderSettings settings, OpenApiYamlReaderSettings yamlSettings)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (yamlSettings is null) throw new ArgumentNullException(nameof(yamlSettings));
        var yamlReader = new OpenApiYamlReader(yamlSettings);
        settings.TryAddReader(OpenApiConstants.Yaml, yamlReader);
        settings.TryAddReader(OpenApiConstants.Yml, yamlReader);
    }
}
