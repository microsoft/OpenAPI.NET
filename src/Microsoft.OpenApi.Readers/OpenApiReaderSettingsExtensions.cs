using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

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
}
