using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Readers;

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
        settings.Readers.Add(OpenApiConstants.Yaml, yamlReader);
        settings.Readers.Add(OpenApiConstants.Yml, yamlReader);
    }
}
