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
        settings.AddReaderToSettings(OpenApiConstants.Yaml, yamlReader);
        settings.AddReaderToSettings(OpenApiConstants.Yml, yamlReader);
    }
    private static void AddReaderToSettings(this OpenApiReaderSettings settings, string format, IOpenApiReader reader)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
        settings.Readers.TryAdd(format, reader);
#else
        if (!settings.Readers.ContainsKey(format))
        {
            settings.Readers.Add(format, reader);
        }
#endif
    }
}
